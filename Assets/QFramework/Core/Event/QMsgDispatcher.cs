using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace QFramework.Event {

	public enum QMsgChannel {
		Global,	// 全局
		UI,
		Logic,
	}

	/// <summary>
	/// 消息分发器
	/// C# this扩展 需要静态类
	/// </summary>
	public static class QMsgDispatcher  {

		/// <summary>
		/// 消息捕捉器
		/// </summary>
		class QMsgHandler {

			public IMsgReceiver receiver;
			public  QVoidDelegate.WithParams callback;

			/*
			 * VoidDelegate.WithParams 是一种委托 ,定义是这样的 
			 * 
			 *  public class VoidDelegate{
			 *  	public delegate void WithParams(params object[] paramList);
			 *  }
			 */
			public QMsgHandler(IMsgReceiver receiver,QVoidDelegate.WithParams callback)
			{
				this.receiver = receiver;
				this.callback = callback;
			}
		}
			
		/// <summary>
		/// 每个消息名字维护一组消息捕捉器。
		/// </summary>
		static Dictionary<QMsgChannel,Dictionary<string,List<QMsgHandler>>> mMsgHandlerDict = new Dictionary<QMsgChannel,Dictionary<string,List<QMsgHandler>>> ();
	
		/// <summary>
		/// 注册消息,
		/// 注意第一个参数,使用了C# this的扩展,
		/// 所以只有实现IMsgReceiver的对象才能调用此方法
		/// </summary>
		public static void RegisterGlobalMsg(this IMsgReceiver self, string msgName,QVoidDelegate.WithParams callback)
		{
			// 略过
			if (string.IsNullOrEmpty(msgName)) {
				Debug.LogError("RegisterMsg:" + msgName + " is Null or Empty");
				return;
			}

			// 略过
			if (null == callback) {
				Debug.LogError ("RegisterMsg:" + msgName + " callback is Null");
				return;
			}
				
			// 添加消息通道
			if (!mMsgHandlerDict.ContainsKey (QMsgChannel.Global)) {
				mMsgHandlerDict [QMsgChannel.Global] = new Dictionary<string, List<QMsgHandler>> ();
			}

			// 略过
			if (!mMsgHandlerDict[QMsgChannel.Global].ContainsKey (msgName)) {
				mMsgHandlerDict[QMsgChannel.Global] [msgName] = new List<QMsgHandler> ();
			}

			// 看下这里
			var handlers = mMsgHandlerDict [QMsgChannel.Global][msgName];

			// 略过
			// 防止重复注册
			foreach (var handler in handlers) {
				if (handler.receiver == self && handler.callback == callback) {
					Debug.LogWarning ("RegisterMsg:" + msgName + " ayready Register");
					return;
				}
			}

			// 再看下这里
			handlers.Add (new QMsgHandler (self, callback));
		}
			
		/// <summary>
		/// 注册消息,
		/// 注意第一个参数,使用了C# this的扩展,
		/// 所以只有实现IMsgReceiver的对象才能调用此方法
		/// </summary>
		public static void RegisterMsgByChannel(this IMsgReceiver self, QMsgChannel channel,string msgName,QVoidDelegate.WithParams callback)
		{
			// 略过
			if (string.IsNullOrEmpty(msgName)) {
				Debug.LogError("RegisterMsg:" + msgName + " is Null or Empty");
				return;
			}

			// 略过
			if (null == callback) {
				Debug.LogError ("RegisterMsg:" + msgName + " callback is Null");
				return;
			}

			// 添加消息通道
			if (!mMsgHandlerDict.ContainsKey (channel)) {
				mMsgHandlerDict [channel] = new Dictionary<string, List<QMsgHandler>> ();
			}

			// 略过
			if (!mMsgHandlerDict[channel].ContainsKey (msgName)) {
				mMsgHandlerDict[channel] [msgName] = new List<QMsgHandler> ();
			}

			// 看下这里
			var handlers = mMsgHandlerDict [channel][msgName];

			// 略过
			// 防止重复注册
			foreach (var handler in handlers) {
				if (handler.receiver == self && handler.callback == callback) {
					Debug.LogWarning ("RegisterMsg:" + msgName + " ayready Register");
					return;
				}
			}

			// 再看下这里
			handlers.Add (new QMsgHandler (self, callback));
		}


		/// <summary>
		/// 其实注销消息只需要Object和Go就足够了 不需要callback
		/// </summary>
		public static void UnRegisterGlobalMsg(this IMsgReceiver self,string msgName)
		{
			if (CheckStrNullOrEmpty (msgName)) {
				return;
			}
				
			if (!mMsgHandlerDict.ContainsKey (QMsgChannel.Global)) {
				Debug.LogError ("Channel:" + QMsgChannel.Global.ToString() + " doesn't exist");
				return;			
			}

			var handlers = mMsgHandlerDict[QMsgChannel.Global] [msgName];

			int handlerCount = handlers.Count;

			// 删除List需要从后向前遍历
			for (int index = handlerCount - 1; index >= 0; index--) {
				var handler = handlers [index];
				if (handler.receiver == self) {
					handlers.Remove (handler);
					break;
				}
			}
		}

		/// <summary>
		/// 其实注销消息只需要Object和Go就足够了 不需要callback
		/// </summary>
		public static void UnRegisterMsgByChannel(this IMsgReceiver self,QMsgChannel channel,string msgName)
		{
			if (CheckStrNullOrEmpty (msgName)) {
				return;
			}

			if (!mMsgHandlerDict.ContainsKey (channel)) {
				Debug.LogError ("Channel:" + channel.ToString () + " doesn't exist");
				return;			
			}

			var handlers = mMsgHandlerDict[channel] [msgName];

			int handlerCount = handlers.Count;

			// 删除List需要从后向前遍历
			for (int index = handlerCount - 1; index >= 0; index--) {
				var handler = handlers [index];
				if (handler.receiver == self) {
					handlers.Remove (handler);
					break;
				}
			}
		}

			
		static bool CheckStrNullOrEmpty(string str)
		{
			if (string.IsNullOrEmpty (str)) {
				Debug.LogWarning ("str is Null or Empty");
				return true;
			}
			return false;
		}

		static bool CheckDelegateNull(QVoidDelegate.WithParams callback)
		{
			if (null == callback) {
				Debug.LogWarning ("callback is Null");

				return true;
			}
			return false;
		}

		/// <summary>
		/// 发送消息
		/// 注意第一个参数
		/// </summary>
		public static void SendGlobalMsg(this IMsgSender sender, string msgName,params object[] paramList)
		{
			if (CheckStrNullOrEmpty (msgName)) {
				return;
			}

			if (!mMsgHandlerDict.ContainsKey (QMsgChannel.Global)) {
				Debug.LogError ("Channel:" + QMsgChannel.Global.ToString() + " doesn't exist");
				return;
			}
				
			// 略过,不用看
			if (!mMsgHandlerDict[QMsgChannel.Global].ContainsKey(msgName)){
				Debug.LogError("SendMsg is UnRegister");
				return;
			}

			// 开始看!!!!
			var handlers = mMsgHandlerDict[QMsgChannel.Global][msgName];

			var handlerCount = handlers.Count;

			// 之所以是从后向前遍历,是因为  从前向后遍历删除后索引值会不断变化
			// 参考文章,http://www.2cto.com/kf/201312/266723.html
			for (int index = handlerCount - 1;index >= 0;index--)
			{
				var handler = handlers[index];

				if (handler.receiver != null) {
					Debug.Log ("SendLogicMsg:" + msgName + " Succeed");
					handler.callback (paramList);
				} else {
					handlers.Remove (handler);
				}
			}
		}
			
		public static void SendMsgByChannel(this IMsgSender sender,QMsgChannel channel,string msgName,params object[] paramList)
		{
			if (CheckStrNullOrEmpty (msgName)) {
				return;
			}

			if (!mMsgHandlerDict.ContainsKey (channel)) {
				Debug.LogError ("Channel:" +channel.ToString() + " doesn't exist");
				return;
			}
				
			// 略过,不用看
			if (!mMsgHandlerDict[channel].ContainsKey(msgName)){
				Debug.LogWarning("SendMsg is UnRegister");
				return;
			}

			// 开始看!!!!
			var handlers = mMsgHandlerDict[channel][msgName];

			var handlerCount = handlers.Count;

			// 之所以是从后向前遍历,是因为  从前向后遍历删除后索引值会不断变化
			// 参考文章,http://www.2cto.com/kf/201312/266723.html
			for (int index = handlerCount - 1;index >= 0;index--)
			{
				var handler = handlers[index];

				if (handler.receiver != null) {
					Debug.Log ("SendLogicMsg:" + msgName + " Succeed");
					handler.callback (paramList);
				} else {
					handlers.Remove (handler);
				}
			}
		}
			
		[Obsolete("RegisterLogicMsg已经弃用了,请使用RegisterGlobalMsg")]
		public static void RegisterLogicMsg(this IMsgReceiver self, string msgName,QVoidDelegate.WithParams callback,QMsgChannel channel = QMsgChannel.Global)
		{
			if (CheckStrNullOrEmpty (msgName)||CheckDelegateNull(callback)) {
				return;
			}

			// 添加消息通道
			if (!mMsgHandlerDict.ContainsKey (channel)) {
				mMsgHandlerDict [channel] = new Dictionary<string, List<QMsgHandler>> ();
			}

			// 略过
			if (!mMsgHandlerDict[channel].ContainsKey (msgName)) {
				mMsgHandlerDict[channel] [msgName] = new List<QMsgHandler> ();
			}

			// 看下这里
			var handlers = mMsgHandlerDict [channel][msgName];

			// 略过
			// 防止重复注册
			foreach (var handler in handlers) {
				if (handler.receiver == self && handler.callback == callback) {
					Debug.LogWarning ("RegisterMsg:" + msgName + " ayready Register");
					return;
				}
			}

			// 再看下这里
			handlers.Add (new QMsgHandler (self, callback));
		}


		[Obsolete("SendLogicMsg已经弃用了,请使用使用SendGlobalMsg或SendMsgByChannel")]
		public static void SendLogicMsg(this IMsgSender sender, string msgName,params object[] paramList)
		{
			if (CheckStrNullOrEmpty (msgName)) {
				return;
			}

			if (!mMsgHandlerDict.ContainsKey (QMsgChannel.Global)) {
				Debug.LogError ("Channel:" + QMsgChannel.Global.ToString() + " doesn't exist");
				return;
			}


			// 略过,不用看
			if (!mMsgHandlerDict[QMsgChannel.Global].ContainsKey(msgName)){
				Debug.LogWarning("SendMsg is UnRegister");
				return;
			}

			// 开始看!!!!
			var handlers = mMsgHandlerDict[QMsgChannel.Global][msgName];

			var handlerCount = handlers.Count;

			// 之所以是从后向前遍历,是因为  从前向后遍历删除后索引值会不断变化
			// 参考文章,http://www.2cto.com/kf/201312/266723.html
			for (int index = handlerCount - 1;index >= 0;index--)
			{
				var handler = handlers[index];

				if (handler.receiver != null) {
					Debug.Log ("SendLogicMsg:" + msgName + " Succeed");
					handler.callback (paramList);
				} else {
					handlers.Remove (handler);
				}
			}
		}

		[Obsolete("UnRegisterMsg已经弃用了,请使用UnRegisterMsg")]
		public static void UnRegisterMsg(this IMsgReceiver self,string msgName,QVoidDelegate.WithParams callback,QMsgChannel channel = QMsgChannel.Global)
		{
			if (CheckStrNullOrEmpty (msgName) || CheckDelegateNull(callback)) {
				return;
			}
				
			// 添加通道
			if (!mMsgHandlerDict.ContainsKey (channel)) {
				Debug.LogError ("Channel:" + channel.ToString () + " doesn't exist");
				return;
			}

			var handlers = mMsgHandlerDict [channel] [msgName];

			int handlerCount = handlers.Count;

			// 删除List需要从后向前遍历
			for (int index = handlerCount - 1; index >= 0; index--) {
				var handler = handlers [index];
				if (handler.receiver == self && handler.callback == callback) {
					handlers.Remove (handler);
					break;
				}
			}
		}


		[Obsolete("UnRegisterMsg已经弃用了,请使用UnRegisterGlobalMsg")]
		public static void UnRegisterMsg(this IMsgReceiver self,string msgName)
		{
			if (CheckStrNullOrEmpty (msgName)) {
				return;
			}

			if (!mMsgHandlerDict.ContainsKey (QMsgChannel.Global)) {
				Debug.LogError ("Channel:" + QMsgChannel.Global.ToString() + " doesn't exist");
				return;			
			}

			var handlers = mMsgHandlerDict[QMsgChannel.Global] [msgName];

			int handlerCount = handlers.Count;

			// 删除List需要从后向前遍历
			for (int index = handlerCount - 1; index >= 0; index--) {
				var handler = handlers [index];
				if (handler.receiver == self) {
					handlers.Remove (handler);
					break;
				}
			}
		}
	}
}
