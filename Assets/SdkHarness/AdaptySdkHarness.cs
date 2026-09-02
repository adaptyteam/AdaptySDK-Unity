using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AdaptySDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Pipeline.Commands;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace AdaptyExample.SdkHarness
{
    /// <summary>
    /// Drives the SDK's public C# API over the Pipeline command channel on a player where the native
    /// SDK is real, and hands back what the SDK decoded — the round-trip the desktop suite cannot make.
    /// </summary>
    /// <remarks>
    /// Every call goes through the typed public surface: the SDK encodes the request and decodes the
    /// reply itself, which is what is under test. The harness only binds arguments, waits, and reports.
    ///
    /// Commands run on the request thread, not the main one. A call is posted to the main thread and
    /// awaited here, because the SDK delivers its callback through <c>SynchronizationContext.Post</c>
    /// and the main loop has to stay free to run it — waiting on the main thread would deadlock every
    /// call. The wait budget stays below the scene's 120 s request timeout.
    ///
    /// A player discovers commands by reflection and reaches the harness from nothing else, so
    /// <c>link.xml</c> beside this file keeps the namespace and the whole SDK assembly, and each
    /// handler carries <see cref="PreserveAttribute"/> as well.
    /// </remarks>
    public sealed class AdaptySdkHarness
        : MonoBehaviour,
            IAdaptyEventListener,
            IAdaptyFlowsEventsListener,
            IAdaptyUISystemRequestsHandler,
            IAdaptyUIObserverModeResolver
    {
        private const string DemoApiKey = "public_live_iNuUlSsN.83zcTTR8D5Y8FI9cGUI6";
        private const int DefaultTimeoutMs = 20000;

        private static SynchronizationContext s_Main;
        private static bool s_Activated;
        private static readonly object s_Gate = new object();
        private static readonly List<JObject> s_Events = new List<JObject>();

        /// <summary>
        /// The on-screen ledger of what the harness did, for whoever is watching the simulator. The
        /// agent reads the command results; a person reads this.
        /// </summary>
        [SerializeField]
        private PanelSettings m_Panel;

        private static AdaptySdkHarness s_Instance;
        private ScrollView m_Ledger;

        // AdaptyJson is internal. Its two entry points are what keep this harness on the SDK's own wire
        // format instead of a second serializer that would hide a converter bug behind a symmetric one.
        private static readonly Type s_Json = typeof(Adapty).Assembly.GetType(
            "AdaptySDK.Serialization.AdaptyJson"
        );
        private static readonly MethodInfo s_Serialize = s_Json.GetMethod(
            "Serialize",
            BindingFlags.NonPublic | BindingFlags.Static
        );
        private static readonly MethodInfo s_CreateSerializer = s_Json.GetMethod(
            "CreateSerializer",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        private void Awake()
        {
            s_Main = SynchronizationContext.Current;
            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            Adapty.SetEventListener(this);
            Adapty.SetFlowsEventsListener(this);
            Adapty.SetSystemRequestsHandler(this);
            Adapty.SetObserverModeResolver(this);
        }

        /// <summary>
        /// Built in code rather than UXML: three elements do not earn an asset, and the one asset
        /// it does need — the panel with its theme — is what the scene references.
        /// </summary>
        private void Start()
        {
            if (m_Panel == null)
            {
                return;
            }

            var document = gameObject.AddComponent<UIDocument>();
            document.panelSettings = m_Panel;

            var root = document.rootVisualElement;
            root.style.flexGrow = 1;
            root.style.paddingTop = 54;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;
            root.style.paddingBottom = 24;
            root.style.backgroundColor = new Color(0.07f, 0.07f, 0.09f);

            var title = new Label($"Adapty SDK harness · SDK {Adapty.SDKVersion} · pipeline :7900");
            title.style.color = new Color(0.6f, 0.6f, 0.65f);
            title.style.fontSize = 12;
            title.style.marginBottom = 8;
            root.Add(title);

            m_Ledger = new ScrollView(ScrollViewMode.Vertical);
            m_Ledger.style.flexGrow = 1;
            // Follow the newest line once its layout exists — scheduling right after Add runs before
            // the new label has a height, and the offset clamps to the previous bottom.
            m_Ledger.contentContainer.RegisterCallback<GeometryChangedEvent>(
                _ => m_Ledger.scrollOffset = new Vector2(0, float.MaxValue)
            );
            Restyle(m_Ledger.verticalScroller);
            root.Add(m_Ledger);

            Ledger("·", "waiting for commands");
        }

        /// <summary>
        /// The default theme's scroller is a desktop one — arrow buttons, a framed track, a column
        /// of its own. On a phone the ledger wants a thin translucent indicator laid over the text.
        /// </summary>
        private static void Restyle(Scroller scroller)
        {
            scroller.lowButton.style.display = DisplayStyle.None;
            scroller.highButton.style.display = DisplayStyle.None;

            scroller.style.position = Position.Absolute;
            scroller.style.top = 0;
            scroller.style.bottom = 0;
            scroller.style.right = 0;
            scroller.style.width = 6;
            scroller.style.backgroundColor = Color.clear;

            var slider = scroller.slider;
            slider.style.marginTop = 0;
            slider.style.marginBottom = 0;
            slider.style.backgroundColor = Color.clear;
            Flat(slider);

            var tracker = slider.Q("unity-tracker");

            if (tracker != null)
            {
                tracker.style.backgroundColor = Color.clear;
                Flat(tracker);
            }

            var dragger = slider.Q("unity-dragger");

            if (dragger != null)
            {
                dragger.style.width = 4;
                dragger.style.left = 1;
                dragger.style.backgroundColor = new Color(1f, 1f, 1f, 0.3f);
                dragger.style.borderTopLeftRadius = 2;
                dragger.style.borderTopRightRadius = 2;
                dragger.style.borderBottomLeftRadius = 2;
                dragger.style.borderBottomRightRadius = 2;
                Flat(dragger);
            }
        }

        private static void Flat(VisualElement element)
        {
            element.style.borderTopWidth = 0;
            element.style.borderBottomWidth = 0;
            element.style.borderLeftWidth = 0;
            element.style.borderRightWidth = 0;
        }

        /// <summary>
        /// Appends one line, from any thread. The mark says what kind of line it is: → request,
        /// ← reply, × failure, « event delivered to a listener.
        /// </summary>
        private static void Ledger(string mark, string text)
        {
            var harness = s_Instance;

            if (harness == null || harness.m_Ledger == null || s_Main == null)
            {
                return;
            }

            var body = text.Length > 320 ? text.Substring(0, 320) + "…" : text;

            s_Main.Post(
                _ =>
                {
                    var line = new Label($"{Time.realtimeSinceStartup,6:F1}s  {mark}  {body}");
                    line.style.whiteSpace = WhiteSpace.Normal;
                    line.style.fontSize = 11;
                    line.style.marginBottom = 3;
                    line.style.color = mark switch
                    {
                        "→" => new Color(0.55f, 0.75f, 1f),
                        "←" => new Color(0.6f, 0.9f, 0.6f),
                        "×" => new Color(1f, 0.5f, 0.5f),
                        "«" => new Color(1f, 0.85f, 0.4f),
                        _ => new Color(0.6f, 0.6f, 0.65f),
                    };

                    var content = harness.m_Ledger.contentContainer;
                    content.Add(line);

                    while (content.childCount > 120)
                    {
                        content.RemoveAt(0);
                    }
                },
                null
            );
        }

        [CliCommand("harness_status", "Whether the SDK was activated through the harness, and how many events it buffered.",
            MainThreadRequired = false, Tags = new[] { "adapty" })]
        [Preserve]
        public static object Status()
        {
            lock (s_Gate)
            {
                return new
                {
                    activated = s_Activated,
                    events = s_Events.Count,
                    sdk = Adapty.SDKVersion,
                    platform = Application.platform.ToString(),
                };
            }
        }

        /// <summary>
        /// Activation is the one call whose argument, <c>AdaptyConfiguration</c>, has no JSON reader —
        /// it is request-only — so it is built here from the builder instead of bound by name.
        /// </summary>
        [CliCommand("harness_activate", "Activate the SDK through AdaptyConfiguration.Builder and wait for the callback.",
            MainThreadRequired = false, Tags = new[] { "adapty" })]
        [Preserve]
        public static object Activate(
            [CliArg("api_key", "Public API key. Defaults to the demo app's.")] string apiKey = DemoApiKey,
            [CliArg("customer_user_id", "Customer user id; empty for none.")] string customerUserId = "",
            [CliArg("observer_mode", "Observer mode.")] bool observerMode = false,
            [CliArg("activate_ui", "Activate AdaptyUI as well.")] bool activateUi = true,
            [CliArg("configuration", "JSON object for the rest of AdaptyConfiguration.Builder: each key is a Set<Key> call, e.g. {\"serverCluster\":\"EU\",\"appleIDFACollectionDisabled\":true}.")] string configuration = "{}",
            [CliArg("timeout_ms", "How long to wait for the callback.")] int timeoutMs = DefaultTimeoutMs)
        {
            var builder = new AdaptyConfiguration.Builder(apiKey)
                .SetCustomerUserId(string.IsNullOrEmpty(customerUserId) ? null : customerUserId)
                .SetObserverMode(observerMode)
                .SetActivateUI(activateUi);
            Apply(builder, JObject.Parse(configuration), (JsonSerializer)s_CreateSerializer.Invoke(null, null));

            Ledger("→", $"Activate  key …{apiKey.Substring(Math.Max(0, apiKey.Length - 6))}  observer={observerMode}  ui={activateUi}"
                + (string.IsNullOrEmpty(customerUserId) ? "" : "  user=" + customerUserId));

            var call = new Call();
            var result = call.Run(() => Adapty.Activate(builder, call.OnVoid), timeoutMs, "Activate");

            if (result.ok)
            {
                lock (s_Gate)
                {
                    s_Activated = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Any other public method: the overload is picked by the argument names supplied, the
        /// arguments are read with the SDK's own serializer, and the callback's payload comes back
        /// serialized by it too.
        /// </summary>
        [CliCommand("harness_call", "Call a public Adapty method by name with JSON arguments, wait for its callback, return what the SDK decoded.",
            MainThreadRequired = false, Tags = new[] { "adapty" })]
        [Preserve]
        public static object CallSdk(
            [CliArg("method", "Public static method on Adapty, e.g. GetProfile or GetFlow.", Required = true)] string method,
            [CliArg("args", "JSON object of arguments keyed by parameter name.")] string args = "{}",
            [CliArg("timeout_ms", "How long to wait for the callback.")] int timeoutMs = DefaultTimeoutMs)
        {
            if (method == nameof(Adapty.Activate))
            {
                return Fail(method, "use harness_activate");
            }

            JObject provided;

            try
            {
                provided = JObject.Parse(string.IsNullOrWhiteSpace(args) ? "{}" : args);
            }
            catch (Exception e)
            {
                return Fail(method, "args is not a JSON object: " + e.Message);
            }

            // The public surface is two static classes: Adapty, and AdaptyUI for the flow views.
            var candidates = new[] { typeof(Adapty), typeof(AdaptyUI) }
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(m => m.Name == method)
                .ToList();

            if (candidates.Count == 0)
            {
                return Fail(method, $"neither Adapty.{method} nor AdaptyUI.{method} exists");
            }

            var chosen = candidates.FirstOrDefault(m => Matches(m, provided));

            if (chosen == null)
            {
                return Fail(
                    method,
                    $"no overload takes exactly [{string.Join(", ", provided.Properties().Select(p => p.Name))}]; "
                        + "options: " + string.Join(" | ", candidates.Select(Describe))
                );
            }

            var call = new Call();
            object[] bound;

            try
            {
                bound = Bind(chosen, provided, call);
            }
            catch (Exception e)
            {
                return Fail(method, "binding: " + e.Message);
            }

            Ledger("→", $"{method}  {provided.ToString(Formatting.None)}");

            return call.Run(() => chosen.Invoke(null, bound), timeoutMs, method);
        }

        [CliCommand("harness_events", "Events the SDK delivered to the harness's listeners since launch, or since the last clear.",
            MainThreadRequired = false, Tags = new[] { "adapty" })]
        [Preserve]
        public static object Events([CliArg("clear", "Empty the buffer after reading.")] bool clear = false)
        {
            lock (s_Gate)
            {
                var copy = s_Events.ToList();

                if (clear)
                {
                    s_Events.Clear();
                }

                return new { count = copy.Count, events = copy };
            }
        }

        private static bool Matches(MethodInfo method, JObject provided)
        {
            var names = method
                .GetParameters()
                .Where(p => !typeof(Delegate).IsAssignableFrom(p.ParameterType))
                .Select(p => p.Name)
                .ToHashSet(StringComparer.Ordinal);

            return names.SetEquals(provided.Properties().Select(p => p.Name));
        }

        private static string Describe(MethodInfo method)
        {
            return method.Name + "("
                + string.Join(", ", method.GetParameters()
                    .Where(p => !typeof(Delegate).IsAssignableFrom(p.ParameterType))
                    .Select(p => p.Name))
                + ")";
        }

        private static object[] Bind(MethodInfo method, JObject provided, Call call)
        {
            var serializer = (JsonSerializer)s_CreateSerializer.Invoke(null, null);
            var parameters = method.GetParameters();
            var bound = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                bound[i] = typeof(Delegate).IsAssignableFrom(parameter.ParameterType)
                    ? call.CallbackFor(parameter.ParameterType)
                    : Read(provided[parameter.Name], parameter.ParameterType, serializer);
            }

            return bound;
        }

        /// <summary>
        /// Reads one argument. Two request-side shapes have no JSON reader of their own, so they get
        /// one here: a type that publishes its instances as public static members, like
        /// <see cref="AdaptyPlacementFetchPolicy"/>, is addressed by member name, and a
        /// <see cref="TimeSpan"/> is taken as seconds. Everything else goes through the SDK's reader,
        /// enums included — their string form is the SDK's own converter's business.
        /// </summary>
        private static object Read(JToken token, Type type, JsonSerializer serializer)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.String && token.Value<string>().StartsWith("$", StringComparison.Ordinal))
            {
                return Recall(token.Value<string>(), type);
            }

            var inner = Nullable.GetUnderlyingType(type) ?? type;

            if (inner == typeof(TimeSpan) && (token.Type == JTokenType.Integer || token.Type == JTokenType.Float))
            {
                return TimeSpan.FromSeconds(token.Value<double>());
            }

            if (token.Type == JTokenType.String && !inner.IsEnum && inner != typeof(string))
            {
                var name = token.Value<string>();
                var member = inner
                    .GetMembers(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)
                        && (m is FieldInfo f && f.FieldType == inner || m is PropertyInfo p && p.PropertyType == inner)
                    );

                if (member != null)
                {
                    return member is FieldInfo field ? field.GetValue(null) : ((PropertyInfo)member).GetValue(null);
                }

                // A type made from one string — a provider by its identifier.
                var fromString = inner.GetConstructor(new[] { typeof(string) });

                if (fromString != null)
                {
                    return fromString.Invoke(new object[] { name });
                }
            }

            // Request-only models (AdaptyProfileParameters, AdaptyConfiguration) have no reader, only a
            // nested Builder: constructor arguments by parameter name, the other keys as Set<Key>.
            if (token.Type == JTokenType.Object && inner.GetNestedType("Builder") is Type builderType)
            {
                var json = (JObject)token;
                var constructor = builderType.GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault(c => c.GetParameters().All(a => json.GetValue(a.Name, StringComparison.OrdinalIgnoreCase) != null))
                    ?? throw new ArgumentException($"{inner.Name}.Builder needs {string.Join(" or ", builderType.GetConstructors().Select(c => "(" + string.Join(", ", c.GetParameters().Select(a => a.Name)) + ")"))}");
                var arguments = constructor.GetParameters().Select(a => a.Name).ToArray();
                var builder = constructor.Invoke(arguments.Select((a, i) => Read(json.GetValue(a, StringComparison.OrdinalIgnoreCase), constructor.GetParameters()[i].ParameterType, serializer)).ToArray());
                Apply(builder, new JObject(json.Properties().Where(x => !arguments.Contains(x.Name, StringComparer.OrdinalIgnoreCase))), serializer);
                return builderType.GetMethod("Build").Invoke(builder, null);
            }

            return token.ToObject(type, serializer);
        }

        /// <summary>
        /// Every key of <paramref name="json"/> as a <c>Set&lt;Key&gt;</c> call on <paramref name="builder"/>.
        /// <c>customAttributes</c> is the one two-argument setter: an object whose entries go to
        /// <c>SetCustomStringAttribute</c> or <c>SetCustomDoubleAttribute</c> by value type.
        /// </summary>
        private static void Apply(object builder, JObject json, JsonSerializer serializer)
        {
            var builderType = builder.GetType();

            foreach (var property in json.Properties())
            {
                if (string.Equals(property.Name, "customAttributes", StringComparison.OrdinalIgnoreCase) && property.Value is JObject attributes)
                {
                    foreach (var attribute in attributes.Properties())
                    {
                        var isNumber = attribute.Value.Type == JTokenType.Integer || attribute.Value.Type == JTokenType.Float;
                        builderType.GetMethod(isNumber ? "SetCustomDoubleAttribute" : "SetCustomStringAttribute")
                            .Invoke(builder, new[] { attribute.Name, isNumber ? attribute.Value.Value<double>() : (object)attribute.Value.Value<string>() });
                    }

                    continue;
                }

                var setter = builderType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m => m.GetParameters().Length == 1 && string.Equals(m.Name, "Set" + property.Name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new ArgumentException($"{builderType.DeclaringType?.Name}.Builder has no single-argument Set{property.Name}");
                setter.Invoke(builder, new[] { Read(property.Value, setter.GetParameters()[0].ParameterType, serializer) });
            }
        }

        /// <summary>
        /// What the last successful call of each method handed back, as the object itself.
        /// </summary>
        /// <remarks>
        /// An argument written as <c>"$GetPaywallProducts[0]"</c> or <c>"$GetFlow"</c> is that
        /// object, not a re-read of its JSON. The SDK never reads a response model back from JSON —
        /// a product goes out on a purchase as <c>AdaptyPaywallProductRequest</c>, a shape of its
        /// own — so a response model has no complete reader, and feeding its dump back would fail on
        /// the first one that lacks one (the subscription offer) while proving nothing about the SDK.
        /// </remarks>
        private static readonly Dictionary<string, object> s_Results = new Dictionary<string, object>(StringComparer.Ordinal);

        private static object Recall(string reference, Type type)
        {
            var open = reference.IndexOf('[');
            var name = (open < 0 ? reference : reference.Substring(0, open)).Substring(1);
            object value;

            lock (s_Gate)
            {
                if (!s_Results.TryGetValue(name, out value))
                {
                    throw new ArgumentException($"{reference}: no successful {name} to recall; known: " + string.Join(", ", s_Results.Keys));
                }
            }

            if (open >= 0)
            {
                var index = int.Parse(reference.Substring(open + 1).TrimEnd(']'));
                var list = value as System.Collections.IList;

                if (list == null)
                {
                    throw new ArgumentException($"{reference}: {name} returned a {value?.GetType().Name}, not a list");
                }

                value = list[index];
            }

            if (value != null && !type.IsInstanceOfType(value))
            {
                throw new ArgumentException($"{reference} is a {value.GetType().Name}, the parameter wants {type.Name}");
            }

            return value;
        }

        private static JToken ToJson(object value)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                return JToken.Parse((string)s_Serialize.Invoke(null, new[] { value }));
            }
            catch (Exception e)
            {
                return new JObject { ["unserializable"] = value.ToString(), ["because"] = e.Message };
            }
        }

        private static CallResult Fail(string method, string error)
        {
            Ledger("×", $"{method}  {error}");
            return new CallResult { method = method, ok = false, error = error };
        }

        /// <summary>
        /// The two callback shapes with a value-type payload, instantiated so IL2CPP compiles them.
        /// Reference-type payloads share one instantiation and need nothing. Never called.
        /// </summary>
        [Preserve]
        private static void AotHints()
        {
            new Capture<bool>(null).On(false, null);
            new Capture<AdaptyLogLevel>(null).On(default, null);
        }

        [Preserve]
        public sealed class CallResult
        {
            public string method;
            public bool ok;
            public bool timedOut;
            public string error;
            public string exception;
            public JToken result;
        }

        /// <summary>
        /// One SDK call: posts it to the main thread, waits on the request thread for whichever
        /// comes first — the callback, an exception out of the call itself, or the deadline.
        /// </summary>
        private sealed class Call
        {
            private readonly ManualResetEventSlim m_Done = new ManualResetEventSlim();

            private object m_Value;
            private AdaptyError m_Error;
            private Exception m_Thrown;
            private string m_Method;
            private DateTime m_Started;
            private volatile bool m_TimedOut;

            public void OnVoid(AdaptyError error) => Complete(null, error);

            public void Complete(object value, AdaptyError error)
            {
                m_Value = value;
                m_Error = error;
                m_Done.Set();

                // A reply after the budget is still a reply: it goes on the ledger and, if it carried
                // an object, becomes the handle — the command that waited for it has already returned.
                if (m_TimedOut)
                {
                    Ledger(error == null ? "←" : "×", $"{m_Method}  late {(DateTime.UtcNow - m_Started).TotalSeconds:F2}s  " + (error?.ToString() ?? "ok"));

                    if (error == null && value != null)
                    {
                        lock (s_Gate)
                        {
                            s_Results[m_Method] = value;
                        }
                    }
                }
            }

            public Delegate CallbackFor(Type callbackType)
            {
                var payload = callbackType.IsGenericType ? callbackType.GetGenericArguments() : Type.EmptyTypes;

                if (payload.Length == 1 && payload[0] == typeof(AdaptyError))
                {
                    return Delegate.CreateDelegate(callbackType, this, typeof(Call).GetMethod(nameof(OnVoid)));
                }

                if (payload.Length == 2 && payload[1] == typeof(AdaptyError))
                {
                    var capture = Activator.CreateInstance(typeof(Capture<>).MakeGenericType(payload[0]), this);
                    return Delegate.CreateDelegate(callbackType, capture, capture.GetType().GetMethod("On"));
                }

                throw new NotSupportedException("callback shape " + callbackType);
            }

            public CallResult Run(Action invoke, int timeoutMs, string method)
            {
                m_Method = method;
                m_Started = DateTime.UtcNow;
                s_Main.Post(
                    _ =>
                    {
                        try
                        {
                            invoke();
                        }
                        catch (Exception e)
                        {
                            m_Thrown = e is TargetInvocationException t && t.InnerException != null ? t.InnerException : e;
                            m_Done.Set();
                        }
                    },
                    null
                );

                var started = DateTime.UtcNow;
                var finished = m_Done.Wait(Math.Max(1000, timeoutMs));
                var elapsed = (DateTime.UtcNow - started).TotalSeconds;
                m_TimedOut = !finished;

                var outcome = new CallResult
                {
                    method = method,
                    ok = finished && m_Thrown == null && m_Error == null,
                    timedOut = !finished,
                    error = m_Error?.ToString(),
                    exception = m_Thrown?.ToString(),
                    result = ToJson(m_Value),
                };

                if (outcome.ok && m_Value != null)
                {
                    lock (s_Gate)
                    {
                        s_Results[method] = m_Value;
                    }
                }

                Ledger(
                    outcome.ok ? "←" : "×",
                    $"{method}  {elapsed:F2}s  "
                        + (outcome.timedOut ? "timed out"
                            : outcome.error ?? outcome.exception?.Split('\n')[0]
                            ?? outcome.result?.ToString(Formatting.None) ?? "ok")
                );

                return outcome;
            }
        }

        private sealed class Capture<T>
        {
            private readonly Call m_Call;

            public Capture(Call call) => m_Call = call;

            public void On(T value, AdaptyError error) => m_Call.Complete(value, error);
        }

        private static void Record(string name, params (string key, object value)[] args)
        {
            var entry = new JObject { ["event"] = name, ["at"] = Time.realtimeSinceStartup };
            var payload = new JObject();

            foreach (var (key, value) in args)
            {
                payload[key] = ToJson(value);
            }

            entry["args"] = payload;

            lock (s_Gate)
            {
                s_Events.Add(entry);
            }

            Ledger("«", $"{name}  {payload.ToString(Formatting.None)}");
        }

        public void OnLoadLatestProfile(AdaptyProfile profile) => Record("did_load_latest_profile", ("profile", profile));

        public void OnReceivePromotedPurchase(AdaptyPromotedProduct product) => Record("did_receive_promoted_purchase", ("product", product));

        public void OnInstallationDetailsSuccess(AdaptyInstallationDetails details) => Record("installation_details_success", ("details", details));

        public void OnInstallationDetailsFail(AdaptyError error) => Record("installation_details_fail", ("error", error));

        public void FlowViewDidAppear(AdaptyUIFlowView view) => Record("flow_view_did_appear", ("view", view));

        public void FlowViewDidDisappear(AdaptyUIFlowView view) => Record("flow_view_did_disappear", ("view", view));

        public void FlowViewDidPerformAction(AdaptyUIFlowView view, AdaptyUIUserAction action) =>
            Record("flow_view_did_perform_action", ("view", view), ("action", action));

        public void FlowViewDidSelectProduct(AdaptyUIFlowView view, string productId) =>
            Record("flow_view_did_select_product", ("view", view), ("productId", productId));

        public void FlowViewDidStartPurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product) =>
            Record("flow_view_did_start_purchase", ("view", view), ("product", product));

        public void FlowViewDidFinishPurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product, AdaptyPurchaseResult purchasedResult) =>
            Record("flow_view_did_finish_purchase", ("view", view), ("product", product), ("result", purchasedResult));

        public void FlowViewDidFailPurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product, AdaptyError error) =>
            Record("flow_view_did_fail_purchase", ("view", view), ("product", product), ("error", error));

        public void FlowViewDidStartRestore(AdaptyUIFlowView view) => Record("flow_view_did_start_restore", ("view", view));

        public void FlowViewDidFinishRestore(AdaptyUIFlowView view, AdaptyProfile profile) =>
            Record("flow_view_did_finish_restore", ("view", view), ("profile", profile));

        public void FlowViewDidFailRestore(AdaptyUIFlowView view, AdaptyError error) =>
            Record("flow_view_did_fail_restore", ("view", view), ("error", error));

        public void FlowViewDidReceiveError(AdaptyUIFlowView view, AdaptyError error) =>
            Record("flow_view_did_receive_error", ("view", view), ("error", error));

        public void FlowViewDidFailLoadingProducts(AdaptyUIFlowView view, AdaptyError error) =>
            Record("flow_view_did_fail_loading_products", ("view", view), ("error", error));

        public void FlowViewDidFinishWebPaymentNavigation(AdaptyUIFlowView view, AdaptyPaywallProduct product, AdaptyError error) =>
            Record("flow_view_did_finish_web_payment_navigation", ("view", view), ("product", product), ("error", error));

        public void FlowViewDidReceiveAnalyticEvent(AdaptyUIFlowView view, string name, IReadOnlyDictionary<string, object> parameters) =>
            Record("flow_view_did_receive_analytic_event", ("view", view), ("name", name), ("parameters", parameters));

        /// <summary>
        /// Recorded and granted: the harness has no user to ask, and a request left unanswered would hold
        /// the view open for the rest of the run.
        /// </summary>
        public void FlowViewDidAskPermission(AdaptyUIFlowView view, string permission, IReadOnlyDictionary<string, string> customArgs, Action<bool, string> respond)
        {
            Record("flow_view_did_ask_permission", ("view", view), ("permission", permission), ("customArgs", customArgs));
            respond(true, null);
        }

        public void FlowViewDidRequestAppReview(AdaptyUIFlowView view) => Record("flow_view_did_request_app_review", ("view", view));

        /// <summary>
        /// Observer mode leaves the purchase to the app; the harness has none, so it reports both stages
        /// at once and lets the view continue.
        /// </summary>
        public void FlowViewDidInitiatePurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product, Action onStartPurchase, Action onFinishPurchase)
        {
            Record("flow_view_did_initiate_purchase", ("view", view), ("product", product));
            onStartPurchase();
            onFinishPurchase();
        }

        public void FlowViewDidInitiateRestore(AdaptyUIFlowView view, Action onStartRestore, Action onFinishRestore)
        {
            Record("flow_view_did_initiate_restore", ("view", view));
            onStartRestore();
            onFinishRestore();
        }
    }
}
