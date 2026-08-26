using System;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

/// <summary>
/// Temporary probe for the Newtonsoft migration: checks on a real IL2CPP build what the plan
/// assumes from CoreCLR behaviour. Delete once the migration is done.
///
/// The decisive question is whether Newtonsoft can assign initonly fields through reflection
/// under AOT — the whole "keep 155 readonly fields as they are" decision rests on it.
/// </summary>
public static class AotSerializationProbe
{
    [DataContract]
    private class Probe
    {
        [DataMember(Name = "flow_id", IsRequired = true)]
        public readonly string InstanceIdentity;

        [DataMember(Name = "payload_data")]
        private readonly string _PayloadData;

        [DataMember(Name = "count")]
        public readonly int Count;

        [DataMember(Name = "offer_type")]
        public readonly ProbeEnum OfferType;

        public string PayloadData => _PayloadData;
    }

    private enum ProbeEnum
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        [EnumMember(Value = "win_back")]
        WinBack,
    }

    private class StrictResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
        {
            var property = base.CreateProperty(member, serialization);
            if (property.Required == Required.AllowNull)
            {
                property.Required = Required.Always;
            }
            return property;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Run()
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new StrictResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
        };

        Report("scripting-backend", Application.platform + " il2cpp=" + IsIl2Cpp());

        Check("readonly-fields", () =>
        {
            var probe = JsonConvert.DeserializeObject<Probe>(
                "{\"flow_id\":\"f-1\",\"payload_data\":\"{}\",\"count\":7,\"offer_type\":\"win_back\"}",
                settings
            );
            return "public-readonly=" + (probe.InstanceIdentity ?? "<null>")
                + " private-readonly=" + (probe.PayloadData ?? "<null>")
                + " readonly-int=" + probe.Count
                + " readonly-enum=" + probe.OfferType;
        });

        Check("required-missing", () =>
        {
            JsonConvert.DeserializeObject<Probe>("{\"count\":1}", settings);
            return "no exception (UNEXPECTED)";
        });

        Check("required-null", () =>
        {
            JsonConvert.DeserializeObject<Probe>("{\"flow_id\":null}", settings);
            return "no exception (UNEXPECTED)";
        });

        Check("enum-write", () =>
            JsonConvert.SerializeObject(
                JsonConvert.DeserializeObject<Probe>("{\"flow_id\":\"f\",\"offer_type\":\"win_back\"}", settings),
                settings
            )
        );

        Check("dictionary-object", () =>
        {
            var parsed = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(
                "{\"n\":42,\"s\":\"x\",\"nested\":{\"k\":true}}",
                settings
            );
            return "n=" + parsed["n"].GetType().Name + " nested=" + parsed["nested"].GetType().Name;
        });
    }

    private static bool IsIl2Cpp()
    {
#if ENABLE_IL2CPP
        return true;
#else
        return false;
#endif
    }

    private static void Check(string name, Func<string> action)
    {
        try
        {
            Report(name, action());
        }
        catch (Exception e)
        {
            Report(name, "threw " + e.GetType().Name + ": " + e.Message);
        }
    }

    private static void Report(string name, string result) =>
        Debug.Log("[AOT-PROBE] " + name + " -> " + result);
}
