#define ToLua

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using QFramework.UI;
using QFramework.AB;
using QFramework.PRIVATE;
using QFramework.Lua;

namespace QFramework {

	public class Instance : QMgrBehaviour {

		public static IEnumerator Init()
		{
			yield return QMsgCenter.Instance.Init ();

//			yield return QResMgr.Instance.Init ();

//			yield return QSoundMgr.Instance.Init ();

			// TODO:要配置 以后支持NGUI
			yield return QUGUIMgr.Init ();

			if (!LuaFramework.Util.CheckEnvironment()) yield return null;

			//-----------------初始化管理器-----------------------
			AddMgr<QLuaMgr>();
			AddMgr<LuaFramework.QTimerMgr>();
			AddMgr<QSoundMgr> ();
			AddMgr<LuaFramework.NetworkManager>();
			AddMgr<QResMgr>();
			AddMgr<LuaFramework.ThreadManager>();
			AddMgr<LuaFramework.ObjectPoolManager>();
			AddMgr<GameManager>();
		}
			
		static GameObject mRoot;
		static Dictionary<string, object> mMgrs = new Dictionary<string, object>();

		static GameObject FrameworkRoot {
			get {
				if (mRoot == null) {
					mRoot = GameObject.Find("QApp");
				}
				return mRoot;
			}
		}
			
		#region 负责管理 管理器
		/// <summary>
		/// 添加管理器
		/// </summary>
		public static  void AddMgr(string typeName, object obj) {
			if (!mMgrs.ContainsKey(typeName)) {
				mMgrs.Add(typeName, obj);
			}
		}

		/// <summary>
		/// 添加Unity对象
		/// </summary>
		public static T AddMgr<T>() where T : Component {
			string typeName = typeof(T).ToString();
			object result = null;
			mMgrs.TryGetValue(typeName, out result);
			if (result != null) {
				return (T)result;
			}
			Component c = FrameworkRoot.AddComponent<T>();
			mMgrs.Add(typeName, c);
			return default(T);
		}

		/// <summary>
		/// 获取系统管理器
		/// </summary>
		public static T GetMgr<T>() where T : class {
			string typeName = typeof(T).ToString();

			if (!mMgrs.ContainsKey(typeName)) {
				return default(T);
			}
			object manager = null;
			mMgrs.TryGetValue(typeName, out manager);
			return (T)manager;
		}
		#endregion

	}
}
