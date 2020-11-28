#!/bin/bash

DEPLOY_PATH=output
UNITY_PATH=/Applications/Unity/Hub/Editor/2020.1.15f1/Unity.app/Contents/MacOS/Unity
PACKAGE_NAME=adapty-unity-plugin-1.0.0.unitypackage
mkdir -p $DEPLOY_PATH

echo "Start Build for $PACKAGE_NAME"

# Build the .unitypackage
$UNITY_PATH \
-gvh_disable \
-batchmode \
-nographics \
-logFile build.log \
-projectPath $PWD/../ \
-exportPackage Assets $PWD/$DEPLOY_PATH/$PACKAGE_NAME \
-quit \
&& echo "The package exported to $PWD/$DEPLOY_PATH/$PACKAGE_NAME" \
|| echo "Failed to export the package. See build.log for details."

# BUG: unity can't import the package
#-importPackage external-dependency-manager-1.2.162.unitypackage \

if [ "$1" == "-p" ]; then
	echo "removing ../Library"
	rm -rf ../Library
	echo "removing ../Logs"
	rm -rf ../Logs
	echo "removing ../Packages"
	rm -rf ../Packages
	echo "removing ../ProjectSettings"
	rm -rf ../ProjectSettings
	echo "removing ../obj"
	rm -rf ../obj
	echo "removing ../ProjectSettings"
	rm -rf ../ProjectSettings
	echo "removing ../UserSettings"
	rm -rf ../UserSettings
	echo "removing ../Temp"
	rm -rf ../Temp
	echo "removing ../Assembly-CSharp-Editor.csproj"
	rm -rf ../Assembly-CSharp-Editor.csproj
	echo "removing ../Assembly-CSharp.csproj"
	rm -rf ../Assembly-CSharp.csproj
	echo "removing ../AdaptySDK-Unity.sln"
	rm -rf ../AdaptySDK-Unity.sln
	echo "removing ./build.log"
	rm ./build.log
	echo "Moving $DEPLOY_PATH/$PACKAGE_NAME to root"
	mv ./$DEPLOY_PATH/$PACKAGE_NAME ..
	echo "removing ./$DEPLOY_PATH"
	rm -rf ./$DEPLOY_PATH
else
	echo "dev mode. No files removed. Run with -p flag for production build."
fi
