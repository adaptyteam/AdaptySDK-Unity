using System.Runtime.InteropServices;
using UnityEngine;

namespace AdaptySDK {
#if UNITY_IOS
	class AdaptyiOS {
		private static readonly string tag = "AdaptyiOS";

		public static Callback getCallback() {
			if (AdaptyUnity_hasCallback()) {
				Debug.Log(tag + " executeCallback()");
				Callback callback = new Callback();
				callback.objectName = AdaptyUnity_getCallbackObjectName();
				callback.method = AdaptyUnity_getCallbackMethod();
				callback.message = AdaptyUnity_getCallbackMessageAndPop();
				return callback;
			}
			return null;
		}

		public static void setLogLevel(int logLevel) {
			Debug.Log(tag + " setLogLevel()");
			AdaptyUnity_setLogLevel(logLevel);
		}

		public static void activate(string key, bool observeMode) {
			Debug.Log(tag + " activate()");
			AdaptyUnity_activate(key, observeMode); 
		}

		public static void identify(string customerUserId, string objectName) {
			Debug.Log(tag + " identify()");
			AdaptyUnity_identify(customerUserId, objectName);
		}

		public static void logout(string objectName) {
			Debug.Log(tag + " logout()");
			AdaptyUnity_logout(objectName);
		}

		public static void getPaywalls(string objectName) {
			Debug.Log(tag + " getPaywalls()");
			AdaptyUnity_getPaywalls(objectName);
		}

		public static void makePurchase(string productJson, string offerId, string objectName) {
			Debug.Log(tag + " makePurchase()");
			AdaptyUnity_makePurchase(productJson, offerId, objectName);
		}

		public static void restorePurchases(string objectName) {
			Debug.Log(tag + " restorePurchases()");
			AdaptyUnity_restorePurchases(objectName);
		}

		public static void validateReceipt(string receipt, string objectName) {
			Debug.Log(tag + " validateReceipt()");
			AdaptyUnity_validateReceipt(receipt, objectName);
		}

		public static void getPurchaserInfo(string objectName) {
			Debug.Log(tag + " getPurchaserInfo()");
			AdaptyUnity_getPurchaserInfo(objectName);
		}

		public static void setOnPurchaserInfoUpdatedListener(string objectName) {
			Debug.Log(tag + " setOnPurchaserInfoUpdatedListener()");
			AdaptyUnity_setOnPurchaserInfoUpdatedListener(objectName);
		}

		public static void updateAttribution(string attributionJson, int sourceInt, string networkUserId, string objectName) {
			Debug.Log(tag + " updateAttribution()");
			AdaptyUnity_updateAttribution(attributionJson, sourceInt, networkUserId, objectName);
		}

		public static void updateProfile(string paramsJson, string objectName) {
			Debug.Log(tag + " updateProfile()");
			AdaptyUnity_updateProfile(paramsJson, objectName);
		}

		public static void getPromo(string objectName) {
			Debug.Log(tag + " getPromo()");
			AdaptyUnity_getPromo(objectName);
		}

		public static void setOnPromoReceivedListener(string objectName) {
			Debug.Log(tag + " setOnPromoReceivedListener()");
			AdaptyUnity_setOnPromoReceivedListener(objectName);
		}

		[DllImport("__Internal")]
		private static extern bool AdaptyUnity_hasCallback();

		[DllImport("__Internal")]
		private static extern string AdaptyUnity_getCallbackObjectName();

		[DllImport("__Internal")]
		private static extern string AdaptyUnity_getCallbackMethod();

		[DllImport("__Internal")]
		private static extern string AdaptyUnity_getCallbackMessageAndPop();

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_setLogLevel(int logLevel);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_activate(string key, bool observeMode);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_identify(string key, string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_logout(string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_getPaywalls(string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_makePurchase(string productJson, string offerId, string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_restorePurchases(string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_validateReceipt(string receipt, string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_getPurchaserInfo(string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_setOnPurchaserInfoUpdatedListener(string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_updateAttribution(string attributionJson, int sourceInt, string networkUserId, string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_updateProfile(string paramsJson, string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_getPromo(string objectName);

		[DllImport("__Internal")]
		private static extern void AdaptyUnity_setOnPromoReceivedListener(string objectName);
	}
#endif
}