//
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using QFramework.AB;
//using System;
//using System.IO;
//using UnityEngine.UI;
//using UObject = UnityEngine.Object;
//
//namespace QFramework
//{
//	public class QResourceManager : QMgrBehaviour
//	{
//
//		protected override void SetupMgrId ()
//		{
//			mMgrId = 0;
//		}
//
//		protected override void SetupMgr ()
//		{
//			
//		}
//
//		public static QResourceManager Instance {
//			get {
//				return QMonoSingletonComponent<QResourceManager>.Instance;
//			}
//		}
//
//		public static void Dispose()
//		{
//			QMonoSingletonComponent<QResourceManager>.Dispose ();
//		}
//
//
//		void Awake() {
//			DontDestroyOnLoad (this);
//		}
//
//		/// <summary>
//		/// Initialize this instance.
//		/// </summary>
//		protected IEnumerator InitInner (Action<bool> action)
//		{
//			var request = QABMgr.Initialize ();
//			if (request != null) 
//			{
//				yield return StartCoroutine (request);
//
//				if (action != null) 
//				{
//					action (true);
//				}
//			}
//		}
//
//		private void SetABPath(bool isAsync)
//		{
//			Debug.Log ("***********hasResUpdated:"+hasResUpdated);
//			if (hasResUpdated) 
//			{
//				QABMgr.SetSourceABURL (Application.persistentDataPath + "/QAB/" + QPlatform.GetPlatformName () + "/");
//
//			} else {
//				if (!isAsync&&Application.platform == RuntimePlatform.Android) {
//					
//					QABMgr.SetSourceABURL (Application.dataPath+"!assets" + "/QAB/" + QPlatform.GetPlatformName () + "/");
//				} else {
//					QABMgr.SetSourceABURL (Application.streamingAssetsPath + "/QAB/" + QPlatform.GetPlatformName () + "/");
//				}
//			}
//		}
//
//		/// <summary>
//		///  初始化加载manifest的异步方式
//		/// </summary>
//		/// <param name="action">Action.</param>
//		public void InitAsync (Action<bool> callback)
//		{
//			SetABPath (true);
//			StartCoroutine (InitInner (callback));
//		}
//
//		/// <summary>
//		/// 初始化加载manifest的同步方式
//		/// </summary>
//		public void Init ()
//		{
//			SetABPath (false);
//			QABMgr.InitializeSync ();
//		}
//
//		/// <summary>
//		/// 加载AssetBundle 同步加载方式
//		/// </summary>
//		/// <returns>The asset.</returns>
//		/// <param name="assetBundleName">Asset bundle name.</param>
//		/// <param name="assetName">Asset name.</param>
//		/// <typeparam name="T">The 1st type parameter.</typeparam>
//		public T LoadAsset<T> (string bundleName, string  assetName) where T : UnityEngine.Object
//		{
//			bundleName = bundleName.ToLower ();
//			return QABMgr.LoadAsset<T> (bundleName, assetName, typeof(T));
//		}
//
//		/// <summary>
//		/// 加载Asset 异步方式
//		/// </summary>
//		/// <param name="assetBundleName">Asset bundle name.</param>
//		/// <param name="assetName">Asset name.</param>
//		/// <param name="action">Action.</param>
//		/// <typeparam name="T">The 1st type parameter.</typeparam>
//		public void LoadResAsync<T> (string bundleName, string  assetName, Action<bool, T> action) where T : UnityEngine.Object
//		{
//			bundleName = bundleName.ToLower ();
//			StartCoroutine (LoadFromABAsync (bundleName, assetName, action));
//		}
//
//		/// <summary>
//		/// 异步加载AssetBundle
//		/// </summary>
//		/// <param name="assetBundleName">Asset bundle name.</param>
//		/// <param name="action">Action.</param>
//		public void LoadABAsync (string assetBundleName, Action<bool,AssetBundle> action)
//		{
//			assetBundleName = assetBundleName.ToLower ();
//
//			if (QABMgr.IsEditorDevMode ()) {
//
//				AssetBundle ab = QABMgr.LoadABSync (assetBundleName, false);
//				if (action != null) {
//					action (ab == null ? false : true, ab);
//				}
//			} else {
//				StartCoroutine (LoadFromABAsync<AssetBundle> (assetBundleName, null, action));
//			}
//		}
//
//		/// <summary>
//		/// 同步加载AssetBundle
//		/// </summary>
//		/// <param name="assetBundleName">Asset bundle name.</param>
//		public void LoadAB (string bundleName)
//		{
//			QABMgr.LoadABSync (bundleName,false);
//		}
//
//		private IEnumerator LoadFromABAsync<T> (string bundleName, string assetName, Action<bool, T> action)where T : UnityEngine.Object
//		{
//			
//			float startTime = Time.realtimeSinceStartup;
//			ABLoadAssetOperation request = QABMgr.LoadAssetAsync (bundleName, assetName, typeof(T));
//			if (request == null)
//			{
//				yield break;
//			}
//			yield return StartCoroutine (request);
//			T prefab = request.GetAsset<T> ();
//			float elapsedTime = Time.realtimeSinceStartup - startTime;
//			Debug.Log (assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
//			if (action != null) 
//			{
//				action (prefab == null ? false : true, prefab);
//			}
//		}
//
//		/// <summary>
//		/// 卸载AssetBundle
//		/// </summary>
//		/// <param name="assetBundleName">Asset bundle name.</param>
//		/// <param name="force">If set to <c>true</c> force.</param>
//		static public void UnloadAssetBundle (string assetBundleName,bool force)
//		{
//			QResourceManager.UnloadAssetBundle (assetBundleName,force);
//		}
//
//		/// <summary>
//		/// Gets the AB download progress.
//		/// </summary>
//		/// <returns>The AB download progress.</returns>
//		/// <param name="assetBundleName">Asset bundle name.</param>
//		public float GetABDownloadProgress(string assetBundleName)
//		{
//			return QABMgr.GetDownLoadProgress (assetBundleName);
//		}
//		/// <summary>
//		/// 强制卸载所有assetbundle
//		/// </summary>
//		static public void ForceUnloadAllAssetBundle ()
//		{
//			QABMgr.ForceUnloadAll ();
//		}
//
//		/// <summary>
//		/// 这个值跟QResUpdateManager.cs中的值保持一致，不要随意更改.
//		/// 此处只可以获取，不可以赋值
//		/// </summary>
//		private const string kpthasresupdated = "kpthasresupdated";
//
//		private bool hasResUpdated {
//			get {
//
//				bool result = PlayerPrefs.GetInt (kpthasresupdated, 0) == 0 ? false : true;
//				return result;
//			}
//		}
//
//		#region LuaFrameworkUGUI
//		private string[] m_Variants = { };
//		private AssetBundleManifest manifest;
//		private AssetBundle shared, assetbundle;
//		private Dictionary<string, AssetBundle> bundles;
//	
//		/// <summary>
//		/// 初始化
//		/// </summary>
//		public void Initialize() {
//			byte[] stream = null;
//			string uri = string.Empty;
//			bundles = new Dictionary<string, AssetBundle>();
//			uri = QUtil.DataPath + "StreamingAssets";
//			if (!File.Exists(uri)) return;
//			stream = File.ReadAllBytes(uri);
//			assetbundle = AssetBundle.LoadFromMemory(stream);
//			manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
//		}
//
//		/// <summary>
//		/// 载入素材
//		/// </summary>
//		public T LoadAssetU<T>(string abname, string assetname) where T : UnityEngine.Object {
//			abname = abname.ToLower();
//			AssetBundle bundle = LoadAssetBundle(abname);
//			return bundle.LoadAsset<T>(assetname);
//		}
//
//		#if ToLua
//		public void LoadPrefab(string abName, string[] assetNames, LuaFunction func) {
//			abName = abName.ToLower();
//			List<UObject> result = new List<UObject>();
//			for (int i = 0; i < assetNames.Length; i++) {
//				UObject go = LoadAsset<UObject>(abName, assetNames[i]);
//				if (go != null) result.Add(go);
//			}
//			if (func != null) func.Call((object)result.ToArray());
//		}
//		#endif
//
//		/// <summary>
//		/// 载入AssetBundle
//		/// </summary>
//		/// <param name="abname"></param>
//		/// <returns></returns>
//		public AssetBundle LoadAssetBundle(string abname) {
//			if (!abname.EndsWith(QAppConst.ExtName)) {
//				abname += QAppConst.ExtName;
//			}
//			AssetBundle bundle = null;
//			if (!bundles.ContainsKey(abname)) {
//				byte[] stream = null;
//				string uri = QUtil.DataPath + abname;
//				Debug.LogWarning("LoadFile::>> " + uri);
//				LoadDependencies(abname);
//
//				stream = File.ReadAllBytes(uri);
//				bundle = AssetBundle.LoadFromMemory(stream); //关联数据的素材绑定
//				bundles.Add(abname, bundle);
//			} else {
//				bundles.TryGetValue(abname, out bundle);
//			}
//			return bundle;
//		}
//
//		/// <summary>
//		/// 载入依赖
//		/// </summary>
//		/// <param name="name"></param>
//		void LoadDependencies(string name) {
//			if (manifest == null) {
//				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
//				return;
//			}
//			// Get dependecies from the AssetBundleManifest object..
//			string[] dependencies = manifest.GetAllDependencies(name);
//			if (dependencies.Length == 0) return;
//
//			for (int i = 0; i < dependencies.Length; i++)
//				dependencies[i] = RemapVariantName(dependencies[i]);
//
//			// Record and load all dependencies.
//			for (int i = 0; i < dependencies.Length; i++) {
//				LoadAssetBundle(dependencies[i]);
//			}
//		}
//
//		// Remaps the asset bundle name to the best fitting asset bundle variant.
//		string RemapVariantName(string assetBundleName) {
//			string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
//
//			// If the asset bundle doesn't have variant, simply return.
//			if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
//				return assetBundleName;
//
//			string[] split = assetBundleName.Split('.');
//
//			int bestFit = int.MaxValue;
//			int bestFitIndex = -1;
//			// Loop all the assetBundles with variant to find the best fit variant assetBundle.
//			for (int i = 0; i < bundlesWithVariant.Length; i++) {
//				string[] curSplit = bundlesWithVariant[i].Split('.');
//				if (curSplit[0] != split[0])
//					continue;
//
//				int found = System.Array.IndexOf(m_Variants, curSplit[1]);
//				if (found != -1 && found < bestFit) {
//					bestFit = found;
//					bestFitIndex = i;
//				}
//			}
//			if (bestFitIndex != -1)
//				return bundlesWithVariant[bestFitIndex];
//			else
//				return assetBundleName;
//		}
//
//		/// <summary>
//		/// 销毁资源
//		/// </summary>
//		void OnDestroy() {
//			if (shared != null) shared.Unload(true);
//			if (manifest != null) manifest = null;
//			Debug.Log("~ResourceManager was destroy!");
//		}
//		#endregion
//
//
//		#region 对ToLua提供支持不能使用泛型
//		public GameObject LoadPrefab(string bundlename,string asset) {
//			return LoadAsset<GameObject> (bundlename, asset);
//		}
//		#endregion
//	}
//
//}
//
