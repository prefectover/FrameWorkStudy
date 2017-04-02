using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace QFramework.AB
{	
	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class QLoadedAB
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;
		
		public QLoadedAB(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}
	
	// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
	public class QABMgr : MonoBehaviour
	{

		void Awake()
		{
			transform.SetParent (Framework.Instance.transform);
		}

		public enum LogMode { All, JustErrors };
		public enum LogType { Info, Warning, Error };
	
		static LogMode m_LogMode = LogMode.All;
		static string m_BaseDownloadingURL = "";
		static string[] m_ActiveVariants =  {  };
		static AssetBundleManifest m_AssetBundleManifest = null;
	#if UNITY_EDITOR	
		static int mDevMode = -1;
		const string kDevMode = "DevModeAB";
	#endif
	
		static Dictionary<string, QLoadedAB> m_LoadedAssetBundles = new Dictionary<string, QLoadedAB> ();
		static Dictionary<string, AssetBundleCreateRequest> m_DownloadingWWWs = new Dictionary<string, AssetBundleCreateRequest> ();
		static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();
		static List<ABLoadOperation> m_InProgressOperations = new List<ABLoadOperation> ();
		static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();
	
		public static LogMode logMode
		{
			get { return m_LogMode; }
			set { m_LogMode = value; }
		}
	
		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
		private static string BaseDownloadingURL
		{
			get { return m_BaseDownloadingURL; }
			set { m_BaseDownloadingURL = value; }
		}
	
		// Variants which is used to define the active variants.
		public static string[] ActiveVariants
		{
			get { return m_ActiveVariants; }
			set { m_ActiveVariants = value; }
		}
	
		// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
		public static AssetBundleManifest ABManifestObject
		{
			set {m_AssetBundleManifest = value; }
		}
	
		private static void Log(LogType logType, string text)
		{
			if (logType == LogType.Error)
				Debug.LogError("[QABManager] " + text);
			else if (m_LogMode == LogMode.All)
				Debug.Log("[QABManager] " + text);
		}
	
	#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool DevABModeInEditor 
		{
			get
			{
				if (mDevMode == -1)
					mDevMode = EditorPrefs.GetBool(kDevMode, true) ? 1 : 0;
				
				return mDevMode != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != mDevMode)
				{
					mDevMode = newValue;
					EditorPrefs.SetBool(kDevMode, value);
				}
			}
		}
		#endif
	
		
		public static void SetSourceABURL(string absolutePath)
		{
			BaseDownloadingURL = absolutePath ;
		}
		
		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
		static public QLoadedAB GetLoadedAssetBundle (string assetBundleName, out string error)
		{
			if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
				return null;
		
			QLoadedAB bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null)
				return null;
			
			// No dependencies are recorded, only the bundle itself is required.
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return bundle;
			
			// Make sure all dependencies are loaded
			foreach(var dependency in dependencies)
			{
				if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
					return bundle;
	
				// Wait all the dependent assetBundles being loaded.
				QLoadedAB dependentBundle;
				m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null)
					return null;
			}
	
			return bundle;
		}
	
		static public ABLoadManifestOperation Initialize ()
		{
			return Initialize(QPlatform.GetPlatformName());
		}

		static public void InitializeSync(){
		
			var go = new GameObject("QABMgr", typeof(QABMgr));
			DontDestroyOnLoad(go);

			#if UNITY_EDITOR	
			if (DevABModeInEditor)
			{
				return;
			}
			#endif
			AssetBundle ab =LoadABSync(QPlatform.GetPlatformName(), true);
			Debug.Log ("***************>>>>"+ab);
			ABManifestObject = ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
			Debug.Log ("***************>>>>vvvv:"+m_AssetBundleManifest);
		}
	
		// Load AssetBundleManifest.
		static public ABLoadManifestOperation Initialize (string manifestAssetBundleName)
		{
	#if UNITY_EDITOR
			Log (LogType.Info, "Simulation Mode: " + (DevABModeInEditor ? "Enabled" : "Disabled"));
	#endif
	
			var go = new GameObject("QABMgr", typeof(QABMgr));
			DontDestroyOnLoad(go);
		
	#if UNITY_EDITOR	
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (DevABModeInEditor)
				return null;
	#endif
	
			LoadAssetBundle(manifestAssetBundleName, true);
			var operation = new ABLoadManifestOperation (manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
			m_InProgressOperations.Add (operation);
			return operation;
		}
		
		// Load AssetBundle and its dependencies.
		static protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
		{
			Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (DevABModeInEditor){
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
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
				LoadDependencies(assetBundleName);
		}
		
		// Remaps the asset bundle name to the best fitting asset bundle variant.
		static protected string RemapVariantName(string assetBundleName)
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
				if (found == -1)
					found = int.MaxValue-1;
						
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
	
		static protected AssetBundle LoadAssetBundleInternalSync(string assetBundleName, bool isLoadingAssetBundleManifest){
			QLoadedAB bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return bundle.m_AssetBundle;
			}
			string url = m_BaseDownloadingURL + assetBundleName;
			AssetBundle ab = AssetBundle.LoadFromFile(url);
			m_LoadedAssetBundles.Add(assetBundleName,new QLoadedAB(ab));
			return  ab;
		}
		// Where we actuall call WWW to download the assetBundle.
		static protected bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest)
		{
			// Already loaded.
			QLoadedAB bundle = null;
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
			string url = m_BaseDownloadingURL + assetBundleName;
		
			download = AssetBundle.LoadFromFileAsync (url);

			m_DownloadingWWWs.Add(assetBundleName, download);
	
			return false;
		}

		static protected void LoadDependenciesSync(string assetBundleName){
			if (m_AssetBundleManifest == null)
			{
				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}

			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;

			for (int i=0;i<dependencies.Length;i++)
				dependencies[i] = RemapVariantName (dependencies[i]);

			// Record and load all dependencies.
			m_Dependencies.Add(assetBundleName, dependencies);
			for (int i = 0; i < dependencies.Length; i++) {
				LoadAssetBundleInternalSync(dependencies[i], false);
			}
				
		}
	
		// Where we get all the dependencies and load them all.
		static protected void LoadDependencies(string assetBundleName)
		{
			if (m_AssetBundleManifest == null)
			{
				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}
	
			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;
				
			for (int i=0;i<dependencies.Length;i++)
				dependencies[i] = RemapVariantName (dependencies[i]);
				
			// Record and load all dependencies.
			m_Dependencies.Add(assetBundleName, dependencies);
			for (int i=0;i<dependencies.Length;i++)
				LoadAssetBundleInternal(dependencies[i], false);
		}
	
		static public void UnloadAssetBundle(string assetBundleName,bool force=false)
		{
	        #if UNITY_EDITOR
			if (DevABModeInEditor)
			{
				return;
			}
	        #endif
			UnloadAssetBundleInternal(assetBundleName,force);
			UnloadDependencies(assetBundleName,force);
		}
	
		static protected void UnloadDependencies(string assetBundleName,bool force=false)
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
	
		static protected void UnloadAssetBundleInternal(string assetBundleName,bool force)
		{
			string error;
			QLoadedAB bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
			{
				return;
			}
	
			if (--bundle.m_ReferencedCount == 0)
			{
				bundle.m_AssetBundle.Unload(force);
				m_LoadedAssetBundles.Remove(assetBundleName);
				Log(LogType.Info, assetBundleName + " has been unloaded successfully");
			}
		}
	
		public static float GetDownLoadProgress(string assetBundleName){
		
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
					m_LoadedAssetBundles.Add(keyValue.Key, new QLoadedAB(download.assetBundle) );
					keysToRemove.Add(keyValue.Key);
				}
			}
			foreach( var key in keysToRemove)
			{
				AssetBundleCreateRequest download = m_DownloadingWWWs[key];
				m_DownloadingWWWs.Remove(key);
			}
	
			// Update all in progress operations
			for (int i=0;i<m_InProgressOperations.Count;)
			{
				if (!m_InProgressOperations[i].Update())
				{
					m_InProgressOperations.RemoveAt(i);
				}
				else
					i++;
			}
		}
	
		static public T LoadAsset<T>(string assetBundleName, string assetName, System.Type type) where T:UnityEngine.Object{
			#if UNITY_EDITOR
			if (DevABModeInEditor) {
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName);
				if (assetPaths.Length == 0) {
					Debug.LogError ("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}
				var t = AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
				return t as T;
			} else
			#endif
			{
				QLoadedAB bundle =null;
				Debug.Log (m_LoadedAssetBundles.Count+" ********** loadedassetbundles count******** " + assetBundleName);
				m_LoadedAssetBundles.TryGetValue (assetBundleName,out bundle);
				if (bundle.m_AssetBundle != null) {
					return bundle.m_AssetBundle.LoadAsset<T> (assetName);
				} else {
				
					Debug.LogError ("***make sure the asset bundle finish loaded first *********");
					return null;
				}
			}
		}
			
		static public void ForceUnloadAll(){
			foreach(var keyValue in m_LoadedAssetBundles){
				QLoadedAB loadedAb = keyValue.Value;
				loadedAb.m_AssetBundle.Unload (true);
			}
			m_LoadedAssetBundles.Clear();
			m_DownloadingWWWs.Clear ();
			m_DownloadingErrors.Clear ();
			m_InProgressOperations.Clear ();
			m_Dependencies.Clear ();
		}


	

		static public AssetBundle LoadABSync(string assetBundleName,bool isLoadingAssetBundleManifest){
			Log(LogType.Info, "Loading "  + assetBundleName + " bundle");

		
			#if UNITY_EDITOR
			if (DevABModeInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
//				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.LogError("There is no assetbundle with name "+assetBundleName);
					return null;
				}
//
//				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
				return target as AssetBundle;
			}
			else
			#endif
			{
				Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

				if (!isLoadingAssetBundleManifest)
				{
					if (m_AssetBundleManifest == null)
					{
						Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
						return null;
					}
				}

				// Check if the assetBundle has already been processed.
				AssetBundle ab= LoadAssetBundleInternalSync(assetBundleName, isLoadingAssetBundleManifest);

				// Load dependencies.
				if (!isLoadingAssetBundleManifest)
					LoadDependenciesSync(assetBundleName);
				return ab;
			}
		}

		static public bool IsEditorDevMode(){

			#if UNITY_EDITOR
			if (DevABModeInEditor)
			{
				return true;
			}
			#endif
		
			return false;
		}



		// Load asset from the given assetBundle.
		static public ABLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
		{
			Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");
	
			ABLoadAssetOperation operation = null;
	#if UNITY_EDITOR
			if (DevABModeInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}
	
				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
				operation = new ABLoadAssetOperationSimulation (target);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName (assetBundleName);
				LoadAssetBundle (assetBundleName);
				if (assetName == null) {
					operation = new ABLoadOperationSelf (assetBundleName);
				} else {
					operation = new ABLoadAssetOperationFull (assetBundleName, assetName, type);
				}

	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	
		// Load level from the given assetBundle.
		static public ABLoadOperation LoadLevelAsync (string assetBundleName, string levelName, bool isAdditive)
		{
			Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");
	
			ABLoadOperation operation = null;
	#if UNITY_EDITOR
			if (DevABModeInEditor)
			{
				operation = new ABLoadLevelDevModeOperation(assetBundleName, levelName, isAdditive);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName(assetBundleName);
				LoadAssetBundle (assetBundleName);
				operation = new ABLoadLevelOperation (assetBundleName, levelName, isAdditive);
	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	} 
}