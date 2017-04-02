using UnityEngine;
using System.Collections;

using QFramework;

namespace QFramework.Example {

	/// <summary>
	/// 定义管理器,转发消息用的
	/// </summary>
	public class ExampleManager : QMgrBehaviour {

		public static ExampleManager Instance {
			get {
				return QMonoSingletonComponent<ExampleManager>.Instance;
			}
		}

		protected override void SetupMgrId ()
		{
			mMgrId = (ushort)QMgrID.Game;
		}
	}
		
	/// <summary>
	/// 定义事件
	/// </summary>
	public enum GameEvent {
		Begin = QMgrID.Game,
		SayHello,
		End,
	}

	public class AdvancedMsgDispatcherExample : QMonoBehaviour{

		IEnumerator Start() {

			// 注册消息
			RegisterSelf (this, new ushort[] {
				(ushort)GameEvent.SayHello
			});

			// 给自己发送消息
			yield return new WaitForSeconds (0.5f);
			this.SendMsg (new QMsg ((ushort)GameEvent.SayHello));
			yield return new WaitForSeconds (0.5f);
			this.SendMsg (new QMsg ((ushort)GameEvent.SayHello));
			yield return new WaitForSeconds (0.5f);
			this.SendMsg (new QMsg ((ushort)GameEvent.SayHello));
		}

		protected override void SetupMgr ()
		{
			mCurMgr = ExampleManager.Instance;
		}

		/// <summary>
		/// 处理消息
		/// </summary>
		public override void ProcessMsg (QMsg msg)
		{
			switch (msg.msgId) {
				case (ushort)GameEvent.SayHello:
					Debug.Log ("Say Hello");
					break;
			}
		}
			
	}

}