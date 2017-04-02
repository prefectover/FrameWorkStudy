using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {

	public enum PATH_ROOT {
		ApplicationDataPath,
		ApplicationPersistentDataPath,
		ApplicationStreamingAssetsPath
	}

	[System.Serializable]
	public class PathItem {
		[Header("描述")]
		[SerializeField] string m_Description = "";
		[SerializeField] PATH_ROOT m_Root = PATH_ROOT.ApplicationDataPath;
		[SerializeField] string m_Name = "";
		[SerializeField] string m_Path = "";

		public string Name {
			get {
				return m_Name;
			}
		}

		public string Path {
			get {
				return m_Path;
			}
		}

		public string FullPath {
			get {
				switch (m_Root) {
				case PATH_ROOT.ApplicationDataPath:
					return Application.dataPath + "/" + m_Path;
				case PATH_ROOT.ApplicationPersistentDataPath:
					return PATH_ROOT.ApplicationPersistentDataPath + "/" + m_Path;
				case PATH_ROOT.ApplicationStreamingAssetsPath:
					return PATH_ROOT.ApplicationStreamingAssetsPath + "/" + m_Path;
				}
				return m_Path;
			}
		}

	}
		
	/// <summary>
	/// Path配置
	/// </summary>
	public class PathConfig : ScriptableObject {
		[SerializeField]  string Description;
		[SerializeField]  List<PathItem> m_PathList;

		public List<PathItem> List {
			get {
				return m_PathList;
			}
		}

		Dictionary<string,PathItem> mCachedPathDict;

		/// <summary>
		/// 根据Path做索引
		/// </summary>
		public PathItem this[string pathName] {
			get {
				if (null == mCachedPathDict) {
					mCachedPathDict = new Dictionary<string, PathItem> ();
					foreach (var pathItem in m_PathList) {
						if (!string.IsNullOrEmpty (pathItem.Name) && !mCachedPathDict.ContainsKey (pathItem.Name)) {
							mCachedPathDict.Add (pathItem.Name, pathItem);
						}
						else {
							Debug.LogError (pathItem.Name + ":" + mCachedPathDict.ContainsKey (pathItem.Name));
						}
					}
				}
	
				return mCachedPathDict[pathName];
			}
		}
	}
}
