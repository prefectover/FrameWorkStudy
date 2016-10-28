using UnityEngine;
using System.Collections;

namespace QFrameworkLua {
	public class QLuaUtil  {

		/// <summary>
		/// 执行Lua方法
		/// </summary>
		public static object[] CallMethod(string module, string func, params object[] args) {
			QFrameworkLua.QLuaMgr luaMgr = QFramework.Instance.GetMgr<QFrameworkLua.QLuaMgr>();
			if (luaMgr == null) return null;
			return luaMgr.CallFunction(module + "." + func, args);
		}
	}
}
