// Minimal UnityEngine stand-ins so the SDK sources link outside the Unity Editor.
// The models need [Preserve], and AdaptyCustomAsset.cs also Color and Gradient; the rest is for
// the platform bridges in Runtime/Plugins.

using System;

namespace UnityEngine
{
    public struct Color
    {
        public float r, g, b, a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct GradientColorKey
    {
        public Color color;
        public float time;

        public GradientColorKey(Color color, float time)
        {
            this.color = color;
            this.time = time;
        }
    }

    public struct GradientAlphaKey
    {
        public float alpha;
        public float time;

        public GradientAlphaKey(float alpha, float time)
        {
            this.alpha = alpha;
            this.time = time;
        }
    }

    public class Gradient
    {
        public GradientColorKey[] colorKeys = new GradientColorKey[0];
        public GradientAlphaKey[] alphaKeys = new GradientAlphaKey[0];

        // Linear interpolation over the colour keys, with alpha taken from the alpha keys.
        // Enough to reproduce what the SDK serializes for a Unity gradient.
        public Color Evaluate(float time)
        {
            var color = Sample(time);
            return new Color(color.r, color.g, color.b, SampleAlpha(time));
        }

        private Color Sample(float time)
        {
            if (colorKeys.Length == 0)
            {
                return new Color(0, 0, 0, 1);
            }

            var previous = colorKeys[0];
            foreach (var key in colorKeys)
            {
                if (key.time >= time)
                {
                    if (key.time == previous.time)
                    {
                        return key.color;
                    }

                    var t = (time - previous.time) / (key.time - previous.time);
                    return new Color(
                        previous.color.r + (key.color.r - previous.color.r) * t,
                        previous.color.g + (key.color.g - previous.color.g) * t,
                        previous.color.b + (key.color.b - previous.color.b) * t,
                        1
                    );
                }

                previous = key;
            }

            return colorKeys[colorKeys.Length - 1].color;
        }

        private float SampleAlpha(float time)
        {
            if (alphaKeys.Length == 0)
            {
                return 1f;
            }

            var previous = alphaKeys[0];
            foreach (var key in alphaKeys)
            {
                if (key.time >= time)
                {
                    if (key.time == previous.time)
                    {
                        return key.alpha;
                    }

                    var t = (time - previous.time) / (key.time - previous.time);
                    return previous.alpha + (key.alpha - previous.alpha) * t;
                }

                previous = key;
            }

            return alphaKeys[alphaKeys.Length - 1].alpha;
        }
    }

    public static class Mathf
    {
        public static int RoundToInt(float f) =>
            (int)Math.Round((double)f, MidpointRounding.AwayFromZero);
    }

    public static class Application
    {
        public static string dataPath => "/stub/dataPath";
    }

    public static class Debug
    {
        public static void Log(object message) => Console.WriteLine(message);

        public static void LogWarning(object message) => Console.WriteLine("WARN: " + message);

        public static void LogError(object message) => Console.WriteLine("ERROR: " + message);
    }
}

namespace UnityEngine
{
    // The Android bridge is linked by the next-package suite so the transport compiles under
    // UNITY_ANDROID. Nothing calls into the JVM here - these only have to satisfy the compiler.
    public class AndroidJavaObject : IDisposable
    {
        public AndroidJavaObject(string className, params object[] args) { }

        public T Call<T>(string methodName, params object[] args) =>
            throw new NotSupportedException();

        public void Call(string methodName, params object[] args) =>
            throw new NotSupportedException();

        public T CallStatic<T>(string methodName, params object[] args) =>
            throw new NotSupportedException();

        public void CallStatic(string methodName, params object[] args) =>
            throw new NotSupportedException();

        public T GetStatic<T>(string fieldName) => throw new NotSupportedException();

        public void Dispose() { }
    }

    public class AndroidJavaClass : AndroidJavaObject
    {
        public AndroidJavaClass(string className)
            : base(className) { }
    }

    public class AndroidJavaProxy
    {
        protected AndroidJavaProxy(string javaInterface) { }
    }
}

namespace AOT
{
    // Marks the reverse-P/Invoke entry points on iOS. Attribute only - it changes nothing here,
    // but the transport's callback boundary is exactly where a thrown exception would be fatal
    // on IL2CPP, so the file has to compile in the test suite.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute(Type type) { }
    }
}

namespace UnityEngine.Scripting
{
    // Unity's marker for "the linker cannot see this is used". Stubbed so the models keep
    // compiling in the plain .NET suites.
    [AttributeUsage(
        AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Enum
            | AttributeTargets.Constructor
            | AttributeTargets.Method
            | AttributeTargets.Property
            | AttributeTargets.Field
            | AttributeTargets.Event
            | AttributeTargets.Interface
            | AttributeTargets.Delegate
    )]
    public sealed class PreserveAttribute : Attribute { }
}
