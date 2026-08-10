using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// The envelope every native reply arrives in: either <c>error</c> or <c>success</c>.
    /// </summary>
    /// <remarks>
    /// Nothing here is allowed to throw. The reply is parsed on a callback from native code, which
    /// on IL2CPP is a reverse-P/Invoke boundary with no handler behind it, so a malformed payload
    /// has to come back as an <see cref="AdaptyError"/> rather than as an exception.
    /// </remarks>
    internal static class AdaptyResponse
    {
        internal static AdaptyResult<T> Parse<T>(string json)
        {
            try
            {
                if (!(AdaptyJson.ParseDocument(json) is JObject response))
                {
                    throw new JsonSerializationException("The reply is not an object.");
                }

                var error = response["error"];

                if (error != null && error.Type != JTokenType.Null)
                {
                    return new AdaptyResult<T>(
                        default(T),
                        error.ToObject<AdaptyError>(AdaptyJson.CreateSerializer())
                    );
                }

                // Required, not optional. A reply carrying neither member is malformed, and
                // silently reporting it as a successful default would turn a broken bridge into a
                // false negative - "not premium", "purchase did not happen".
                return new AdaptyResult<T>(
                    JsonRequire.Token(response, "success").ToObject<T>(AdaptyJson.CreateSerializer()),
                    null
                );
            }
            catch (Exception ex)
            {
                return new AdaptyResult<T>(
                    default(T),
                    new AdaptyError(
                        AdaptyErrorCode.DecodingFailed,
                        "Failed decoding result ",
                        $"AdaptyUnityError.DecodingFailed({ex})"
                    )
                );
            }
        }
    }
}
