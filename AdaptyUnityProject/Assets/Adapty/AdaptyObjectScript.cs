using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptySDK;

public class AdaptyObjectScript : MonoBehaviour {
	public string key;
	public bool observeMode = false;
	public LogLevel logLevel = LogLevel.Error;

	void Start() {
		Adapty.setLogLevel(logLevel);
		Adapty.activate(key, observeMode);
	}

	void Update() {
		#if !UNITY_EDITOR
			Adapty.executeCallback();
		#endif
	}
}
