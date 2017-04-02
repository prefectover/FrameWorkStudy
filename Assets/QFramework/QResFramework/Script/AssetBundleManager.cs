using UnityEngine;


#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;

namespace QFrameworkAB
{	
	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class LoadedAssetBundle
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;
		
		public LoadedAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}
	
	// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
	public class AssetBundleManager : MonoBehaviour
	{

		string m_BaseDownloadingURL = "";
		string[] m_ActiveVariants =  {  };

		private string _projectTag = null;


		AssetBundleManifest m_AssetBundleManifest = null;

	
		Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle> ();
		Dictionary<string, AssetBundleCreateRequest> m_DownloadingWWWs = new Dictionary<string, AssetBundleCreateRequest> ();
		Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();
		List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation> ();
		Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();
	

		#if UNITY_EDITOR
		private int m_SimulateAssetBundleInEditor = -1;
		private const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改
		#endif

		#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public bool SimulateAssetBundleInEditor {
			get {
				if (m_SimulateAssetBundleInEditor == -1)
				{
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool (kSimulateAssetBundles, true) ? 1 : 0;
				}
				return m_SimulateAssetBundleInEditor != 0;
			}
			set {
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor) 
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool (kSimulateAssetBundles, value);
				}
			}
		}
		#endif

	
		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
		private string BaseDownloadingURL {
			get { return m_BaseDownloadingURL; }
			set { m_BaseDownloadingURL = value; }
		}
	
		// Variants which is used to define the active variants.
		public string[] ActiveVariants
		{
			get { return m_ActiveVariants; }
			set { m_ActiveVariants = value; }
		}
	
		// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
		public AssetBundleManifest AssetBundleManifestObject
		{
			set {m_AssetBundleManifest = value; }
		}
	

		public void SetSourceAssetBundleURL(string absolutePath)
		{
			BaseDownloadingURL = absolutePath ;
		}
		
		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
		public LoadedAssetBundle GetLoadedAssetBundle (string assetBundleName, out string error)
		{
			if (m_DownloadingErrors.TryGetValue (assetBundleName, out error)) {
				return null;
			}
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null) {
				return null;
			}
			// No dependencies are recorded, only the bundle itself is required.
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue (assetBundleName, out dependencies)) {
				return bundle;
			}
			// Make sure all dependencies are loaded
			foreach(var dependency in dependencies)
			{
				if (m_DownloadingErrors.TryGetValue (assetBundleName, out error)) {
					return bundle;
				}
				// Wait all the dependent assetBundles being loaded.
				LoadedAssetBundle dependentBundle;
				m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null) {
					return null;
				}

			}
	
			return bundle;
		}
	
		public AssetBundleLoadManifestOperation InitializeAsync (string projectTag)
		{
			_projectTag = projectTag;
//			string name = ABUtility.GetPlatformName ();
//			if (_projectTag != null) {
//				name = _projectTag;
//			}
			return InitializeAsync(projectTag,projectTag);
		}

		private string GetAssetBundleFullPath(string assetBundle,bool isLoadSync) {
			
			string baseDir = Application.persistentDataPath;
			string midDir =  "/AssetBundles/" + ABUtility.GetPlatformName () + "/";
//			string file = assetBundle;
			string endDir = "";
			if (_projectTag != null) {
				endDir = _projectTag + "/";
//				if (assetBundle != _projectTag) {
//					file = assetBundle + "_project_" + _projectTag;
//				}
			}
				
			//先找可写路径
			if (File.Exists (baseDir + midDir + endDir + assetBundle)) {
				return baseDir + midDir + endDir + assetBundle;
			}
			//再找内存
			if (isLoadSync && Application.platform == RuntimePlatform.Android) {
				baseDir = Application.dataPath + "!assets"; 
			} else {
				baseDir = Application.streamingAssetsPath;
			}
			return baseDir + midDir + endDir + assetBundle;
		}



		public void InitializeSync(string projectTag) {
		
//			var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
//			DontDestroyOnLoad(go);

			_projectTag = projectTag;
	
			#if UNITY_EDITOR	
			if (SimulateAssetBundleInEditor)
			{
				return;
			}
			#endif
//			string name = ABUtility.GetPlatformName ();
//			if (_projectTag != null) {
//				name = _projectTag;
//			}
			AssetBundle ab = LoadAssetBundleSync(_projectTag, true);
			AssetBundleManifestObject = ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		}
	
		// Load AssetBundleManifest.
		public AssetBundleLoadManifestOperation InitializeAsync (string manifestAssetBundleName,string projectTag)
		{
	#if UNITY_EDITOR
			Debug.Log ("Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (SimulateAssetBundleInEditor){
				return null;
			}
	#endif
	
			LoadAssetBundle(manifestAssetBundleName, true);
			var operation = new AssetBundleLoadManifestOperation (manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest), this);
			m_InProgressOperations.Add (operation);
			return operation;
		}
		
		// Load AssetBundle and its dependencies.
		protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
		{
			Debug.Log ("Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (SimulateAssetBundleInEditor){
				return;
			}
	#endif
	
			if (!isLoadingAssetBundleManifest)
			{
				if (m_AssetBundleManifest == null)
				{
					Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
					return;
				}
			}
	
			// Check if the assetBundle has already been processed.
			bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);
	
			// Load dependencies.
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest) {
				LoadDependencies(assetBundleName);
			}
				
		}
		
		// Remaps the asset bundle name to the best fitting asset bundle variant.
		protected string RemapVariantName(string assetBundleName)
		{
			string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

			string[] split = assetBundleName.Split('.');

			int bestFit = int.MaxValue;
			int bestFitIndex = -1;
			// Loop all the assetBundles with variant to find the best fit variant assetBundle.
			for (int i = 0; i < bundlesWithVariant.Length; i++)
			{
				string[] curSplit = bundlesWithVariant[i].Split('.');
				if (curSplit[0] != split[0])
					continue;
				
				int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);
				
				// If there is no active variant found. We still want to use the first 
				if (found == -1){
					found = int.MaxValue-1;
				}

						
				if (found < bestFit)
				{
					bestFit = found;
					bestFitIndex = i;
				}
			}
			
			if (bestFit == int.MaxValue-1)
			{
				Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
			}
			
			if (bestFitIndex != -1)
			{
				return bundlesWithVariant[bestFitIndex];
			}
			else
			{
				return assetBundleName;
			}
		}
	

		protected AssetBundle LoadAssetBundleInternalSync(string assetBundleName, bool isLoadingAssetBundleManifest){
			LoadedAssetBundle bundle = null;
		
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return bundle.m_AssetBundle;
			}
			string url = GetAssetBundleFullPath (assetBundleName,true);
			AssetBundle ab = AssetBundle.LoadFromFile(url);
			m_LoadedAssetBundles.Add(assetBundleName,new LoadedAssetBundle(ab));
			return  ab;
		}
		// Where we actuall call WWW to download the assetBundle.
		protected bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest)
		{
			// Already loaded.
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return true;
			}
	
			// @TODO: Do we need to consider the referenced count of WWWs?
			// In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
			if (m_DownloadingWWWs.ContainsKey(assetBundleName))
			{
				return true;
			}
				
			AssetBundleCreateRequest download = null;

