using UnityEngine;
using System.Collections;
using QFramework;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using QFramework;

namespace QFramework {
	public class QResourceManager : MonoBehaviour
	{
		
	
		public static QResourceManager Instance {
			get {
				return QMonoSingletonComponent<QResourceManager>.Instance;
			}
		}
		/// <summary>
		/// 这个值跟PTHotUpdateManager.cs中的值保持一致，不要随意更改.
		/// 此处只可以获取，不可以赋值
		/// </summary>
		//private const string kpthasresupdated = "kpthasresupdated";


		private Dictionary<string,AssetBundleManager> _managerDic;
		private string _defaultGameObjectName = "AssetBundleManager";

		private const string default_projecttag = "qframework";


		public QResourceManager(){
			_managerDic = new Dictionary<string,AssetBundleManager> ();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected IEnumerator Init (Action<bool> action,string projectTag, AssetBundleManager manager)
		{
			var request = manager.InitializeAsync (projectTag);
			if (request != null) {
				yield return StartCoroutine (request);
				if (action != null) {
					action (true);
				}
			} else {
				#if UNITY_EDITOR
				if (action != null) {
					action (true);
				}
				#endif
			}
		}


		/// <summary>
		///  初始化加载manifest的异步方式
		/// </summary>
		/// <param name="action">Action.</param>
		public void InitAsync (Action<bool> action,string projectTag = null)
		{
			projectTag = projectTag == null ? default_projecttag : projectTag;
			string objectName =_defaultGameObjectName+"_"+projectTag;
			if (!_managerDic.ContainsKey(projectTag)) {
				GameObject go = new GameObject (objectName, typeof(AssetBundleManager));
				go.transform.parent = this.transform;
				_managerDic.Add (projectTag, go.transform.GetComponent<AssetBundleManager> ());
				StartCoroutine (Init (action, projectTag, _managerDic [projectTag]));
			}
		}

//		bool mInitialized = false;
		/// <summary>
		/// 初始化加载manifest的同步方式
		/// </summary>
		public void Init (string projectTag = null)
		{
			projectTag = projectTag == null ? default_projecttag : projectTag;
			string objectName =_defaultGameObjectName+"_"+projectTag;
			if (!_managerDic.ContainsKey(projectTag)) {
				GameObject go = new GameObject (objectName, typeof(AssetBundleManager));
				go.transform.parent = this.transform;
				_managerDic.Add (projectTag, go.transform.GetComponent<AssetBundleManager> ());
				_managerDic [projectTag].InitializeSync (projectTag);
			}
		}


		private AssetBundleManager GetManager(string projectTag){
			if (projectTag == null) {
				return _managerDic [default_projecttag];
			}
			if (_managerDic.ContainsKey(projectTag))
				return _managerDic [projectTag];
			return null;
		}

		/// <summary>
		/// 加载AssetBundle 同步方式
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T LoadAsset<T> (string assetBundleName, string  assetName, string projectTag = null) where T : UnityEngine.Object
		{
			assetBundleName = assetBundleName.ToLower ();
			if (string.IsNullOrEmpty (projectTag)) {

				projectTag = default_projecttag;
			}
			assetBundleName = RemapAssetBundleName (assetBundleName,projectTag);
			return GetManager(projectTag).LoadAsset<T> (assetBundleName, assetName, typeof(T));
		}

		/// <summary>
		/// 加载Asset 异步方式
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void LoadAssetAsync<T> (string assetBundleName, string  assetName, Action<bool, T> action, string projectTag = null) where T : UnityEngine.Object
		{
			if (string.IsNullOrEmpty (projectTag)) {

				projectTag = default_projecttag;
			}
			assetBundleName = assetBundleName.ToLower ();
			assetBundleName =RemapAssetBundleName (assetBundleName,projectTag);
			StartCoroutine (LoadFromAssetBundleAsync (assetBundleName, assetName, action, projectTag));
		}

		/// <summary>
		/// 异步加载AssetBundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="action">Action.</param>
		public void LoadAssetBundleAsync (string assetBundleName, Action<bool,AssetBundle> action, string projectTag = null)
		{
			if (string.IsNullOrEmpty (projectTag)) {

				projectTag = default_projecttag;
			}
			assetBundleName = assetBundleName.ToLower ();
			assetBundleName =RemapAssetBundleName (assetBundleName,projectTag);

			if (GetManager(projectTag).IsEditorSimulate ()) {

				AssetBundle ab = GetManager(projectTag).LoadAssetBundleSync (assetBundleName, false);
				if (action != null) {
					action (ab == null ? false : true, ab);
				}
			} else {
				StartCoroutine (LoadFromAssetBundleAsync<AssetBundle> (assetBundleName, null, action, projectTag));
			}
		}

		public void LoadAB(string assetBundleName, string projectTag = null)
		{
			LoadAssetBundle (assetBundleName);
		}
		/// <summary>
		/// 同步加载AssetBundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public void LoadAssetBundle (string assetBundleName, string projectTag = null)
		{
			if (string.IsNullOrEmpty (projectTag)) {
			
				projectTag = default_projecttag;
			}
			assetBundleName = assetBundleName.ToLower ();
			assetBundleName =RemapAssetBundleName (assetBundleName,projectTag);
			GetManager(projectTag).LoadAssetBundleSync (assetBundleName,false);
		}

		private IEnumerator LoadFromAssetBundleAsync<T> (string assetBundleName, string assetName, Action<bool, T> action, string projectTag = null)where T : UnityEngine.Object
		{
			assetBundleName = assetBundleName.ToLower ();
			
			float startTime = Time.realtimeSinceStartup;
			AssetBundleLoadAssetOperation request = GetManager(projectTag).LoadAssetAsync (assetBundleName, assetName, typeof(T));
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
		public void UnloadAssetBundle (string assetBundleName,bool force, string projectTag = null)
		{
			if (string.IsNullOrEmpty (projectTag)) {

				projectTag = default_projecttag;
			}
			assetBundleName = assetBundleName.ToLower ();
			assetBundleName =RemapAssetBundleName (assetBundleName,projectTag);

			GetManager(projectTag).UnloadAssetBundle (assetBundleName,force);
		}

		/// <summary>
		/// Gets the AB download progress.
		/// </summary>
		/// <returns>The AB download progress.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public float GetABDownloadProgress(string assetBundleName, string projectTag = null)
		{
			if (string.IsNullOrEmpty (projectTag)) {

				projectTag = default_projecttag;
			}
			assetBundleName = assetBundleName.ToLower ();
			assetBundleName =RemapAssetBundleName (assetBundleName,projectTag);

			return GetManager(projectTag).GetDownLoadProgress (assetBundleName);
		}
		/// <summary>
		/// 强制卸载所有assetbundle
		/// </summary>
		public void ForceUnloadAllAssetBundle (string projectTag = null)
		{
			if (string.IsNullOrEmpty (projectTag)) {

				projectTag = default_projecttag;
			}
			var manager = GetManager (projectTag);
			if (manager == null) {
				return;
			}

			manager.ForceUnloadAll ();

//			string objectName = default_projecttag;
//			if (projectTag != null) {
//				objectName =  projectTag;
//			}
			Destroy(_managerDic [projectTag].gameObject);
			_managerDic.Remove (projectTag);

		}

		private string RemapAssetBundleName(string abName,string projectTag){
			if (!string.IsNullOrEmpty(projectTag)&&projectTag != abName)
			{
				return abName + "_project_" + projectTag;
			}
			return abName;
		}

		// Update is called once per frame
		void Update ()
		{
	
		}
	}

}