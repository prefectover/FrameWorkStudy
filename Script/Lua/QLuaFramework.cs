
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
namespace QFrameworkLua {

	public class Instance  {

		public static IEnumerator Init()
		{
			yield return QMsgCenter.Instance.Init ();

			if (!QLuaUtil.CheckLuaEnvironment()) yield return null;

			var a = QLuaMgr.Instance;
			var b = QTimerMgr.Instance;
			var c = QSoundMgr.Instance;
			var d = QResMgr.Instance;
			var e = QThreadMgr.Instance;
			var f = QPoolManager.Instance;
			var g = GameManager.Instance;

			// 如果已经初始化过了 就执行Lua文件 (一般是从其他场景切换过来的时候) TODO:这样太危险
			if (QLuaMgr.Instance.Initialized) {
				QLuaMgr.Instance.ExecuteLuaFile (QLuaApp.Instance.luaFileName,"Main");
			}

		}
			
		static Dictionary<string, object> mMgrs = new Dictionary<string, object>();
	}
}