//			string url = m_BaseDownloadingURL + assetBundleName;

			string url = GetAssetBundleFullPath (assetBundleName,false);
		
			download = AssetBundle.LoadFromFileAsync (url);


			m_DownloadingWWWs.Add(assetBundleName, download);
	
			return false;
		}

		protected void LoadDependenciesSync(string assetBundleName){
			if (m_AssetBundleManifest == null)
			{
				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}

			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0) {
				return;
			}
				
			for (int i = 0; i < dependencies.Length; i++) {
				dependencies[i] = RemapVariantName (dependencies[i]);
			}				

			// Record and load all dependencies.
//			if (!m_Dependencies.ContainsKey (assetBundleName)) {
			m_Dependencies.Add (assetBundleName, dependencies);
			for (int i = 0; i < dependencies.Length; i++) {
				LoadAssetBundleInternalSync (dependencies [i], false);
			}
//			}
//			else {
//				Debug.LogWarning ("AssetBundle already Loaded:" + assetBundleName);
//			}
		}
	
		// Where we get all the dependencies and load them all.
		protected void LoadDependencies(string assetBundleName)
		{
			if (m_AssetBundleManifest == null)
			{
				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}
	
			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0) {
				return;
			}
				
				
			for (int i = 0; i < dependencies.Length; i++) {
				dependencies[i] = RemapVariantName (dependencies[i]);
			}

			// Record and load all dependencies.
			m_Dependencies.Add(assetBundleName, dependencies);
			for (int i = 0; i < dependencies.Length; i++) {
				LoadAssetBundleInternal(dependencies[i], false);
			}
				
		}
	
		public void UnloadAssetBundle(string assetBundleName,bool force=false)
		{
	        #if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				return;
			}
	        #endif
			UnloadAssetBundleInternal(assetBundleName,force);
			UnloadDependencies(assetBundleName,force);
		}
	
		protected void UnloadDependencies(string assetBundleName,bool force=false)
		{
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue (assetBundleName, out dependencies)) 
			{
				return;
			}
			foreach(var dependency in dependencies)
			{
				UnloadAssetBundleInternal(dependency,force);
			}
			m_Dependencies.Remove(assetBundleName);
		}
	
		protected void UnloadAssetBundleInternal(string assetBundleName,bool force)
		{
			string error;
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
			{
				return;
			}
	
			if (--bundle.m_ReferencedCount == 0)
			{
				bundle.m_AssetBundle.Unload(force);
				m_LoadedAssetBundles.Remove(assetBundleName);
				Debug.Log(assetBundleName + " has been unloaded successfully");
			}
		}
	
		public float GetDownLoadProgress(string assetBundleName){
		
			if (m_DownloadingWWWs.ContainsKey (assetBundleName)) {
				AssetBundleCreateRequest abRequesst;
				m_DownloadingWWWs.TryGetValue (assetBundleName,out abRequesst);
				if (abRequesst != null) {
					return abRequesst.progress;
				}
			}
			return 1;
		}
		void Update()
		{
			var keysToRemove = new List<string>();
			foreach (var keyValue in m_DownloadingWWWs)
			{
				AssetBundleCreateRequest download = keyValue.Value;
				if(download.isDone)
				{
					AssetBundle bundle = download.assetBundle;
					if (bundle == null)
					{
						m_DownloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
						keysToRemove.Add(keyValue.Key);
						continue;
					}
					m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle) );
					keysToRemove.Add(keyValue.Key);
				}
			}
			foreach( var key in keysToRemove)
			{
				AssetBundleCreateRequest download = m_DownloadingWWWs[key];
				m_DownloadingWWWs.Remove(key);
			}
			for (int i=0;i<m_InProgressOperations.Count;)
			{
				if (!m_InProgressOperations [i].Update ()) {
					m_InProgressOperations.RemoveAt (i);
				} else {
					i++;
				}
			}
		}
	
		public T LoadAsset<T>(string assetBundleName, string assetName, System.Type type) where T:UnityEngine.Object{
//			assetBundleName= ResetAssetBundleName(assetBundleName);
			#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor) {
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName);
				if (assetPaths.Length == 0) {
//					Debug.LogError ("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}
				var t = AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
				return t as T;
			} else
			#endif
			{
				LoadedAssetBundle bundle =null;
				m_LoadedAssetBundles.TryGetValue (assetBundleName,out bundle);
				foreach (var v in m_LoadedAssetBundles) {
					var xx = v.Key;
				}


				if (bundle != null && bundle.m_AssetBundle != null) 
				{
					return bundle.m_AssetBundle.LoadAsset<T> (assetName);
				} else {
//					Debug.LogError ("***make sure the asset bundle finish loaded first *********");
					return null;
				}
			}
		}
			
		public void ForceUnloadAll(){
			foreach(var keyValue in m_LoadedAssetBundles)
			{
				LoadedAssetBundle loadedAb = keyValue.Value;
				loadedAb.m_AssetBundle.Unload (true);
			}
			m_LoadedAssetBundles.Clear();
			m_DownloadingWWWs.Clear ();
			m_DownloadingErrors.Clear ();
			m_InProgressOperations.Clear ();
			m_Dependencies.Clear ();
		}


		public AssetBundle LoadAssetBundleSync(string assetBundleName,bool isLoadingAssetBundleManifest){
//			assetBundleName= ResetAssetBundleName(assetBundleName);
			Debug.Log("Loading "  + assetBundleName + " bundle");
			#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
				if (assetPaths.Length == 0)
				{
					Debug.LogError("There is no assetbundle with name "+assetBundleName);
					return null;
				}
				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
				return target as AssetBundle;
			}
			else
			#endif
			{
				Debug.Log("Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

				if (!isLoadingAssetBundleManifest)
				{
					if (m_AssetBundleManifest == null)
					{
						Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
						return null;
					}
				}
				AssetBundle ab= LoadAssetBundleInternalSync(assetBundleName, isLoadingAssetBundleManifest);
				if (!isLoadingAssetBundleManifest){
					LoadDependenciesSync(assetBundleName);
				}
					
				return ab;
			}
		}

		public bool IsEditorSimulate(){
			#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				return true;
			}
			#endif
			return false;
		}


		public AssetBundleLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
		{
			Debug.Log("Loading " + assetName + " from " + assetBundleName + " bundle");
//			assetBundleName= ResetAssetBundleName(assetBundleName);
			AssetBundleLoadAssetOperation operation = null;
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}
	
				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
				operation = new AssetBundleLoadAssetOperationSimulation (target);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName (assetBundleName);
				LoadAssetBundle (assetBundleName);
				if (assetName == null) {
					operation = new AssetBundleLoadOperationSelf (assetBundleName,this);
				} else {
					operation = new AssetBundleLoadAssetOperationFull (assetBundleName, assetName, type, this);
				}

	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	
		public AssetBundleLoadOperation LoadLevelAsync (string assetBundleName, string levelName, bool isAdditive)
		{
			Debug.Log("Loading " + levelName + " from " + assetBundleName + " bundle");
	
			AssetBundleLoadOperation operation = null;
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName(assetBundleName);
				LoadAssetBundle (assetBundleName);
				operation = new AssetBundleLoadLevelOperation (assetBundleName, levelName, isAdditive, this);
	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}

//		private string ResetAssetBundleName(string assetBundleName){
//			string ret = assetBundleName;
//			if (_projectTag != null && _projectTag != assetBundleName)
//			{
//				ret = ret + "_project_" + _projectTag;
//			}
//			return ret;
//		}
	} 
}