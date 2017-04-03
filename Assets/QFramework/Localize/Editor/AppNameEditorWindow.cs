using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace QFramework {
	public class AppNameEditorWindow : EditorWindow {

		[MenuItem("QFramework/Localize/App Name")]
		static void Open() {

			AppNameEditorWindow appNameEditorWindow = (AppNameEditorWindow)EditorWindow.GetWindow(typeof(AppNameEditorWindow),true);
			appNameEditorWindow.titleContent = new  GUIContent("App Name Config");
			appNameEditorWindow.CurConfigData = AppNameConfigData.Load ();
			appNameEditorWindow.Show ();
		}

		public AppNameEditorWindow() {

		}

		public AppNameConfigData CurConfigData;
		LanguageData mLastLanguageItem = new LanguageData ();

		void UpdateItemsView(LanguageData data) {
			EditorGUILayout.BeginHorizontal ();
			data.Index = EditorGUILayout.Popup (data.Index,AppNameConfigData.LanguageDef);

			if (data.Index != 0) {

				data.AppName = EditorGUILayout.TextField (data.AppName);
				if (GUILayout.Button ("Delete")) {
					data.Index = 0;
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		void OnGUI() {
			int willRemovedIndex = -1;
			for (int i = 0; i < CurConfigData.SupportedLanguageItems.Count; i++) {
				UpdateItemsView (CurConfigData.SupportedLanguageItems [i]);
				if (CurConfigData.SupportedLanguageItems [i].Index == 0) {
					willRemovedIndex = i;
				}
			}

			if (willRemovedIndex != -1) {
				CurConfigData.SupportedLanguageItems.RemoveAt (willRemovedIndex);
			}

			UpdateItemsView (mLastLanguageItem);
			if (mLastLanguageItem.Index != 0) {
				CurConfigData.SupportedLanguageItems.Add (mLastLanguageItem);
				mLastLanguageItem = new LanguageData ();
			}

			if (GUILayout.Button ("Apply")) {
				CurConfigData.Save ();
				AppNameAndroidResGenerator.Generate (CurConfigData);
			}
		}
	}
}