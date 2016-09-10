using UnityEngine;
using System.Collections;

namespace QFramework {
	
	/// <summary>
	/// 所有的路径常量都在这里
	/// </summary>
	public class QPath 
	{
		/// <summary>
		/// 资源输出的路径
		/// </summary>
		public static string ABBuildOutPutDir(RuntimePlatform platform) {
			string retDirPath = null;
			switch (platform) {
			case RuntimePlatform.Android:
				retDirPath = Application.streamingAssetsPath + "/QAB/Android";
				break;
			case RuntimePlatform.IPhonePlayer:
				retDirPath = Application.streamingAssetsPath + "/QAB/iOS";
				break;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				retDirPath = Application.streamingAssetsPath + "/QAB/Windows";
				break;
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.OSXEditor:
				retDirPath = Application.streamingAssetsPath + "/QAB/OSX";
				break;
			}

			return retDirPath;
		}

		/// <summary>
		/// 打包之前的源资源文件
		/// </summary>
		public static string SrcABDir  {
			get {
				return Application.dataPath + "/QArt/QAB";
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
				string game = QAppConst.ABPath.ToLower();
				if (Application.isMobilePlatform) {
					return Application.persistentDataPath + "/" + game + "/";
				}
				if (QAppConst.DebugMode) {
					return Application.dataPath + "/" + QAppConst.AssetDir + "/";
				}
				if (Application.platform == RuntimePlatform.OSXEditor) {
					int i = Application.dataPath.LastIndexOf('/');
					return Application.dataPath.Substring(0, i + 1) + game + "/";
				}
				return "c:/" + game + "/";
			}
		}
	}
}
