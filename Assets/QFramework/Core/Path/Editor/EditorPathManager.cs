using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QFramework {
	public class EditorPathManager {

		public const string DefaultPathConfigGenerateForder = "Assets/QFrameworkData/Path/Config";

		public const string DefaultPathScriptGenerateForder = "Assets/QFrameworkData/Path/Script";

		static Dictionary<string,PathConfig> m_CachedPathConfigDict;

		static PathConfig Load(string configName) {
			if (null == m_CachedPathConfigDict || m_CachedPathConfigDict.Count == 0) {
				m_CachedPathConfigDict = new Dictionary<string, PathConfig> ();
			}

			PathConfig retConfig = null;

			m_CachedPathConfigDict.TryGetValue (configName, out retConfig);

			if (null == retConfig) {
				retConfig = AssetDatabase.LoadAssetAtPath<PathConfig>(DefaultPathConfigGenerateForder + "/" + configName + ".asset");
				m_CachedPathConfigDict.Add (configName, retConfig);
			}

			return retConfig;
		}

		public static PathConfig GetPathConfig<T>() {
			string configName = typeof(T).ToString ();
			return Load (configName);
		}

		public static PathItem GetPathItem<T>(string pathName) {
			string configName = typeof(T).ToString ();
			return Load (configName) [pathName];
		}

		public static string GetPath<T>(string pathName)  {
			return GetPathItem<T>(pathName).Path;
		}

		public static string GetAssetPath<T>(string pathName){
			return "Assets/" + GetPathItem<T>(pathName).Path;
		}

	}
}
