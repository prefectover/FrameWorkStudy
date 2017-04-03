using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Diagnostics;

public class NewEventSystemWithGeneric : QMonoBehaviour,IMsgSender,IMsgReceiver {

	public class QMsgWithStr : QMsg
	{
		public string str;
		public QMsgWithStr(ushort msgId,string str) :base(msgId) {
			this.str = str;
		}
	}

	protected override void SetupMgr ()
	{
		mCurMgr = Framework.Instance;
	}

	public override void ProcessMsg (QMsg msg)
	{
		switch (msg.msgId) {
			case (ushort)RECEIVE_MSG_FROM_SELF:
				string a = ((QMsgWithStr)msg).str;
				break;
		}
	}

	public const int RECEIVE_MSG_FROM_SELF = 1;

	// Use this for initialization
	void Start () {

		RegisterSelf (this, new ushort[] {
			(ushort)RECEIVE_MSG_FROM_SELF,
		});

		QEventSystem.Instance.Register (RECEIVE_MSG_FROM_SELF, delegate(int key, object[] param) {
			string a = param[0] as string;
		});

		QMsgDispatcher.RegisterGlobalMsg (this, RECEIVE_MSG_FROM_SELF.ToString (), delegate(object[] param) {
			string a = param[0] as string;

		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {
		if (GUI.Button (new Rect (100, 100, 200, 100), "Send A ")) {
			Stopwatch watch = new Stopwatch ();
			watch.Start ();
			for (int i = 0; i < 100000; i++){
				QEventSystem.Instance.Send (RECEIVE_MSG_FROM_SELF, "1");
			}
			watch.Stop ();
			UnityEngine.Debug.Log(watch.ElapsedMilliseconds);
		}

		if (GUI.Button (new Rect (100, 200, 200, 100), "SendB")) {
			Stopwatch watch = new Stopwatch ();
			watch.Start ();
			string a = RECEIVE_MSG_FROM_SELF.ToString ();
			for (int i = 0; i < 100000; i++){
				QMsgDispatcher.SendGlobalMsg(this,a, "1");
			}
			watch.Stop ();
			UnityEngine.Debug.Log(watch.ElapsedMilliseconds);
		}

		if (GUI.Button (new Rect (100, 300, 200, 100), "SendC")) {
			Stopwatch watch = new Stopwatch ();
			watch.Start ();
			for (int i = 0; i < 100000; i++){
				this.SendMsg (new QMsgWithStr ((ushort)RECEIVE_MSG_FROM_SELF, "1"));
			}
			watch.Stop ();
			UnityEngine.Debug.Log(watch.ElapsedMilliseconds);
		}
	}
}
