using UnityEngine;
using System.Collections;
using QFramework;

public abstract class AssetBase : QMonoBehaviour {

	protected override void SetupMgr ()
	{
		mCurMgr = QAssetMgr.Instance;
	}

}
