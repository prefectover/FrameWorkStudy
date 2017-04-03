using UnityEngine;
using System.Collections;

namespace QFramework {
	
	/// <summary>
	/// 所有的路径常量都在这里
	/// </summary>
	public class QPath 
	{
		/// <summary>
		/// 相对资源路径.
		/// </summary>
		public const string RelativeABPath = "QArt/QAB";

		/// <summary>
		/// 绝对资源路径
		/// </summary>
		public static string SrcABDir  {
			get {
				return Application.dataPath + "/QArt/QAB";
			}
		}

		public static string StreamingAssetsABDir {
			get {
				return Application.streamingAssetsPath + "/QAB";
			}
		}
			
		public static string LuaSrcPath {
			get {
				return SrcABDir + "/Lua/";
			}
		}
			
		public static string ToLuaFilePath {
			get {
				return Application.dataPath + "/ToLua/Lua/";
			}
		}
			
		/// <summary>
		/// 取得数据存放目录
		/// </summary>
		public static string DataPath {
			get {
				string game = QPath.RelativeABPath.ToLower();
				if (Application.isMobilePlatform) {
					return Application.persistentDataPath + "/" + game + "/";
				}
				if (Application.platform == RuntimePlatform.OSXEditor) {
					int i = Application.dataPath.LastIndexOf('/');
					return Application.dataPath.Substring(0, i + 1) + game + "/";
				}
				return "c:/" + game + "/";
			}
		}
			
		public static string FrameworkPath {
			get {
				return Application.dataPath + "/" + QPath.RelativeABPath;
			}
		}


	}
}
