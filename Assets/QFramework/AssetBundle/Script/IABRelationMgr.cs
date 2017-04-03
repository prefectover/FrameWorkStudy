using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 处理关系
/// </summary>
public class IABRelationMgr  {
	/// <summary>
	///   -----> yy  
	/// xx					xx dependence yy aa
	///   -----> aa 
	/// </summary>
	List<string> dependBundle = null;

	string mBundleName;

	public string GetBundleName()
	{
		return mBundleName;
	}
	/// <summary>
	/// 表示 yy aa refer xx
	/// </summary>
	List<string> referenceBundle = null;

	IABLoader mAssetLoader;

	LoaderProgress mLoadProgress = null;

	public IABRelationMgr()
	{
		dependBundle = new List<string> ();
		referenceBundle = new List<string> ();
	}
		
	public LoaderProgress GetProgress()
	{
		return mLoadProgress;
	}

	public IABLoader GetLoader()
	{
		return mAssetLoader;
	}
	/// <summary>
	/// 添加 ref关系
	/// </summary>
	/// <param name="bundleName">Bundle name.</param>
	public void AddReference(string bundleName)
	{
		referenceBundle.Add (bundleName);
	}

	/// <summary>
	/// 获取 ref关系
	/// </summary>
	public List<string> GetReference()
	{
		return referenceBundle;
	}

	/// <summary>
	/// Removes the reference.
	/// </summary>
	/// <returns><c>true</c>, if reference was removed, <c>false</c> otherwise.</returns>
	/// <param name="bundleName">表示是否释放</param>
	public bool removeReference(string bundleName)
	{
		for (int i = 0; i < referenceBundle.Count; i++) {
			if (bundleName.Equals (referenceBundle [i])) {
				referenceBundle.RemoveAt (i);
			}
		}

		if (referenceBundle.Count <= 0) {
			Dispose ();

			return true;
		}

		return false;
	}


	public void SetDependences(string[] depenceABNames)
	{
		if (depenceABNames.Length > 0) {
			dependBundle.AddRange (depenceABNames);
		}
	}

	public List<string> GetDependences()
	{
		return dependBundle;
	}

	public void RemoveDepence(string bundleName)
	{
		for (int i = 0; i < dependBundle.Count; i++) {
			if (bundleName.Equals (dependBundle [i])) {
				dependBundle.RemoveAt (i);
			}
		}
	}

	bool IsLoadFinish;

	public void BundleLoadFinish(string bundleName)
	{
		IsLoadFinish = true;
	}
		
	public bool IsBundleLoadFinish()
	{
		return IsLoadFinish;
	}

	public void Initialize(string bundle,LoaderProgress progress)
	{
		IsLoadFinish = false;

		mBundleName = bundle;

		mLoadProgress = progress;

		mAssetLoader = new IABLoader (progress,BundleLoadFinish);
	}

	#region 由下层提供API

	public void DebuggerAsset()
	{
		if (null != mAssetLoader) {
			mAssetLoader.DebugerLoader ();
		} else {
			Debug.Log ("asset load is null");
		}
	}

	public IEnumerator LoadAB()
	{
		yield return mAssetLoader.CommonLoad ();
	}

	// 释放过程
	public void Dispose()
	{
		mAssetLoader.Dispose ();
	}

	public Object GetSingleRes(string bundleName)
	{
		return mAssetLoader.GetRes (bundleName);
	}

	public Object[] GetMultiRes(string bundleName)
	{
		return mAssetLoader.GetMultiRes (bundleName);
	}

	#endregion
}
