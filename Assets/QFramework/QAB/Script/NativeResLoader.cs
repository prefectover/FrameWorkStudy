using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void NativeResCallback(NativeResCallbackNode node);

public class NativeResCallbackNode
{
	public string sceneName;

	public string bundleName;

	public string resName;

	public ushort backMsgId;


	public bool isSingle;

	public NativeResCallback callback;

	public NativeResCallbackNode nextValue;


	public NativeResCallbackNode(bool single,string sceneName,string bundleName,string resName,ushort backMsgId,NativeResCallback callback,NativeResCallbackNode next)
	{
		this.isSingle = single;
		this.sceneName = sceneName;
		this.bundleName = bundleName;
		this.resName = resName;
		this.backMsgId = backMsgId;
		this.callback = callback;
		this.nextValue = nextValue;
	}

	public void Dispose()
	{
		callback = null;
		nextValue = null;

		this.isSingle = false;
		this.sceneName = null;
		this.bundleName = null;
		this.resName = null;
		this.backMsgId = 0;
	}
}

public class NativeResCallbackMgr
{
	Dictionary<string,NativeResCallbackNode> mMgr = null;

	public NativeResCallbackMgr()
	{
		mMgr = new Dictionary<string, NativeResCallbackNode>();
	}

	/// <summary>
	/// 请求添加的过程
	/// </summary>
	public void AddBundle(string bundle,NativeResCallbackNode curNode)
	{
		if (mMgr.ContainsKey (bundle)) {
			NativeResCallbackNode tmpNode = mMgr [bundle];

			while (tmpNode.nextValue != null) {
				tmpNode = tmpNode.nextValue;
			}

			tmpNode.nextValue = curNode;
		}
		else {
			mMgr.Add (bundle, curNode);
		}
	}

	/// <summary>
	/// 加载完成后 消息向上层传递完成了 就把这些缓存的命令删除
	/// </summary>
	public void Dispose(string bundle)
	{
		if (mMgr.ContainsKey (bundle)) {
			NativeResCallbackNode tmpNode = mMgr [bundle];

			while (tmpNode.nextValue != null) {
				NativeResCallbackNode curNode = tmpNode;

				tmpNode = tmpNode.nextValue;

				curNode.Dispose ();
			}

			tmpNode.Dispose ();

			mMgr.Remove (bundle);
		}
	}


	public void CallbackRes(string bundle)
	{
		if (mMgr.ContainsKey (bundle)) {
			NativeResCallbackNode tmpNode = mMgr [bundle];

			do {
				tmpNode.callback(tmpNode);

				tmpNode = tmpNode.nextValue;
			}
			while (tmpNode != null);
		}
	}
}

public class NativeResLoader : AssetBase {

	public override void ProcessMsg (QMsg receiveMsg)
	{
		switch (receiveMsg.msgId) {
			case (ushort)AssetEvent.ReleaseSingleObj:
				{
					HunkAssetRes tmpMsg = (HunkAssetRes) receiveMsg;

//					ILoadMgr.Instance.UnloadResObj (tmpMsg.sceneName, tmpMsg.bundleName,tmpMsg.resName);
				}
				break;

			case (ushort)AssetEvent.ReleaseBundleObj:
				{
					HunkAssetRes tmpMsg = (HunkAssetRes) receiveMsg;

//					ILoadMgr.Instance.UnloadBundleResObjs (tmpMsg.sceneName, tmpMsg.bundleName);
				}
				break;
			
			case (ushort)AssetEvent.ReleaseSingleBundle:
				{
					HunkAssetRes tmpMsg = (HunkAssetRes) receiveMsg;

//					ILoadMgr.Instance.UnloadBundle (tmpMsg.sceneName, tmpMsg.bundleName);
				}
				break;
			case (ushort)AssetEvent.ReleaseSceneBundle:
				{
					HunkAssetRes tmpMsg = (HunkAssetRes) receiveMsg;

//					ILoadMgr.Instance.UnloadAllAB (tmpMsg.sceneName);
				}
				break;
			case (ushort)AssetEvent.ReleaseAll:
				{
					HunkAssetRes tmpMsg = (HunkAssetRes) receiveMsg;

//					ILoadMgr.Instance.UnLoadAllABAndResObjs (tmpMsg.sceneName);
				}
				break;


			case (ushort)AssetEvent.HunkRes:
				{
					HunkAssetRes tmpMsg = (HunkAssetRes)receiveMsg;

					GetRes (tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.resName, tmpMsg.isSingle, tmpMsg.backMsgId);
				}
				break;
		}
	}

