using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

/// <summary>
/// 配置路径用的
/// </summary>
namespace QFramework {
	public class PathEditorWindow : EditorWindow {

		[MenuItem("QFramework/IO/PathConfig")]
		static void Open() {
			PathEditorWindow pathEditorWindow = (PathEditorWindow)EditorWindow.GetWindow (typeof(PathEditorWindow), true);
			pathEditorWindow.titleContent = new GUIContent ("Path Config");
			pathEditorWindow.CurConfigData = PathConfigData.Load ();
			pathEditorWindow.Show ();
		}
			
		public PathEditorWindow() {}

		public PathConfigData CurConfigData;

		void OnGUI() {

			for (int i = 0; i < CurConfigData.pathInfoItems.Count; i++) {

			}

			// 创建PathInfo
			if (GUILayout.Button ("Create")) {
				// TODO: 没写完
			}
		}

//		LanguageData mLastLanguageItem = new LanguageData ();
//
//		void UpdateItemsView(LanguageData data) {
//			EditorGUILayout.BeginHorizontal ();
//			data.Index = EditorGUILayout.Popup (data.Index,AppNameConfigData.LanguageDef);
//
//			if (data.Index != 0) {
//
//				data.AppName = EditorGUILayout.TextField (data.AppName);
//				if (GUILayout.Button ("Delete")) {
//					data.Index = 0;
//				}
//			}
//			EditorGUILayout.EndHorizontal();
//		}
//
//		void OnGUI() {
//			int willRemovedIndex = -1;
//			for (int i = 0; i < CurConfigData.SupportedLanguageItems.Count; i++) {
//				UpdateItemsView (CurConfigData.SupportedLanguageItems [i]);
//				if (CurConfigData.SupportedLanguageItems [i].Index == 0) {
//					willRemovedIndex = i;
//				}
//			}
//
//			if (willRemovedIndex != -1) {
//				CurConfigData.SupportedLanguageItems.RemoveAt (willRemovedIndex);
//			}
//
//			UpdateItemsView (mLastLanguageItem);
//			if (mLastLanguageItem.Index != 0) {
//				CurConfigData.SupportedLanguageItems.Add (mLastLanguageItem);
//				mLastLanguageItem = new LanguageData ();
//			}
//
//			if (GUILayout.Button ("Apply")) {
//				CurConfigData.Save ();
//				AppNameAndroidResGenerator.Generate (CurConfigData);
//			}
//		}
	}
		
	/// <summary>
	/// 路径数据配置
	/// </summary>
	public class PathConfigData {

		string mConfigPath {
			get {
				return Application.dataPath + "/QFrameworkData/Path";
			}
		}

		public List<PathInfo> pathInfoItems;

		public static PathConfigData Load() {

			var retPathConfigData = new PathConfigData();



			return retPathConfigData;
		}
	}
		
	/// <summary>
	/// 路径信息(每个XML文件)
	/// </summary>
	public class PathInfo {
		public string name;
		public Dictionary<string,PathItemInfo> pathsDict;
	}

	/// <summary>
	/// 路径条(XMLNode)
	/// </summary>
	public class PathItemInfo {
		public string name;
		public string path;
		public string description;
		public PathItemInfo(string name,string path,string description){
			this.name = name;
			this.path = path;
			this.description = description;
		}
	}
}