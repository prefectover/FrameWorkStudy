
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {

	public class Instance : QMgrBehaviour {

		protected override void SetupMgrId ()
		{
			mMgrId = 0;
		}
			
		protected override void SetupMgr ()
		{
			
		}

		public static IEnumerator Init()
		{
			yield return QMsgCenter.Instance.Init ();

//			yield return QResMgr.Instance.Init ();

//			yield return QSoundMgr.Instance.Init ();

			//-----------------初始化管理器-----------------------
			var a = QTimerMgr.Instance;
			var b = QSoundMgr.Instance;
			var c = QResourceManager.Instance;
			var d = QThreadMgr.Instance;
			var f = GameManager.Instance;
		}
	}
}
