using UnityEngine;
using System.Collections;
using QFramework;

public abstract class NPCBase : QMonoBehaviour {

	protected override void SetupMgr ()
	{
		mCurMgr = QNPCMgr.Instance;
	}
}
