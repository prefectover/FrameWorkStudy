using UnityEngine;
using System.Collections;

namespace QFramework
{
	public abstract class AssetBundleLoadOperation : IEnumerator
	{
		public object Current
		{
			get
			{
				return null;
			}
		}
		public bool MoveNext()
		{
			return !IsDone();
		}
		
		public void Reset()
		{
		}
		
		abstract public bool Update ();
		
		abstract public bool IsDone ();
	}
	
	#if UNITY_EDITOR
	public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
	{	
		AsyncOperation m_Operation = null;
	
	
		public AssetBundleLoadLevelSimulationOperation (string assetBundleName, string levelName, bool isAdditive)
		{
			string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
			if (levelPaths.Length == 0)
			{	
				Debug.LogError("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
				return;
			}
			
			if (isAdditive) {
				m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode (levelPaths [0]);
			} else {
				m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
			}
		}
		
		public override bool Update ()
		{
			return false;
		}
		
		public override bool IsDone ()
		{		
			return m_Operation == null || m_Operation.isDone;
		}
	}
	
	#endif
	public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
	{
		protected string 				m_AssetBundleName;
		protected string 				m_LevelName;
		protected bool 					m_IsAdditive;
		protected string 				m_DownloadingError;
		protected AsyncOperation		m_Request;
		public AssetBundleManager _manager;
	
		public AssetBundleLoadLevelOperation (string assetbundleName, string levelName, bool isAdditive, AssetBundleManager manager)
		{
			m_AssetBundleName = assetbundleName;
			m_LevelName = levelName;
			m_IsAdditive = isAdditive;
			_manager = manager;
		}
	
		public override bool Update ()
		{
			if (m_Request != null) {
				return false;
			}
			LoadedAssetBundle bundle = _manager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
			if (bundle != null) {
				
				if (m_IsAdditive) {
					m_Request = Application.LoadLevelAdditiveAsync (m_LevelName);
				} else {
					m_Request = Application.LoadLevelAsync (m_LevelName);
				}
				return false;
			} 
			else {
				
				return true;
			}
				
		}
		
		public override bool IsDone ()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null)
			{
				Debug.LogError(m_DownloadingError);
				return true;
			}
			
			return m_Request != null && m_Request.isDone;
		}
	}
	
	public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
	{
		public abstract T GetAsset<T>() where T : UnityEngine.Object;
	}
	
	public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
	{
		Object	m_SimulatedObject;
		
		public AssetBundleLoadAssetOperationSimulation (Object simulatedObject)
		{
			m_SimulatedObject = simulatedObject;
		}
		
		public override T GetAsset<T>()
		{
			return m_SimulatedObject as T;
		}
		
		public override bool Update ()
		{
			return false;
		}
		
		public override bool IsDone ()
		{
			return true;
		}
	}

	public class AssetBundleLoadOperationSelf:AssetBundleLoadAssetOperation{
		protected string 				m_AssetBundleName;
		protected string 				m_DownloadingError;
		private bool loadFinish = false;
		private AssetBundleManager _manager = null;
		public AssetBundleLoadOperationSelf (string bundleName, AssetBundleManager manager)
		{
			m_AssetBundleName = bundleName;
			_manager = manager;
	
		}

		public override T GetAsset<T>()
		{
			return bundle.m_AssetBundle as T;
		}

		LoadedAssetBundle bundle ;
		// Returns true if more Update calls are required.
		public override bool Update ()
		{
			if (loadFinish) {
				return false;
			}
				

			bundle = _manager.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
			if (bundle != null)
			{
				///@TODO: When asset bundle download fails this throws an exception...
				loadFinish = true;
				return false;
			}
			else
			{
				return true;
			}
		}

		public override bool IsDone ()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_DownloadingError != null)
			{
				Debug.LogError(m_DownloadingError);
				return true;
			}

			return loadFinish ;
		}

	}
	public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
	{
		protected string 				m_AssetBundleName;
		protected string 				m_AssetName;
		protected string 				m_DownloadingError;
		protected System.Type 			m_Type;
		protected AssetBundleRequest	m_Request = null;

		public AssetBundleManager _mananger = null;
	
		public AssetBundleLoadAssetOperationFull (string bundleName, string assetName, System.Type type, AssetBundleManager manager)
		{
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
			_mananger = manager;
		}
		
		public override T GetAsset<T>()
		{
			if (m_Request != null && m_Request.isDone) {
				
				return m_Request.asset as T;

			} else {
				
				return null;
			}
				
		}
		
		// Returns true if more Update calls are required.
		public override bool Update ()
		{
			if (m_Request != null)
				return false;
	
			LoadedAssetBundle bundle = _mananger.GetLoadedAssetBundle (m_AssetBundleName, out m_DownloadingError);
			if (bundle != null)
			{
				///@TODO: When asset bundle download fails this throws an exception...
				m_Request = bundle.m_AssetBundle.LoadAssetAsync (m_AssetName, m_Type);
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public override bool IsDone ()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null && m_DownloadingError != null)
			{
				Debug.LogError(m_DownloadingError);
				return true;
			}
	
			return m_Request != null && m_Request.isDone;
		}
	}
	
	public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
	{
		public AssetBundleLoadManifestOperation (string bundleName, string assetName, System.Type type, AssetBundleManager manager)
			: base(bundleName, assetName, type, manager)
		{
		}
	
		public override bool Update ()
		{
			base.Update();

			if (m_Request != null && m_Request.isDone) {
				_mananger.AssetBundleManifestObject = GetAsset<AssetBundleManifest> ();
				return false;
			} else {
				return true;
			}
				
		}
	}
	
	
}
