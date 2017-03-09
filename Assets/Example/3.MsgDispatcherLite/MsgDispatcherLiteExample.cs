using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using QFramework;

namespace QFramework.Example {

	/// <summary>
	/// 轻量级消息机制
	/// </summary>
	public class MsgDispatcherLiteExample : MonoBehaviour,IMsgReceiver,IMsgSender {

		/// <summary>
		/// 消息体定义
		/// </summary>
		public const string RECEIVE_MSG_FROM_OTHER_OBJECT = "ReceiveMsgFromOtherObject";

		void Awake() {
			/// <summary>
			/// 接收消息
			/// 需要实现IMsgReceiver接口
			/// </summary>
			this.RegisterGlobalMsg (RECEIVE_MSG_FROM_OTHER_OBJECT, delegate(object[] paramList) {
				Debug.Log("ReceiveMsg");
				foreach(object msgContentItem in paramList) {
					Debug.Log(msgContentItem);
				}
			});
				
			this.RegisterMsgByChannel (QMsgChannel.UI, RECEIVE_MSG_FROM_OTHER_OBJECT, delegate(object[] paramList) {
				Debug.Log("这里接收不到消息,因为通道不一样");	
			});
		}
			
		/// <summary>
		/// 发送消息
		/// 需要实现IMsgSender接口
		/// </summary>
		void OnGUI() {
			if (GUI.Button (new Rect (200, 200, 200, 100), "Send Msg")) {
				this.SendGlobalMsg (RECEIVE_MSG_FROM_OTHER_OBJECT, new object[]{ "1", "2", 123});
			}
		}


		void OnDestroy() {
			this.UnRegisterGlobalMsg (RECEIVE_MSG_FROM_OTHER_OBJECT);
			this.UnRegisterMsgByChannel (QMsgChannel.UI, RECEIVE_MSG_FROM_OTHER_OBJECT);
		}
	}
}

