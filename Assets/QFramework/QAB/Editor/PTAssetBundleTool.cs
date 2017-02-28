using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditorInternal;

namespace PTGame.AssetBundles
{
	[InitializeOnLoad]
	public class PTAssetBundleTool
	{

		public const string AssetBundlesOutputPath = "AssetBundles";

		public static int m_SimulateAssetBundleInEditor = -1;
		private const string kSimulateAssetBundles = "SimulateAssetBundles";

	
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool SimulateAssetBundleInEditor 
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}

		private const string Mark_AssetBundle = "Assets/PTMark/AssetBundle";
		private const string Mark_HotUpdateFile = "Assets/PTMark/File";
		private const string Mark_HotUpdateZip = "Assets/PTMark/Zip";

		public const string LABEL_AB = "ptab_ab";
		public const string LABEL_ZIP = "ptab_zip";
		public const string LABEL_FILE = "ptab_file";
		static PTAssetBundleTool ()
		{
			Selection.selectionChanged = OnSelectionChanged;
		 
			EditorApplication.update += Update;

		}

		private static int markCounter = 0;
		private static string lastMarkPth = string.Empty;
		private static void ResetPTMark(string path){

			Object tempObj = PrefabUtility.CreateEmptyPrefab ( "Assets/ptabmanager_temp.prefab");
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			Selection.activeObject = tempObj;
			lastMarkPth = path;
			markCounter = 0;

		}
		static void Update(){
			
			if (markCounter < 2) {
				markCounter++;
			}else if(markCounter==2){
				markCounter++;
				if (!string.IsNullOrEmpty (lastMarkPth)) {
					Object obj = AssetDatabase.LoadAssetAtPath (lastMarkPth,typeof(Object));
					Selection.activeObject = obj;
					AssetDatabase.DeleteAsset ("Assets/ptabmanager_temp.prefab");
					AssetDatabase.Refresh ();
				

				}
			}
		}

		public static void OnSelectionChanged ()
		{
			string path = GetSelectedPathOrFallback ();
			if (!string.IsNullOrEmpty (path)) {
				Object obj = AssetDatabase.LoadAssetAtPath (path,typeof(Object));
				bool contain = HasPTABLabel(obj,LABEL_AB);
				Menu.SetChecked (Mark_AssetBundle, contain);
				contain = HasPTABLabel (obj,LABEL_ZIP);
				Menu.SetChecked (Mark_HotUpdateZip,contain);
				contain = HasPTABLabel (obj,LABEL_FILE);
				Menu.SetChecked (Mark_HotUpdateFile,contain);
			
			}
		}

		public static bool HasPTABLabel(Object obj,string label){
			string[] labels =  AssetDatabase.GetLabels (obj);
			List<string> lbs = new List<string> (labels);
			foreach (string lb in lbs){
				if (lb == label) {
					return true;
				}
			}
			return false;
		}
		private static void RemoveUselessLabels(Object obj){
			string[] labels =  AssetDatabase.GetLabels (obj);
			List<string> lbs = new List<string> (labels);
			for(int i=lbs.Count-1;i>=0;i--){
				string lb = lbs [i];
				if(lb!=LABEL_AB&&lb!=LABEL_ZIP&&lb!=LABEL_FILE){
					lbs.Remove (lb);
				}
			}
			AssetDatabase.SetLabels (obj,lbs.ToArray());
		}

		public static bool SetLabels(Object obj,string label){
			RemoveUselessLabels (obj);
			string[] labels =  AssetDatabase.GetLabels (obj);
			List<string> lbs = new List<string> (labels);
			foreach (string lb in lbs){
				if (lb == label) {
					lbs.Remove (lb);
					AssetDatabase.SetLabels (obj,lbs.ToArray());
					return false;
				}
			}
			lbs.Add (label);
			AssetDatabase.SetLabels (obj,lbs.ToArray());
		
			EditorUtility.SetDirty (obj);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

			return true;

		}

		[MenuItem(Mark_HotUpdateZip)]
		public static void MarkHotUpdateZip(){
			string path = GetSelectedPathOrFallback ();
			if (string.IsNullOrEmpty (path) || !Directory.Exists (path)) {
				return;
			}

			Object obj = AssetDatabase.LoadAssetAtPath (path, typeof(Object));
			bool contain = SetLabels (obj, LABEL_ZIP);
			if (!contain) {
				Menu.SetChecked (Mark_HotUpdateZip, false);
			} else {
				Menu.SetChecked (Mark_HotUpdateZip, true);
			}
			ResetPTMark(path);
		}

		[MenuItem (Mark_HotUpdateFile)]
		public static void MarkHotUpdateFile ()
		{
			string path = GetSelectedPathOrFallback ();
			if (string.IsNullOrEmpty (path) || !File.Exists (path)) {
				return;
			}

			Object obj = AssetDatabase.LoadAssetAtPath (path, typeof(Object));
			bool contain = SetLabels (obj, LABEL_FILE);
			if (!contain) {
				Menu.SetChecked (Mark_HotUpdateFile, false);
			} else {
				Menu.SetChecked (Mark_HotUpdateFile, true);
			}
			ResetPTMark(path);
	
		}

		[MenuItem (Mark_AssetBundle)]
		public static void MarkPTABDir ()
		{
			string path = GetSelectedPathOrFallback ();
			if (!string.IsNullOrEmpty (path)) {
				AssetImporter ai = AssetImporter.GetAtPath (path);
			
				Object obj = AssetDatabase.LoadAssetAtPath (path,typeof(Object));
				bool contain = SetLabels(obj,LABEL_AB);
				if (!contain) {
					Menu.SetChecked (Mark_AssetBundle, false);
					ai.assetBundleName = null;
				} else {
					DirectoryInfo dir = new DirectoryInfo (path);
					Menu.SetChecked (Mark_AssetBundle, true);
					ai.assetBundleName = dir.Name.Replace(".","_");
				}
				AssetDatabase.RemoveUnusedAssetBundleNames ();

				ResetPTMark(path);
			}
		}

		public static string GetSelectedPathOrFallback ()
		{
			string path = string.Empty;

			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
				path = AssetDatabase.GetAssetPath (obj);
				if (!string.IsNullOrEmpty (path) && File.Exists (path)) {
				}
			}
			//Debug.Log ("path ***** :"+path);
			return path;
		}





	}
}
