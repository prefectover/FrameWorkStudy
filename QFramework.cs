using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using QFramework.UI;
using QFramework.AB;
using QFramework.PRIVATE;

namespace QFramework {
	/// <summary>
	/// 事件命令
	/// </summary>
	public class ControllerCommand : ICommand {
		public virtual void  Execute(IMessage message) {
		}
	}

	public class Instance : QMgrBehaviour {

		public static IEnumerator Init()
		{
			yield return QMsgCenter.Instance.Init ();

//			yield return QResMgr.Instance.Init ();

//			yield return QSoundMgr.Instance.Init ();

			// TODO:要配置 以后支持NGUI
			yield return QUGUIMgr.Init ();

			// 初始化框架
			if (mCtrl != null) yield return null;
			mCtrl = Controller.Instance;

			if (!LuaFramework.Util.CheckEnvironment()) yield return null;

			//-----------------关联命令-----------------------
			RegisterCommand(QFrameworkMsg.DISPATCH_MESSAGE, typeof(SocketCommand));

			//-----------------初始化管理器-----------------------
			AddMgr<LuaFramework.QLuaManager>();
			AddMgr<LuaFramework.TimerManager>();
			AddMgr<QSoundMgr> ();
			AddMgr<LuaFramework.NetworkManager>();
			AddMgr<LuaFramework.ResourceManager>();
			AddMgr<LuaFramework.ThreadManager>();
			AddMgr<LuaFramework.ObjectPoolManager>();
			AddMgr<GameManager>();
		}
			
		static Controller mCtrl;
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
			
		public static void RegisterCommand(QFrameworkMsg msgName, Type commandType) {
			mCtrl.RegisterCommand(msgName, commandType);
		}

		public static void RemoveCommand(QFrameworkMsg msgName) {
			mCtrl.RemoveCommand(msgName);
		}

		public static bool HasCommand(QFrameworkMsg msgName) {
			return mCtrl.HasCommand(msgName);
		}

		public static void RegisterMultiCommand(Type commandType, params QFrameworkMsg[] msgNames) {
			int count = msgNames.Length;
			for (int i = 0; i < count; i++) {
				RegisterCommand(msgNames[i], commandType);
			}
		}

		public static void RemoveMultiCommand(params QFrameworkMsg[] msgName) {
			int count = msgName.Length;
			for (int i = 0; i < count; i++) {
				RemoveCommand(msgName[i]);
			}
		}

		public static void SendMessageCommand(QFrameworkMsg message, object body = null) {
			mCtrl.ExecuteCommand(new Message(message, body));
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
