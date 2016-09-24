using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;
using System.Xml;

public class IABSceneMgr 
{
	IABMgr mABMgr;


	public IABSceneMgr(string sceneName)
	{
		mABMgr = new IABMgr (sceneName);
	}

	Dictionary<string,string> mAllAsset = new Dictionary<string, string>();


		
	public IEnumerator ReadConfig()
	{
		WWW www = new WWW ("file://" + Application.streamingAssetsPath + "/QAB/iOS/resitems.xml");

		mAllAsset = new Dictionary<string,string> ();

		mABMgr = new IABMgr ("res");

		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			XmlDocument xml = new XmlDocument ();
			xml.LoadXml (www.text);
			Debug.LogError (www.text);
			XmlNodeList list = xml.SelectSingleNode ("config").ChildNodes;
			for (int i = 0; i < list.Count; i++) {
				mAllAsset.Add (list [i].Attributes ["abspath"].Value.ToString (),list [i].Attributes ["name"].Value.ToString ());
				Debug.LogError (list [i].Attributes ["abspath"].Value.ToString () + ":" + list [i].Attributes ["name"].Value.ToString ());
					
			}
		}
	}

	public void LoadAsset(string bundleName,LoaderProgress progress,LoadABCallBack callback)
	{
		if (mAllAsset.ContainsKey (bundleName)) {

			mABMgr.LoadAB (bundleName, progress, callback);
		}
		else {

			Debug.Log ("Dont contain the bundle ==" + bundleName);
		}

	}


	#region 由下层提供功能


	public string GetBundleRelateName(string bundleName)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			return mAllAsset [bundleName];
		}
		else {
			return null;
		}
	}

	public IEnumerator LoadAssetSync(string bundleName)
	{
		yield return mABMgr.LoadABs(bundleName);
	}


	public Object GetSingleRes(string bundleName,string resName)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			return mABMgr.GetSingleRes (bundleName, resName);
		}
		else {
			Debug.Log ("Dont conatin the bundnle ==" + bundleName);
			return null;
		}
	}

	public Object[] GetMultiRes(string bundleName,string resName)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			return mABMgr.GetMultiRes (mAllAsset [bundleName], resName);
		}
		else {
			Debug.Log ("Dont contain the  bundle == " + bundleName);
			return null;
		}
	}

	/// <summary>
	/// 释放单个资源
	/// </summary>
	public void DisposeResObj(string bundleName,string res)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			mABMgr.DisposeResObj (bundleName, res);	
		}
		else {
			Debug.Log ("Dont contain the bundle == " + bundleName + " :" + res);
		}
	}


	public void DisposeBundleRes(string bundleName)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			mABMgr.DisposeResObj (mAllAsset [bundleName]);
		}
		else {
			Debug.Log ("Dont contain the bundle == " + bundleName);
		}
	}

	/// <summary>
	/// 删除
	/// </summary>
	public void DisposeAllRes()
	{
		mABMgr.DisposeAllObj ();
	}



	public void DisposeBundle(string bundle)
	{
		if (mAllAsset.ContainsKey (bundle)) {
			mABMgr.DisposeBundle (bundle);
		}
	}

	public void DisposeAllBundle()
	{
		mABMgr.DisposeAllBundle ();

		mAllAsset.Clear ();
	}


	public void DisposeAllBundleAndRes()
	{
		mABMgr.DisposeAllBundleAndRes ();

		mAllAsset.Clear ();
	}

	#endregion


	public void DebugAllAsset()
	{
		List<string> keys = new List<string> ();

		keys.AddRange (mAllAsset.Keys);

		for (int i = 0; i < keys.Count; i++) {
			mABMgr.DebugAB (keys [i]);
		}
	}

	public bool IsLoadingFinish(string bundleName)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			return mABMgr.IsLoaddingFinish (mAllAsset [bundleName]);
		}
		else {
			Debug.Log ("is not contain bundle ==" + bundleName);
		}
		return false;
	}

	public bool IsLoadingAB(string bundleName)
	{
		if (mAllAsset.ContainsKey (bundleName)) {
			return mABMgr.IsLoadingAB (mAllAsset [bundleName]);
		}
		else {
			Debug.Log ("is not contain bundle ==" + bundleName);
		}
		return false;
	}
}
