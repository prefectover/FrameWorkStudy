using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using QFramework;
using System.Linq;

namespace QFramework
{
	public class QAssetBundleBuilder : EditorWindow
	{

		private int buildTargetIndex = 0;
		private string[] platformLabels = new string[]{ "Windows32", "iOS", "Android" };
		private Vector2 scrollPos;
		private const string KEY_PTAssetBundleBuilder_RESVERSION = "KEY_PTAssetBundleBuilder_RESVERSION";
		private const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";

		private const string KEY_ProjectTag = "KEY_ProjectTag";
		private const string KEY_ZipFramework = "KEY_ZipFramework";


		public static string resVersion = "100";
		private static string projectTag = "";
		//public static bool isUseFramework = true;
		public static bool isEnableGenerateClass = false;

		//[MenuItem ("PuTaoTool/AssetBundles/ForceClear")]
		public static void ForceClear(){
			if (Directory.Exists (QAssetBundleTool.AssetBundlesOutputPath)) {
				Directory.Delete (QAssetBundleTool.AssetBundlesOutputPath,true);
			}
			if(Directory.Exists(Application.streamingAssetsPath+"/AssetBundles")){
				Directory.Delete (Application.streamingAssetsPath+"/AssetBundles",true);
			}
			AssetDatabase.Refresh ();
		}

		[MenuItem ("PuTaoTool/AssetBundles/AssetBundleBuilder")]
		public static void ExecuteAssetBundle ()
		{
		
			QAssetBundleBuilder	window = (QAssetBundleBuilder)GetWindow (typeof(QAssetBundleBuilder), true);
			Debug.Log (Screen.width + " screen width*****");
			window.position = new Rect (100, 100, 500, 400);
			window.Show ();
		}

		void OnEnable ()
		{
			resVersion = EditorPrefs.GetString (KEY_PTAssetBundleBuilder_RESVERSION,"100");
			isEnableGenerateClass = EditorPrefs.GetBool (KEY_AUTOGENERATE_CLASS,true);

			projectTag = EditorPrefs.GetString (KEY_ProjectTag,"");
			//isUseFramework = EditorPrefs.GetBool (KEY_ZipFramework,true);


			switch (EditorUserBuildSettings.activeBuildTarget) {
			case BuildTarget.Android:
				buildTargetIndex = 2; 
				break;
			case BuildTarget.iOS:
				buildTargetIndex = 1;
				break;
			default:
				buildTargetIndex = 0;
				break;

			}
		}

		void DrawMenu ()
		{
			GUILayout.Toolbar (buildTargetIndex, platformLabels);
		}

		void DrawAssetBundleList ()
		{
//			GUILayout.BeginVertical ();
//
//			List<MarkItem> nodelist = PTConfigManager.Instance.markItems;
//			if (nodelist != null) {
//				for (int i = 0; i < nodelist.Count; i++) {
//					EditorGUILayout.LabelField (nodelist [i].path, new GUIStyle (EditorStyles.helpBox){ fontSize = 13 }, GUILayout.Width (400), GUILayout.Height (30));
//				}
//			}
//
//			GUILayout.EndVertical ();
		}

//		public void OnFocus ()
//		{
//			PTConfigManager.Instance.CheckItems ();
//		}

		public void OnDisable ()
		{
			EditorPrefs.SetBool (KEY_AUTOGENERATE_CLASS,isEnableGenerateClass);
			EditorPrefs.SetString (KEY_PTAssetBundleBuilder_RESVERSION,resVersion);
			EditorPrefs.SetString (KEY_ProjectTag,projectTag);
			//EditorPrefs.SetBool (KEY_ZipFramework,isUseFramework);


//			PTConfigManager.Instance.Dispose ();
		}



		void OnGUI ()
		{
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (500), GUILayout.Height (400));
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("PersistanePath:");
			GUILayout.TextField (Application.persistentDataPath);
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("Go To Persistance")) {
				EditorUtility.RevealInFinder (Application.persistentDataPath);
			}

			DrawMenu ();
			DrawAssetBundleList ();

			isEnableGenerateClass = GUILayout.Toggle (isEnableGenerateClass, "auto generate class");



			GUILayout.BeginHorizontal ();
			GUILayout.Label ("ResVersion:");
			resVersion = GUILayout.TextField (resVersion);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Project Tag:");
			projectTag = GUILayout.TextField (projectTag);
		
			//isUseFramework = GUILayout.Toggle (isUseFramework, "zip the framework");
			GUILayout.EndHorizontal ();



			if (GUILayout.Button ("Build")) {
				BuildWithTarget (EditorUserBuildSettings.activeBuildTarget);
			}
			if (GUILayout.Button ("ForceClear")) {
				ForceClear ();
			}

			GUILayout.EndVertical ();
			GUILayout.Space (50);

			EditorGUILayout.EndScrollView ();
	
		}

		void BuildWithTarget (BuildTarget buildTarget)
		{
//			List<MarkItem> nodelist = PTConfigManager.Instance.markItems;
//			for (int i = 0; i < nodelist.Count; i++) {
//				AssetImporter ai = AssetImporter.GetAtPath (nodelist [i].path);
//				ai.assetBundleName = nodelist [i].name;
//				Debug.Log (ai.assetBundleName);
//			}
//			AssetDatabase.Refresh ();

//			PTConfigManager.Instance.SaveConfigFile ();
			//PTAssetBundleTool.SetProjectTag();
			AssetDatabase.RemoveUnusedAssetBundleNames ();
			AssetDatabase.Refresh ();
			BuildScript.BuildAssetBundles (buildTarget,projectTag);
		}

	
	

	}

}
