using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace AdaptySDK
{
    /// <summary>
    /// Base class for custom assets that can be used in Adapty UI.
    /// </summary>
    [Preserve]
    public abstract class AdaptyCustomAsset
    {
        /// <summary>
        /// Creates a custom asset from local image data.
        /// </summary>
        /// <param name="data">The image data as byte array.</param>
        /// <returns>A custom asset representing the image data.</returns>
        public static AdaptyCustomAsset LocalImageData(byte[] data)
        {
            return new AdaptyCustomAssetLocalImageData(data);
        }

        /// <summary>
        /// Creates a custom asset from a local image asset ID.
        /// </summary>
        /// <param name="assetId">The asset ID of the image.</param>
        /// <returns>A custom asset representing the image asset.</returns>
        public static AdaptyCustomAsset LocalImageAsset(string assetId)
        {
            return new AdaptyCustomAssetLocalImageAsset(assetId);
        }

        /// <summary>
        /// Creates a custom asset from a local image file path.
        /// </summary>
        /// <param name="path">The file path to the image.</param>
        /// <returns>A custom asset representing the image file.</returns>
        public static AdaptyCustomAsset LocalImageFile(string path)
        {
            return new AdaptyCustomAssetLocalImageFile(path);
        }

        /// <summary>
        /// Creates a custom asset from a local video asset ID.
        /// </summary>
        /// <param name="assetId">The asset ID of the video.</param>
        /// <returns>A custom asset representing the video asset.</returns>
        public static AdaptyCustomAsset LocalVideoAsset(string assetId)
        {
            return new AdaptyCustomAssetLocalVideoAsset(assetId);
        }

        /// <summary>
        /// Creates a custom asset from a local video file path.
        /// </summary>
        /// <param name="path">The file path to the video.</param>
        /// <returns>A custom asset representing the video file.</returns>
        public static AdaptyCustomAsset LocalVideoFile(string path)
        {
            return new AdaptyCustomAssetLocalVideoFile(path);
        }

        /// <summary>
        /// Creates a custom asset from a Unity Color.
        /// </summary>
        /// <param name="color">The Unity Color.</param>
        /// <returns>A custom asset representing the color.</returns>
        public static AdaptyCustomAsset Color(Color color)
        {
            return new AdaptyCustomAssetColor(color);
        }

        /// <summary>
        /// Creates a custom asset from a Unity Gradient.
        /// </summary>
        /// <param name="gradient">The Unity Gradient.</param>
        /// <returns>A custom asset representing the linear gradient.</returns>
        public static AdaptyCustomAsset LinearGradient(Gradient gradient)
        {
            return new AdaptyCustomAssetLinearGradient(gradient);
        }
    }

    /// <summary>
    /// Custom asset representing local image data.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetLocalImageData : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "image";

        [DataMember(Name = "value", IsRequired = true)]
        [Preserve]
        private byte[] _Data { get; }

        /// <summary>
        /// The image data as byte array.
        /// </summary>
        /// <remarks>
        /// A copy, as is the array the asset was built from: the request must not change because
        /// the caller kept writing into the array it handed over, or into this one. For a large
        /// image that is a real copy each way - hold the result if you need it twice.
        /// </remarks>
        public byte[] Data => (byte[])_Data.Clone();

        internal AdaptyCustomAssetLocalImageData(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _Data = (byte[])data.Clone();
        }
    }

    /// <summary>
    /// Custom asset representing a local image asset.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetLocalImageAsset : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "image";

        /// <summary>
        /// The asset ID of the image.
        /// </summary>
        [DataMember(Name = "asset_id", IsRequired = true)]
        [Preserve]
        public string AssetId { get; }

        internal AdaptyCustomAssetLocalImageAsset(string assetId)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
        }
    }

    /// <summary>
    /// Custom asset representing a local image file.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetLocalImageFile : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "image";

        /// <summary>
        /// The file path to the image.
        /// </summary>
        public string Path { get; }

        [DataMember(Name = "path", IsRequired = true)]
        [Preserve]
        private string PathForRequest => AdaptyCustomAssetPath.Resolve(Path);

        internal AdaptyCustomAssetLocalImageFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }

    /// <summary>
    /// Custom asset representing a local video asset.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetLocalVideoAsset : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "video";

        /// <summary>
        /// The asset ID of the video.
        /// </summary>
        [DataMember(Name = "asset_id", IsRequired = true)]
        [Preserve]
        public string AssetId { get; }

        internal AdaptyCustomAssetLocalVideoAsset(string assetId)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
        }
    }

    /// <summary>
    /// Custom asset representing a local video file.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetLocalVideoFile : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "video";

        /// <summary>
        /// The file path to the video.
        /// </summary>
        public string Path { get; }

        [DataMember(Name = "path", IsRequired = true)]
        [Preserve]
        private string PathForRequest => AdaptyCustomAssetPath.Resolve(Path);

        internal AdaptyCustomAssetLocalVideoFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }

    /// <summary>
    /// Custom asset representing a color.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetColor : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "color";

        /// <summary>
        /// The Unity Color.
        /// </summary>
        public Color ColorValue { get; }

        [DataMember(Name = "value", IsRequired = true)]
        [Preserve]
        private string ValueForRequest => AdaptyCustomAssetPath.ColorToHex(ColorValue);

        internal AdaptyCustomAssetColor(Color color)
        {
            ColorValue = color;
        }
    }

    /// <summary>
    /// Custom asset representing a linear gradient.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomAssetLinearGradient : AdaptyCustomAsset
    {
        [DataMember(Name = "type", IsRequired = true)]
        [Preserve]
        private string Type => "linear-gradient";

        /// <summary>
        /// The Unity Gradient.
        /// </summary>
        public Gradient Gradient { get; }

        [DataMember(Name = "values", IsRequired = true)]
        [Preserve]
        private System.Collections.Generic.List<Stop> ValuesForRequest
        {
            get
            {
                var stops = new System.Collections.Generic.List<Stop>();
                foreach (var time in KeyTimes())
                {
                    stops.Add(
                        new Stop(AdaptyCustomAssetPath.ColorToHex(Gradient.Evaluate(time)), time)
                    );
                }
                return stops;
            }
        }

        /// <summary>
        /// A Unity gradient always runs left to right over its full width.
        /// </summary>
        [DataMember(Name = "points", IsRequired = true)]
        [Preserve]
        private Points PointsForRequest => new Points();

        internal AdaptyCustomAssetLinearGradient(Gradient gradient)
        {
            Gradient = gradient ?? throw new ArgumentNullException(nameof(gradient));
        }

        /// <summary>
        /// Color keys and alpha keys are independent in a Unity Gradient: they may differ in count and sit
        /// at different times. Emit a stop at every key time of either channel and let Gradient.Evaluate
        /// resolve the RGBA there, so the serialized gradient matches what Unity renders.
        /// </summary>
        private System.Collections.Generic.List<float> KeyTimes()
        {
            var times = new System.Collections.Generic.List<float>();

            foreach (var key in Gradient.colorKeys)
            {
                if (!times.Contains(key.time))
                {
                    times.Add(key.time);
                }
            }

            foreach (var key in Gradient.alphaKeys)
            {
                if (!times.Contains(key.time))
                {
                    times.Add(key.time);
                }
            }

            times.Sort();
            return times;
        }

        [DataContract]
        private sealed class Stop
        {
            internal Stop(string color, double position)
            {
                Color = color;
                Position = position;
            }

            [DataMember(Name = "color", IsRequired = true)]
            [Preserve]
            private string Color { get; }

            [DataMember(Name = "p", IsRequired = true)]
            [Preserve]
            private double Position { get; }
        }

        [DataContract]
        private sealed class Points
        {
            [DataMember(Name = "x0", IsRequired = true)]
            [Preserve]
            private double X0 => 0.0;

            [DataMember(Name = "y0", IsRequired = true)]
            [Preserve]
            private double Y0 => 0.0;

            [DataMember(Name = "x1", IsRequired = true)]
            [Preserve]
            private double X1 => 1.0;

            [DataMember(Name = "y1", IsRequired = true)]
            [Preserve]
            private double Y1 => 0.0;
        }
    }

    /// <summary>
    /// Shared helpers for the write-only custom asset payloads.
    /// </summary>
    [Preserve]
    internal static class AdaptyCustomAssetPath
    {
        /// <summary>
        /// A path given by the app is relative to StreamingAssets; the native side needs the real
        /// location, which differs per platform.
        /// </summary>
        internal static string Resolve(string path)
        {
#if UNITY_IOS && !UNITY_EDITOR
            return UnityEngine.Application.dataPath + "/Raw/" + path;
#elif UNITY_ANDROID && !UNITY_EDITOR
            return "jar:file://" + UnityEngine.Application.dataPath + "!/assets/" + path;
#else
            return path;
#endif
        }

        internal static string ColorToHex(Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255);
            var g = Mathf.RoundToInt(color.g * 255);
            var b = Mathf.RoundToInt(color.b * 255);
            var a = Mathf.RoundToInt(color.a * 255);

            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        }
    }
}
