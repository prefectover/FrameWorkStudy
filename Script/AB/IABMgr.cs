using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

// 对一个场景的所有的 bundle包的管理

public delegate void LoadABCallBack(string sceneName,string bundle);

/// <summary>
/// 单个 存取
/// </summary>
public class AssetObj
{
	public List<Object> objs;

	public AssetObj(params Object[] tmpObjs)
	{
		this.objs = new List<Object> ();

		this.objs.AddRange (tmpObjs);
	}

	public void ReleaseObj()
	{
		for (int i = 0;i < objs.Count;i++)
		{
			Resources.UnloadAsset (objs [i]);
		}
	}
}

/// <summary>
/// 多个存取
/// </summary>
public class AssetResObj
{
	public Dictionary<string,AssetObj> resObjs;

	public AssetResObj(string name,AssetObj tmp)
	{
		resObjs = new Dictionary<string,AssetObj> ();

		resObjs.Add (name, tmp);
	}

	public void AddResObj(string name,AssetObj tmpObj)
	{
		resObjs.Add (name, tmpObj);
	}

	public void ReleaseAllResObj()
	{
		List<string> keys = new List<string> ();

		keys.AddRange (resObjs.Keys);

		for (int i = 0;i < keys.Count;i++)
		{
			ReleaseResObj (keys [i]);
		}
	}

	public void ReleaseResObj(string name)
	{
		if (resObjs.ContainsKey (name)) {
			AssetObj tmpObj = resObjs [name];

			tmpObj.ReleaseObj ();
		} else {
			Debug.Log ("release object name is not exit ==" + name);
		}
	}

	public List<Object> GetResObj(string name)
	{
		if (resObjs.ContainsKey (name)) {
			AssetObj tmpObj = resObjs [name];

			return tmpObj.objs;
		} else {
			Debug.Log ("release object name is not exit ==" + name);

			return null;
		}
	}
}


public class IABMgr  
{
	/// <summary>
	/// 把每一个包 都存起来
	/// </summary>
	Dictionary<string,IABRelationMgr> mLoadHelper = new Dictionary<string, IABRelationMgr>();

	Dictionary<string,AssetResObj> mLoadObj = new Dictionary<string, AssetResObj> ();

	string mBundleName;


	public IABMgr(string sceneName)
	{
		mSceneName = sceneName;
	}
	public bool IsLoadingAB(string bundleName)
	{
		if (!mLoadHelper.ContainsKey (bundleName)) {
			return false;
		} else {
			return true;
		}
	}
	#region 加载的几个接口

	string mSceneName;

	public void LoadAB(string bundleName,LoaderProgress progress,LoadABCallBack callback)
	{
		if (!mLoadHelper.ContainsKey (bundleName)) {
			IABRelationMgr loader = new IABRelationMgr ();

			loader.Initialize (bundleName, progress);

			mLoadHelper.Add (bundleName, loader);

			callback (mSceneName, bundleName);

		} else {
			Debug.Log ("IABMgr contain bundle name ==" + bundleName);
		}
	}

	public IEnumerator LoadABDependences(string bundleName,string refName,LoaderProgress progress)
	{
		if(!mLoadHelper.ContainsKey(bundleName))
		{
			IABRelationMgr loader = new IABRelationMgr();

			loader.Initialize(bundleName,progress);

			if (refName != null)

			{
				loader.AddReference(refName);
			}

			mLoadHelper.Add(bundleName,loader);

			yield return LoadABs(bundleName);
		}
		else
		{
			if (refName != null)
			{
				IABRelationMgr loader = mLoadHelper[bundleName];

				loader.AddReference(bundleName);
			}
		}
	}
	/// <summary>
	/// 加载assetBundle必须先加载manifest
	/// </summary> callback(sceneName,bundlename);返回上层 调用这个api
	/// <returns>The A.</returns>
	/// <param name="bundleName">Bundle name.</param>
	public IEnumerator LoadABs(string bundleName)
	{
		while (!IABManifestLoader.Instance.IsLoadFinish) {


			yield return null;
		}

		IABRelationMgr loader = mLoadHelper [bundleName];

		string[] dependences = GetDependences (bundleName);

		loader.SetDependences (dependences);

		for (int i = 0; i < dependences.Length; i++) {
			yield return LoadABDependences (dependences [i], bundleName, loader.GetProgress());
		}

		yield return loader.LoadAB ();
	}


	string[] GetDependences(string bundleName)
	{
		return IABManifestLoader.Instance.GetDepences (bundleName);
	}
	#endregion

	#region 释放的几个接口
	public void DisposeResObj(string bundleName,string resName)
	{
		if (mLoadObj.ContainsKey (bundleName)) {
			AssetResObj tmpObj = mLoadObj [bundleName];

			tmpObj.ReleaseResObj (resName);
		}
	}

