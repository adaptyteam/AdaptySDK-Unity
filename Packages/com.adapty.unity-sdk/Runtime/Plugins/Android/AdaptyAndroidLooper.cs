using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AdaptySDK.Android
{
#if UNITY_ANDROID
    /// <summary>
    /// Posts to the native <c>ALooper</c> of the thread that installed it - Unity's scripting thread.
    /// </summary>
    /// <remarks>
    /// A flow is an Activity of its own, and Unity's player loop is paused underneath it: a
    /// <c>SynchronizationContext</c> post, even <c>UnityPlayer.invokeOnMainThread</c>, waits for the
    /// resume, and the flow's <c>close</c> would reach the listener with nothing left to dismiss.
    /// The thread's native looper keeps turning through the pause on both entry points -
    /// <c>Looper.loop()</c> polls it under UnityPlayerActivity, the activity glue under GameActivity,
    /// where the thread has no Java <c>Looper</c> at all. An <c>eventfd</c> registered on it wakes
    /// the thread, and the callback drains the queue there. It is not a <c>Handler</c> on
    /// <c>Looper.myLooper()</c> because that finds none under GameActivity - or, after someone's
    /// <c>Looper.prepare()</c>, one that nothing loops. Only the NDK's stable C API is called, so
    /// the SDK ships no native library of its own.
    /// </remarks>
    internal static class AdaptyAndroidLooper
    {
        private const int LooperPollCallback = -2;
        private const int LooperEventInput = 1;
        private const int EfdNonblockCloexec = 0x800 | 0x80000;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LooperCallback(int fd, int events, IntPtr data);

        [DllImport("android")]
        private static extern IntPtr ALooper_forThread();

        [DllImport("android")]
        private static extern int ALooper_addFd(
            IntPtr looper,
            int fd,
            int ident,
            int events,
            LooperCallback callback,
            IntPtr data
        );

        [DllImport("c")]
        private static extern int eventfd(uint initval, int flags);

        [DllImport("c")]
        private static extern IntPtr write(int fd, ref ulong value, IntPtr count);

        [DllImport("c")]
        private static extern IntPtr read(int fd, out ulong value, IntPtr count);

        [DllImport("c")]
        private static extern int close(int fd);

        private static readonly ConcurrentQueue<Action> s_Queue = new ConcurrentQueue<Action>();

        // Held in a static so the marshalled pointer the looper keeps stays valid.
        private static readonly LooperCallback s_Callback = OnReadable;

        private static int s_Fd = -1;

        /// <summary>
        /// Registers on the calling thread's <c>ALooper</c>; false when that fails.
        /// </summary>
        internal static bool Install()
        {
            if (s_Fd >= 0)
            {
                return true;
            }
            var looper = ALooper_forThread();
            if (looper == IntPtr.Zero)
            {
                return false;
            }
            var fd = eventfd(0, EfdNonblockCloexec);
            if (fd < 0)
            {
                return false;
            }
            if (ALooper_addFd(looper, fd, LooperPollCallback, LooperEventInput, s_Callback, IntPtr.Zero) != 1)
            {
                close(fd);
                return false;
            }
            s_Fd = fd;
            return true;
        }

        internal static void Post(Action action)
        {
            s_Queue.Enqueue(action);
            ulong one = 1;
            write(s_Fd, ref one, (IntPtr)sizeof(ulong));
        }

        [AOT.MonoPInvokeCallback(typeof(LooperCallback))]
        private static int OnReadable(int fd, int events, IntPtr data)
        {
            read(fd, out _, (IntPtr)sizeof(ulong));
            while (s_Queue.TryDequeue(out var action))
            {
                // Nothing may unwind into the looper: it is native code with no handler above it.
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogError("Adapty: a callback posted to the Unity thread failed: " + e);
                }
            }
            return 1;
        }
    }
#endif
}
