using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using QFramework;

namespace QFrameworkLua {
	public class QLuaUtil  {

		/// <summary>
		/// 执行Lua方法
		/// </summary>
		public static object[] CallMethod(string module, string func, params object[] args) {
			QLuaMgr luaMgr = QLuaMgr.Instance;
			if (luaMgr == null) return null;
			return luaMgr.CallFunction(module + "." + func, args);
		}


		/// <summary>
		/// 防止初学者不按步骤来操作
		/// </summary>
		/// <returns></returns>
		public static int CheckRuntimeFile() {
			if (!Application.isEditor) return 0;
			string streamDir = Application.dataPath + "/StreamingAssets/";
			if (!Directory.Exists(streamDir)) {
				return -1;
			} else {
				string[] files = Directory.GetFiles(streamDir);
				if (files.Length == 0) return -1;

				if (!File.Exists(streamDir + "files.txt")) {
					return -1;
				}
			}
			// 这里用生成
			string sourceDir = Application.dataPath + "/ToLua/Source/Generate/";
			if (!Directory.Exists(sourceDir)) {
				return -2;
			} else {
				string[] files = Directory.GetFiles(sourceDir);
				if (files.Length == 0) return -2;
			}
			return 0;
		}

		/// <summary>
		/// 检查运行环境
		/// </summary>
		public static bool CheckLuaEnvironment() {
			#if UNITY_EDITOR 
			int resultId = QLuaUtil.CheckRuntimeFile();
			if (resultId == -1) {
			Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
			EditorApplication.isPlaying = false;
			return false;
			} else if (resultId == -2) {
			Debug.LogError("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
			EditorApplication.isPlaying = false;
			return false;
			}
			if (Application.loadedLevelName == "Test" && !QAppConst.DebugMode) {
			Debug.LogError("测试场景，必须打开调试模式，AppConst.DebugMode = true！！");
			EditorApplication.isPlaying = false;
			return false;
			}
			#endif
			return true;
		}
	}
}
