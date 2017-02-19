using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class LocalizeAssetPostprocessor : AssetPostprocessor {

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{
		// if (importedAssets.Length == 0) {
			// return;
		// }
		
		var buildSettingsScenes = EditorBuildSettings.scenes;
		var launchScreenScenePath = "Assets/PTUGame/PTLocalize/LaunchScreen/PTLaunchScreen.unity";

		bool hasLaunchScreenScene = false;
		foreach (EditorBuildSettingsScene buildSettingsScene in buildSettingsScenes) {
			if (buildSettingsScene.path == launchScreenScenePath) {
				hasLaunchScreenScene = true;
				buildSettingsScene.enabled = true;
			}
		}

		if (!hasLaunchScreenScene) {
			var newBuildSettingsScenes = new EditorBuildSettingsScene[buildSettingsScenes.Length + 1];
			newBuildSettingsScenes [0] = new EditorBuildSettingsScene (launchScreenScenePath, true);
			for (int i = 1; i < newBuildSettingsScenes.Length; i++) {
				newBuildSettingsScenes [i] = buildSettingsScenes [i - 1];
			}
			EditorBuildSettings.scenes = newBuildSettingsScenes;
		}
	}
}
