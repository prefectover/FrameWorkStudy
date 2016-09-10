using UnityEngine;
using System.Collections;
using QFramework.AB;
using System;
using UnityEngine.UI;

namespace QFramework.AB
{
	public class QResMgr : QMonoSingleton<QResMgr>
	{

		public GameObject LoadUIPrefabSync(string uiName)
		{
			return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/QArt/QAB" + "/UIPrefab/" + uiName + ".prefab");
		}



		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected IEnumerator InitInner (Action<bool> action)
		{
			var request = QABMgr.Initialize ();
			if (request != null) 
			{
				yield return StartCoroutine (request);

				if (action != null) 
				{
					action (true);
				}
			}


		
		}
		private void SetABPath(bool isAsync)
		{
			Debug.Log ("***********hasResUpdated:"+hasResUpdated);
			if (hasResUpdated) 
			{
				QABMgr.SetSourceABURL (Application.persistentDataPath + "/QAB/" + QPlatform.GetPlatformName () + "/");

			} else {
				if (!isAsync&&Application.platform == RuntimePlatform.Android) {
					
					QABMgr.SetSourceABURL (Application.dataPath+"!assets" + "/QAB/" + QPlatform.GetPlatformName () + "/");
				} else {
					QABMgr.SetSourceABURL (Application.streamingAssetsPath + "/QAB/" + QPlatform.GetPlatformName () + "/");
				}

			}
		}

		/// <summary>
		///  初始化加载manifest的异步方式
		/// </summary>
		/// <param name="action">Action.</param>
		public void InitAsync (Action<bool> callback)
		{
			SetABPath (true);
			StartCoroutine (InitInner (callback));
		}

		/// <summary>
		/// 初始化加载manifest的同步方式
		/// </summary>
		public IEnumerator Init ()
		{
			SetABPath (false);
			QABMgr.InitializeSync ();
			yield return null;
		}

		/// <summary>
		/// 加载AssetBundle 异步方式
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T LoadAsset<T> (string bundleName, string  assetName) where T : UnityEngine.Object
		{
			bundleName = bundleName.ToLower ();
			return QABMgr.LoadAsset<T> (bundleName, assetName, typeof(T));
		}

		/// <summary>
		/// 加载Asset 异步方式
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void LoadResAsync<T> (string bundleName, string  assetName, Action<bool, T> action) where T : UnityEngine.Object
		{
			bundleName = bundleName.ToLower ();
			StartCoroutine (LoadFromABAsync (bundleName, assetName, action));
		}

		/// <summary>
		/// 异步加载AssetBundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="action">Action.</param>
		public void LoadABAsync (string assetBundleName, Action<bool,AssetBundle> action)
		{
			assetBundleName = assetBundleName.ToLower ();

			if (QABMgr.IsEditorDevMode ()) {

				AssetBundle ab = QABMgr.LoadABSync (assetBundleName, false);
				if (action != null) {
					action (ab == null ? false : true, ab);
				}
			} else {
				StartCoroutine (LoadFromABAsync<AssetBundle> (assetBundleName, null, action));
			}


		}

		/// <summary>
		/// 同步加载AssetBundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public IEnumerator LoadAB (string bundleName)
		{
			QABMgr.LoadABSync (bundleName,false);

			yield return null;
		}

		private IEnumerator LoadFromABAsync<T> (string bundleName, string assetName, Action<bool, T> action)where T : UnityEngine.Object
		{
			
			float startTime = Time.realtimeSinceStartup;
			ABLoadAssetOperation request = QABMgr.LoadAssetAsync (bundleName, assetName, typeof(T));
			if (request == null)
			{
				yield break;
			}
			yield return StartCoroutine (request);
			T prefab = request.GetAsset<T> ();
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log (assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
			if (action != null) 
			{
				action (prefab == null ? false : true, prefab);
			}
		}

		/// <summary>
		/// 卸载AssetBundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="force">If set to <c>true</c> force.</param>
		static public void UnloadAssetBundle (string assetBundleName,bool force)
		{
			QResMgr.UnloadAssetBundle (assetBundleName,force);
		}

		/// <summary>
		/// Gets the AB download progress.
		/// </summary>
		/// <returns>The AB download progress.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public float GetABDownloadProgress(string assetBundleName)
		{
			return QABMgr.GetDownLoadProgress (assetBundleName);
		}
		/// <summary>
		/// 强制卸载所有assetbundle
		/// </summary>
		static public void ForceUnloadAllAssetBundle ()
		{
			QABMgr.ForceUnloadAll ();
		}

		/// <summary>
		/// 这个值跟QResUpdateManager.cs中的值保持一致，不要随意更改.
		/// 此处只可以获取，不可以赋值
		/// </summary>
		private const string kpthasresupdated = "kpthasresupdated";

		private bool hasResUpdated {
			get {

				bool result = PlayerPrefs.GetInt (kpthasresupdated, 0) == 0 ? false : true;
				return result;
			}
		}
	}

}