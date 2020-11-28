using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdaptyObjectScript))]
[CanEditMultipleObjects]
public class AdaptyObjectEditor : Editor {
	SerializedProperty key, observeMode, logLevel;

	void OnEnable() {
		key = serializedObject.FindProperty("key");
		observeMode = serializedObject.FindProperty("observeMode");
		logLevel = serializedObject.FindProperty("logLevel");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		int width = 236;
		GUILayout.Box((Texture)AssetDatabase.LoadAssetAtPath("Assets/Adapty/Editor/logo.png", typeof(Texture)), new GUILayoutOption[] {GUILayout.Width(width), GUILayout.Height(63)});

		EditorGUILayout.Separator();

		EditorGUILayout.HelpBox("Set your key to init the Adapty SDK.", MessageType.Info);
		EditorGUILayout.PropertyField(key);

		EditorGUILayout.Separator();

		EditorGUILayout.HelpBox("If you have a functioning subscription system and want to give Adapty SDK a quick try, you can use Observer mode. On Android you have to call Adapty.syncPurchases() after each purchase or restoring purchases.", MessageType.Info);
		EditorGUILayout.PropertyField(observeMode);

		EditorGUILayout.Separator();

		EditorGUILayout.HelpBox("Set log level for errors and other important information to help you understand what is going on.", MessageType.Info);
		EditorGUILayout.PropertyField(logLevel);

		EditorGUILayout.Separator();

		EditorGUILayout.HelpBox("For more information on setting up Adapty check out our relevant docs.", MessageType.None);

		
		if (GUILayout.Button("Adapty Unity Docs", new GUILayoutOption[] {GUILayout.Width(width)})) {
			Application.OpenURL("https://docs.adapty.io/sdk/integrating-adapty-sdk/unity-sdk-intro");
		}

		if (GUILayout.Button("Adapty iOS Docs", new GUILayoutOption[] {GUILayout.Width(width)})) {
			Application.OpenURL("https://docs.adapty.io/sdk/integrating-adapty-sdk/ios-sdk-intro");
		}

		if (GUILayout.Button("Adapty Android Docs", new GUILayoutOption[] {GUILayout.Width(width)})) {
			Application.OpenURL("https://docs.adapty.io/sdk/integrating-adapty-sdk/android-sdk-intro");
		}

		serializedObject.ApplyModifiedProperties();
	}
}