	HunkAssetResBack resBackMsg = null;

	HunkAssetResBack ReleaseBack {

		get {
			if (resBackMsg == null) {
				resBackMsg = new HunkAssetResBack ();
			}

			return resBackMsg;
		}
	}

	public void SendToBackMsg(NativeResCallbackNode tmpNode)
	{
		if (tmpNode.isSingle) {
//			Object tmpObj = ILoadMgr.Instance.GetSingleRes (tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);

//			this.ReleaseBack.Changer (tmpNode.backMsgId, tmpObj);

//			SendMsg (ReleaseBack);
		}
		else {
//			Object[] tmpObj = ILoadMgr.Instance.GetMultiRes (tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);

//			this.ReleaseBack.Changer (tmpNode.backMsgId, tmpObj);

//			SendMsg (ReleaseBack);
		}
	}

	// sceneone/load.ld
	void LoadProgress(string bundleName,float progress)
	{
		if (progress >= 1.0f) {
			Callback.CallbackRes (bundleName);

			Callback.Dispose (bundleName);
		}
	}

	public void GetRes(string sceneName,string bundleName,string resName,bool isSingle,ushort backMsgId)
	{
//		// 没有加载 就执行加载
//		if (!ILoadMgr.Instance.IsLoadingAB (sceneName,bundleName)) {
//			ILoadMgr.Instance.LoadAsset (sceneName, bundleName, LoadProgress);
//
//			string bundleFullName = ILoadMgr.Instance.GetBundleRelateName (sceneName, bundleName);
//
//
//			if (bundleFullName != null) {
//				NativeResCallbackNode tmpNode = new NativeResCallbackNode (isSingle,sceneName, bundleName, resName, backMsgId, SendToBackMsg, null);
//
//				Callback.AddBundle (bundleName, tmpNode);
//			}
//			else {
//				Debug.LogError (" Do Not Contain Bundle ==" + bundleName);
//			}
//		}
//		// 已经加载并且完成 就直接返回给上层
//		else if (!ILoadMgr.Instance.IsLoadingBundleFinish (sceneName, bundleName)) {
//			if (isSingle) {
//				Object tmpObj = ILoadMgr.Instance.GetSingleRes (sceneName, bundleName, resName);
//
//				this.ReleaseBack.Changer (backMsgId, tmpObj);
//
//				SendMsg (ReleaseBack);
//			}
//			else {
//				Object[] tmpObj = ILoadMgr.Instance.GetMultiRes (sceneName, bundleName, resName);
//
//				this.ReleaseBack.Changer (backMsgId, tmpObj);
//
//				SendMsg (ReleaseBack);
//			}
//		}
//		// 已经在加载 但是没有加载完全，把你这个命令 存起来 等我加载完全了 再返回归你
//		else {
//			string bundleFullname = ILoadMgr.Instance.GetBundleRelateName (sceneName, bundleName);
//
//			if (bundleName != null) {
//
//				NativeResCallbackNode tmpNode = new NativeResCallbackNode (isSingle, sceneName, bundleName, resName, backMsgId, SendToBackMsg, null);
//
//				Callback.AddBundle (bundleFullname, tmpNode);
//			}
//			else {
//				Debug.LogWarning (" Do not contain bundle ==" + bundleName);
//			}
//		}

	}


	NativeResCallbackMgr callback = null;

	NativeResCallbackMgr Callback {
		get {
			if (callback == null) {
				callback = new NativeResCallbackMgr ();
			}

			return callback;
		}
	}
	void Awake()
	{
		mMsgIds = new ushort[] {
			(ushort)AssetEvent.ReleaseSingleObj,
			(ushort)AssetEvent.ReleaseBundleObj,
			(ushort)AssetEvent.ReleaseSingleBundle,
			(ushort)AssetEvent.ReleaseSceneBundle,
			(ushort)AssetEvent.ReleaseAll,
			(ushort)AssetEvent.HunkRes,
		};

		RegisterSelf (this, mMsgIds);
	}
}
