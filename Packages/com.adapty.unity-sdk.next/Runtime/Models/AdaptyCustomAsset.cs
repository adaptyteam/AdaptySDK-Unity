//
//  AdaptyCustomAsset.cs
//  AdaptySDK
//
//  Created by Assistant on 14.01.2025.
//

using System;
using UnityEngine;

namespace AdaptySDK
{
    /// <summary>
    /// Base class for custom assets that can be used in Adapty UI.
    /// </summary>
    public abstract partial class AdaptyCustomAsset
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
    public sealed partial class AdaptyCustomAssetLocalImageData : AdaptyCustomAsset
    {
        /// <summary>
        /// The image data as byte array.
        /// </summary>
        public byte[] Data { get; }

        internal AdaptyCustomAssetLocalImageData(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }

    /// <summary>
    /// Custom asset representing a local image asset.
    /// </summary>
    public sealed partial class AdaptyCustomAssetLocalImageAsset : AdaptyCustomAsset
    {
        /// <summary>
        /// The asset ID of the image.
        /// </summary>
        public string AssetId { get; }

        internal AdaptyCustomAssetLocalImageAsset(string assetId)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
        }
    }

    /// <summary>
    /// Custom asset representing a local image file.
    /// </summary>
    public sealed partial class AdaptyCustomAssetLocalImageFile : AdaptyCustomAsset
    {
        /// <summary>
        /// The file path to the image.
        /// </summary>
        public string Path { get; }

        internal AdaptyCustomAssetLocalImageFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }

    /// <summary>
    /// Custom asset representing a local video asset.
    /// </summary>
    public sealed partial class AdaptyCustomAssetLocalVideoAsset : AdaptyCustomAsset
    {
        /// <summary>
        /// The asset ID of the video.
        /// </summary>
        public string AssetId { get; }

        internal AdaptyCustomAssetLocalVideoAsset(string assetId)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
        }
    }

    /// <summary>
    /// Custom asset representing a local video file.
    /// </summary>
    public sealed partial class AdaptyCustomAssetLocalVideoFile : AdaptyCustomAsset
    {
        /// <summary>
        /// The file path to the video.
        /// </summary>
        public string Path { get; }

        internal AdaptyCustomAssetLocalVideoFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }

    /// <summary>
    /// Custom asset representing a color.
    /// </summary>
    public sealed partial class AdaptyCustomAssetColor : AdaptyCustomAsset
    {
        /// <summary>
        /// The Unity Color.
        /// </summary>
        public Color ColorValue { get; }

        internal AdaptyCustomAssetColor(Color color)
        {
            ColorValue = color;
        }
    }

    /// <summary>
    /// Custom asset representing a linear gradient.
    /// </summary>
    public sealed partial class AdaptyCustomAssetLinearGradient : AdaptyCustomAsset
    {
        /// <summary>
        /// The Unity Gradient.
        /// </summary>
        public Gradient Gradient { get; }

        internal AdaptyCustomAssetLinearGradient(Gradient gradient)
        {
            Gradient = gradient ?? throw new ArgumentNullException(nameof(gradient));
        }
    }
}
