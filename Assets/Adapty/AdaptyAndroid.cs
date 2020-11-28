using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdaptySDK {
	#if UNITY_ANDROID 
		class AdaptyAndroid {
			private static readonly string tag = "AdaptyAndroid";
			private static AndroidJavaClass AdaptyAndroidClass = new AndroidJavaClass("com.adapty.unity.AdaptyAndroidWrapper");

			private static AndroidJavaObject getActivity() {
				AndroidJavaClass unity_player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				return unity_player.GetStatic<AndroidJavaObject>("currentActivity");
			}

			private static AndroidJavaObject getContext() {
				AndroidJavaObject activity = getActivity();
				return activity.Call<AndroidJavaObject>("getApplicationContext");
			}

			public static Callback getCallback() {
				AndroidJavaObject javaCallback = AdaptyAndroidClass.CallStatic<AndroidJavaObject>("getCallback");
				if (javaCallback != null) {
					Debug.Log(tag + " executeCallback()");
					Callback callback = new Callback();
					callback.objectName = javaCallback.Get<string>("objectName");
					callback.method = javaCallback.Get<string>("method");
					callback.message = javaCallback.Get<string>("message");
					return callback;
				}
				return null;
			}

			public static void setLogLevel(int logLevel) {
				Debug.Log(tag + " setLogLevel()");
				AdaptyAndroidClass.CallStatic("setLogLevel", logLevel);
			}
			
			public static void activate(string key) {
				Debug.Log(tag + " activate()");
				AdaptyAndroidClass.CallStatic("activate", getContext(), key);
			}

			public static void identify(string customerUserId, string objectName) {
				Debug.Log(tag + " identify()");
				AdaptyAndroidClass.CallStatic("identify", customerUserId, objectName);
			}

			public static void logout(string objectName) {
				Debug.Log(tag + " logout()");
				AdaptyAndroidClass.CallStatic("logout", objectName);
			}

			public static void getPaywalls(string objectName) {
				Debug.Log(tag + " getPaywalls()");
				AdaptyAndroidClass.CallStatic("getPaywalls", objectName);
			}

			public static void makePurchase(string productJson, string objectName) {
				Debug.Log(tag + " makePurchase()");
				AdaptyAndroidClass.CallStatic("makePurchase", getActivity(), productJson, objectName);
			}

			public static void restorePurchases(string objectName) {
				Debug.Log(tag + " restorePurchases()");
				AdaptyAndroidClass.CallStatic("restorePurchases", objectName);
			}

			public static void syncPurchases(string objectName) {
				Debug.Log(tag + " syncPurchases()");
				AdaptyAndroidClass.CallStatic("syncPurchases", objectName);
			}

			public static void validatePurchase(int purchaseTypeInt, string productId, string purchaseToken, string purchaseOrderId, string productJson, string objectName) {
				Debug.Log(tag + " validatePurchase()");
				AdaptyAndroidClass.CallStatic("validatePurchase", purchaseTypeInt, productId, purchaseToken, purchaseOrderId, productJson, objectName);
			}

			public static void getPurchaserInfo(string objectName) {
				Debug.Log(tag + " getPurchaserInfo()");
				AdaptyAndroidClass.CallStatic("getPurchaserInfo", objectName);
			}

			public static void setOnPurchaserInfoUpdatedListener(string objectName) {
				Debug.Log(tag + " setOnPurchaserInfoUpdatedListener()");
				AdaptyAndroidClass.CallStatic("setOnPurchaserInfoUpdatedListener", objectName);
			}

			public static void updateAttribution(string attributionJson, int sourceInt, string networkUserId, string objectName) {
				Debug.Log(tag + " updateAttribution()");
				AdaptyAndroidClass.CallStatic("updateAttribution", attributionJson, sourceInt, networkUserId, objectName);
			}

			public static void updateProfile(string paramsJson, string objectName) {
				Debug.Log(tag + " updateProfile()");
				AdaptyAndroidClass.CallStatic("updateProfile", paramsJson, objectName);
			}

			public static void getPromo(string objectName) {
				Debug.Log(tag + " getPromo()");
				AdaptyAndroidClass.CallStatic("getPromo", objectName);
			}

			public static void setOnPromoReceivedListener(string objectName) {
				Debug.Log(tag + " setOnPromoReceivedListener()");
				AdaptyAndroidClass.CallStatic("setOnPromoReceivedListener", objectName);
			}
		}
	#endif
}