	public void DisposeResObj(string bundleName)
	{
		if (mLoadObj.ContainsKey (bundleName)) {
			AssetResObj tmpObj = mLoadObj [bundleName];

			tmpObj.ReleaseAllResObj ();
		}

		Resources.UnloadUnusedAssets ();
	}

	public void DisposeAllObj()
	{
		List<string> keys = new List<string> ();

		keys.AddRange (mLoadObj.Keys);

		for (int i = 0; i < mLoadObj.Count; i++) {
			DisposeResObj (keys [i]);
		}

		mLoadObj.Clear ();
	}
	#endregion

	#region 由下层提供 API


	/// <summary>
	/// Debugs A.
	/// </summary>
	/// <param name="bundleName">Scene/test.prefab</param>
	public void DebugAB(string bundleName)
	{
		if (mLoadHelper.ContainsKey (bundleName)) 
		{
			IABRelationMgr loader = mLoadHelper [bundleName];

			loader.DebuggerAsset ();
		}
	}
		
	public bool IsLoaddingFinish(string bundleName)
	{
		if (mLoadHelper.ContainsKey (bundleName)) {
			IABRelationMgr loader = mLoadHelper [bundleName];

			return loader.IsBundleLoadFinish ();
		} else {
			Debug.Log ("IABRelation no contain bundle==" + bundleName);
			return false;
		}
	}


	public Object GetSingleRes(string bundleName,string resName)
	{
		// 表示 是否已经缓存了物体
		if (mLoadObj.ContainsKey (resName)) {
			AssetResObj tmpRes = mLoadObj [bundleName];

			List<Object> tmpObj = tmpRes.GetResObj (resName);

			if (tmpObj != null) {
				return tmpObj [0];
			}
		} 

		// 表示已经加载过bundle
		if (mLoadHelper.ContainsKey (bundleName)) {
			IABRelationMgr loader = mLoadHelper [bundleName];

			Object tmpObj = loader.GetSingleRes (resName);

			AssetObj tmpAssetObj = new AssetObj (tmpObj);

			if (mLoadObj.ContainsKey (bundleName)) {
				AssetResObj tmpRes = mLoadObj [bundleName];
				tmpRes.AddResObj (resName, tmpAssetObj);
			} else {
				// 没有加载过 这个包
				AssetResObj tmpRes = new AssetResObj (resName, tmpAssetObj);
				mLoadObj.Add (bundleName, tmpRes);
			}

			return tmpObj;
		} else {
			return null;
		}
	}


	public Object[] GetMultiRes(string bundleName,string resName)
	{
		if (mLoadHelper.ContainsKey (bundleName)) {
			IABRelationMgr loader = mLoadHelper [bundleName];

			Object[] tmpObjs = loader.GetMultiRes (resName);

			AssetObj tmpAssetObj = new AssetObj (tmpObjs);

			// 缓存里面 是否已经有这个包
			if (mLoadObj.ContainsKey (bundleName)) {
				AssetResObj tmpRes = mLoadObj [bundleName];

				tmpRes.AddResObj (resName, tmpAssetObj);
			} else {
				// 没有加载过 这个包
				AssetResObj tmpRes = new AssetResObj (resName, tmpAssetObj);

				mLoadObj.Add (bundleName, tmpRes);
			}

			return tmpObjs;
		} else {
			return null;
		}
	}

	/// <summary>
	/// 循环处理关系
	/// </summary>
	/// <param name="bundleName">Bundle name.</param>
	public void DisposeBundle(string bundleName)
	{
		if (mLoadHelper.ContainsKey (bundleName)) {
			IABRelationMgr loader = mLoadHelper [bundleName];

			List<string> dependences = loader.GetDependences ();

			for (int i = 0; i < dependences.Count; i++) {
				if (mLoadHelper.ContainsKey (dependences [i])) {
					IABRelationMgr dependence = mLoadHelper [dependences[i]];

					if (dependence.removeReference (bundleName)) {
						DisposeBundle (dependence.GetBundleName ());
					}
				}
			}

			if (loader.GetReference ().Count <= 0) {
				loader.Dispose ();

				mLoadHelper.Remove (bundleName);
			}
		}
	}

	public void DisposeAllBundle()
	{
		
	}

	/// <summary>
	/// 全部删除
	/// </summary>
	public void DisposeAllBundleAndRes()
	{
		DisposeAllObj ();

		List<string> keys = new List<string> ();

		keys.AddRange (mLoadHelper.Keys);

		for (int i = 0; i < mLoadHelper.Count; i++) {
			IABRelationMgr loader = mLoadHelper [keys [i]];

			loader.Dispose ();
		}

		mLoadHelper.Clear ();
	}

	#endregion
}
