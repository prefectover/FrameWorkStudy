using UnityEngine;
using System.Collections;
using System;

public class IABResLoader : IDisposable {

	AssetBundle mABRes;

	public IABResLoader(AssetBundle ab)
	{
		mABRes = ab;
	}

	public UnityEngine.Object this[string resName]
	{
		get {
			if (this.mABRes == null || !this.mABRes.Contains (resName)) {
				Debug.Log (resName +  " res not contain ");
				return null;
			}

			return mABRes.LoadAsset (resName);
		}
	}
		
	public UnityEngine.Object[] LoadRes(string resName)
	{
		if (this.mABRes == null || !this.mABRes.Contains (resName)) {
			Debug.Log (resName + " res not contain");
			return null;
		}

		return this.mABRes.LoadAssetWithSubAssets (resName);
	}

	/// <summary>
	/// 卸载单个资源
	/// </summary>
	public void UnloadRes(UnityEngine.Object resObj)
	{
		Resources.UnloadAsset (resObj);
	}

	/// <summary>
	/// 销毁
	/// </summary>
	/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="IABResLoader"/>. The <see cref="Dispose"/>
	/// method leaves the <see cref="IABResLoader"/> in an unusable state. After calling <see cref="Dispose"/>, you must
	/// release all references to the <see cref="IABResLoader"/> so the garbage collector can reclaim the memory that the
	/// <see cref="IABResLoader"/> was occupying.</remarks>
	public void Dispose()
	{
		if (this.mABRes == null)
			return;

		mABRes.Unload (false);
	}
		
	public void DebugAllRes()
	{
		string[] tmpAssetName = mABRes.GetAllAssetNames ();

		for (int i = 0; i < tmpAssetName.Length; i++) {
			Debug.Log (" ABRes Contain Asset Name == " + tmpAssetName);
		}
	}
}
