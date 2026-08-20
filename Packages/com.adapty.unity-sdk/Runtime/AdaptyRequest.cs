using System;
using System.Runtime.CompilerServices;
#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif
using AdaptySDK.Serialization;
using Newtonsoft.Json.Linq;

namespace AdaptySDK
{
    /// <summary>
    /// The one way a public method reaches the native side, and the one place a request names
    /// itself when the app's completion handler throws.
    /// </summary>
    /// <remarks>
    /// <see cref="Send{T}"/> and <see cref="SendVoid"/> are the only entry points: the raw
    /// transport is private, so a call site cannot reach the bridge without the guard, and neither
    /// can spell the diagnostic wrong - <c>[CallerMemberName]</c> is what fills the name in.
    /// </remarks>
    internal static class AdaptyRequest
    {
        /// <summary>
        /// Sends one request to the native side and hands the typed reply to
        /// <paramref name="completionHandler"/>.
        /// </summary>
        /// <param name="method">The method name the bridge dispatches on.</param>
        /// <param name="request">
        /// The parameters, either a model or a <see cref="JObject"/> built at the call site. Null
        /// sends the method alone.
        /// </param>
        /// <param name="completionHandler">
        /// Called with the decoded reply, or with the error the reply carried.
        /// </param>
        /// <param name="caller">
        /// The public method the request was made from, filled in by the compiler. It names the
        /// call in the diagnostic when the app's handler throws.
        /// </param>
        internal static void Send<T>(
            string method,
            object request,
            Action<T, AdaptyError> completionHandler,
            [CallerMemberName] string caller = null
        )
        {
            SendRaw<T>(
                method,
                request,
                (value, error) =>
                    InvokeCompletion(() => completionHandler?.Invoke(value, error), caller)
            );
        }

        /// <summary>
        /// Sends one request whose reply carries no value of its own, and reports only the error.
        /// </summary>
        /// <param name="method">The method name the bridge dispatches on.</param>
        /// <param name="request">
        /// The parameters, either a model or a <see cref="JObject"/> built at the call site. Null
        /// sends the method alone.
        /// </param>
        /// <param name="completionHandler">Called with the error the reply carried, or null.</param>
        /// <param name="caller">
        /// The public method the request was made from, filled in by the compiler. It names the
        /// call in the diagnostic when the app's handler throws.
        /// </param>
        internal static void SendVoid(
            string method,
            object request,
            Action<AdaptyError> completionHandler,
            [CallerMemberName] string caller = null
        ) =>
            Send<bool>(
                method,
                request,
                (value, error) => completionHandler?.Invoke(error),
                caller
            );

        /// <summary>
        /// Names the request that is calling back, and hands the call to the one callback policy.
        /// </summary>
        /// <remarks>
        /// The wrapping itself belongs to <see cref="AdaptyCallbacks.InvokeSafe"/> - what lives here is
        /// only the wording, in one place, so that the 40 requests cannot drift apart.
        /// </remarks>
        private static void InvokeCompletion(Action invocation, string caller) =>
            AdaptyCallbacks.InvokeSafe(invocation, $"Failed to invoke completionHandler in {caller}(..)");

        /// <summary>
        /// Reports a request that could not be encoded before it reached <see cref="SendVoid"/>,
        /// with the error the transport would have produced had the encoding happened inside it.
        /// </summary>
        /// <remarks>
        /// For the one overload that has to serialize an argument of its own before it can build
        /// the request. Without this the exception would leave the SDK synchronously, and that one
        /// public method would report failure differently from the other forty.
        /// </remarks>
        internal static void FailEncoding(
            string method,
            Exception exception,
            Action<AdaptyError> completionHandler,
            [CallerMemberName] string caller = null
        ) =>
            InvokeCompletion(
                () => completionHandler?.Invoke(EncodingFailed(method, exception)),
                caller
            );

        private static AdaptyError EncodingFailed(string method, Exception exception) =>
            new AdaptyError(
                AdaptyErrorCode.EncodingFailed,
                $"Failed encoding request: {method}",
                $"AdaptyUnityError.EncodingFailed({exception})"
            );

        private static void SendRaw<T>(
            string method,
            object request,
            Action<T, AdaptyError> completionHandler
        )
        {
            string payload;
            try
            {
                payload = AdaptyJson.SerializeRequest(method, request);
            }
            catch (Exception ex)
            {
                completionHandler(default(T), EncodingFailed(method, ex));
                return;
            }

            _Adapty.Invoke(
                method,
                payload,
                (json) =>
                {
                    var result = AdaptyResponse.Parse<T>(json);
                    completionHandler(result.Value, result.Error);
                }
            );
        }
    }
}
