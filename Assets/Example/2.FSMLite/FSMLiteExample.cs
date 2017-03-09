using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using QFramework;

namespace QFramework {

	/// <summary>
	/// 轻量级状态机
	/// </summary>
	public class FSMLiteExample : MonoBehaviour {

		/// <summary>
		/// 状态
		/// </summary>
		public const string IDLE_STATE = "Idle";
		public const string RUN_STATE = "Run";
		public const string WALK_STATE = "Walk";
		public const string JUMP_STATE = "Jump";

		/// <summary>
		/// 跳转事件
		/// </summary>
		public const string RUN_CLICK_EVENT = "RunClick";
		public const string WALK_CLICK_EVENT = "WalkClick";
		public const string JUMP_CLICK_EVENT = "JumpClick";


		QFSMLite mFSM = new QFSMLite();

		// Use this for initialization
		IEnumerator Start () {

			mFSM.AddState (IDLE_STATE);
			mFSM.AddState (RUN_STATE);
			mFSM.AddState (WALK_STATE);
			mFSM.AddState (JUMP_STATE);

			// 白名单方式添加通道
			// 待机->跑
			mFSM.AddTranslation (IDLE_STATE, RUN_CLICK_EVENT, RUN_STATE, delegate {
				Debug.LogError("跳转中 IDLE_STATE->RUN_STATE");
			});

			// 待机->走
			mFSM.AddTranslation (IDLE_STATE, WALK_CLICK_EVENT, WALK_STATE, delegate {
				Debug.LogError("跳转中 IDLE_STATE->WALK_STATE");
			});

			// 待机->跳
			mFSM.AddTranslation (IDLE_STATE, JUMP_CLICK_EVENT, JUMP_STATE, delegate {
				Debug.LogError("跳转中 IDLE_STATE->JUMP_STATE");
			});
				

			mFSM.Start (IDLE_STATE);
			Debug.LogError ("Cur State:" + mFSM.State);
			yield return null;
		}


		void OnGUI() {
			if (GUI.Button (new Rect (0, 100, 200, 100), "Run")) {
				mFSM.HandleEvent (RUN_CLICK_EVENT);
				Debug.LogError ("Cur State:" + mFSM.State);
			}

			if (GUI.Button (new Rect (0, 200, 200, 100), "Walk")) {
				mFSM.HandleEvent (WALK_CLICK_EVENT);
				Debug.LogError ("Cur State:" + mFSM.State);
			}

			if (GUI.Button (new Rect (0, 300, 200, 100), "Jump")) {
				mFSM.HandleEvent (JUMP_CLICK_EVENT);
				Debug.LogError ("Cur State:" + mFSM.State);
			}

			if (GUI.Button(new Rect(0,400,200,100),"Reset")) {
				mFSM.Start(IDLE_STATE);
				Debug.LogError ("Cur State:" + mFSM.State);
			}
		}


	}

}
