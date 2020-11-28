# Guides

<img src="https://github.com/adaptyteam/AdaptySDK-Android/raw/master/adapty.png" width="512">

AdaptySDK Unity guides.

# Installation

To install the SDK you can either find it in the Asset Store or you can download from github `adapty-unity-plugin-*.unitypackage` and import it into your project.

The SDK uses "External Dependency Manager" plugin to handle iOS Cocoapods dependencies and Android gradle dependencies. After the installation you may need to invoke the dependency manager

`Assets -> External Dependency Manager -> Android Resolver -> Force Resolve`

and

`Assets -> External Dependency Manager -> iOS Resolver -> Install Cocoapods`

When building your Unity project for iOS, you would get `Unity-iPhone.xcworkspace` file, which you have to open instead of `Unity-iPhone.xcodeproj`, otherwise Cocoapods dependencies won't be used.

# Initial Setup

After importing the plugin, add `AdaptyObject` prefab located at `Assets/Adapty/` into your starting scene.

Select the object in your scene and enter your Adapty key.

You can also set the desired log level and enable the observe mode.

Read the usage documentation on what you can do with Adapty.

# API Usage

Adapty uses `AdaptySDK` namespace.

```c#
using AdaptySDK;
```