using System;

namespace AdaptySDK
{
    /// <summary>
    /// The one policy for calling back into the app, and the only implementation of it.
    /// </summary>
    /// <remarks>
    /// Safe does not mean swallowed. The app's own exception is rethrown with the context of the
    /// call that raised it and the original as <see cref="Exception.InnerException"/>, which is
    /// what a caller sees on a request and what <c>Adapty.OnMessage</c> logs on an event - that
    /// boundary is a reverse P/Invoke with no handler behind it, and it keeps its own guard.
    /// Requests reach this through <c>Request</c>, which supplies the wording; events name
    /// themselves at the call site, since the listener method is not the enclosing one.
    /// </remarks>
    internal static class Callbacks
    {
        internal static void InvokeSafe(Action invocation, string failureContext)
        {
            try
            {
                invocation();
            }
            catch (Exception e)
            {
                throw new Exception(failureContext, e);
            }
        }
    }
}
