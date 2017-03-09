using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using QFramework;

namespace QFramework {

	/// <summary>
	/// State a.
	/// </summary>
	public class StateA : QFSMState {

		public StateA (StateName name) : base ((ushort)name){}

		public override void OnEnter ()
		{
			Debug.Log ("State A OnEnter");
		}

		public override void OnExit ()
		{
			Debug.Log ("State A Exist");	
		}
	}

	public class StateB : QFSMState {

		public StateB (StateName name):base ((ushort)name){
		}

		public override void OnEnter ()
		{
			Debug.Log ("State B OnEnter");
		}
	}

	public class StateC : QFSMState {

		public StateC(StateName name):base((ushort)name) {
		}
		public override void OnEnter ()
		{
			Debug.Log ("State C OnEnter");
		}
	}


	public enum StateName {
		A,
		B,
		C,
	}


	/// <summary>
	/// 跳转事件
	/// </summary>
	public enum TranslationEvent {
		A_2_B,
		A_2_C,
	}


	public class AdvancedFSMExample : MonoBehaviour {

		QFSM mFSM = new QFSM();

		void Awake() {

			var stateA = new StateA (StateName.A);
			var stateB = new StateB (StateName.B);
			var stateC = new StateC (StateName.C);

			var a2b = new QFSMTranslation (stateA, (ushort)TranslationEvent.A_2_B, stateB);
			var a2c = new QFSMTranslation (stateA, (ushort)TranslationEvent.A_2_C, stateC);

			mFSM.AddState (stateA);
			mFSM.AddState (stateB);
			mFSM.AddState (stateC);

			mFSM.AddTranslation (a2b);
			mFSM.AddTranslation (a2c);

			mFSM.Start (stateA);



			mFSM.HandleEvent ((ushort)TranslationEvent.A_2_B);
		}

	}

}