using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LuaInterface;


namespace QFramework
{
	public class QBundleLoader : MonoBehaviour {

		public delegate void LoadFinishHandle(String lunBundleModule);   
		public event LoadFinishHandle loadFinishEvent;   

		public string bundleName=null;

		private bool _isLoaded = false;


		public bool IsLoaded{
			get{ 
				return _isLoaded;
			}
		}
		// Use this for initialization
		void Start () {
//			LoadBundle (bundleName);
//			LoadBundle (bundleName);
		}

		void Awake(){
			
		}
		// Update is called once per frame
		void Update () {
			if (!isstart) {
				DebugStr = "Update";
				LoadBundle (bundleName);
			}
		}

		private bool isstart = false;

		public void LoadBundle(String lunBundleModule){

			isstart = true;

			if (lunBundleModule != null)
				bundleName = lunBundleModule;
			if(bundleName!=null)
				StartCoroutine(LoadBundles());
		}




		int bundleCount = int.MaxValue;

		IEnumerator CoLoadBundle(string name, string path)
		{
			DebugStr = "CoLoadBundle";
			using (WWW www = new WWW(path))
			{
				if (www == null)
				{
					Debugger.LogError(name + " bundle not exists");
					yield break;
				}

				yield return www;

				if (www.error != null)
				{
					Debugger.LogError(string.Format("Read {0} failed: {1}", path, www.error));
					yield break;
				}

				--bundleCount;
				LuaFileUtils.Instance.AddSearchBundle(name, www.assetBundle);
				www.Dispose();
			}                     
		}

		IEnumerator LoadFinished()
		{
			while (bundleCount > 0)
			{
				yield return null;
			}

			OnBundleLoad();
		}
		public IEnumerator LoadBundles()
		{
			DebugStr = bundleName;
			string streamingPath = Application.streamingAssetsPath.Replace ('\\', '/');

//			#if UNITY_5
			#if UNITY_ANDROID && !UNITY_EDITOR
			string main = streamingPath + "/LuaAssetBundles/"+bundleName+"/"+bundleName;
			#else
			string main = "file://" + streamingPath + "/LuaAssetBundles/"+bundleName+"/"+bundleName;
			#endif
			DebugStr = main;
			WWW mwww = WWW.LoadFromCacheOrDownload (main, 0);
			yield return mwww;
			DebugStr = "";
			//			WWW www = new WWW(main);
			//			yield return www;

			//			AssetBundle ab =  AssetBundle.LoadFromFile("file://"+streamingPath + "/AssetBundles/AssetBundles");

			AssetBundle ab = mwww.assetBundle;
			AssetBundleManifest manifest = (AssetBundleManifest)ab.LoadAsset ("AssetBundleManifest");
			List<string> list = new List<string> (manifest.GetAllAssetBundles ());        
//			#else
//			//此处应该配表获取
//			List<string> list = new List<string>() { "lua.unity3d", "lua_cjson.unity3d", "lua_system.unity3d", "lua_unityengine.unity3d", "lua_protobuf.unity3d", "lua_misc.unity3d", "lua_socket.unity3d", "lua_system_reflection.unity3d" };
//			#endif
			bundleCount = list.Count;

			for (int i = 0; i < list.Count; i++) {
				string str = list [i];

				#if UNITY_ANDROID && !UNITY_EDITOR
				string path = streamingPath + "/LuaAssetBundles/"+bundleName+"/" + str;
				#else
				string path = "file://" + streamingPath + "/LuaAssetBundles/"+bundleName+"/" + str;
				#endif
				string name = Path.GetFileNameWithoutExtension (str);
				StartCoroutine (CoLoadBundle (name, path));            
			}
			ab.Unload (false);
			yield return StartCoroutine (LoadFinished ());
		}


		void OnBundleLoad()
		{                
//			LuaMain.assetBundleLoaded = true;
//			loadFinishEvent(bundleName);
			_isLoaded = true;
		}	

		private string DebugStr = "";
//		void OnGUI(){
//			GUI.TextArea(new Rect(0,100,200,100),"bundleCount:"+bundleCount+"\n"+DebugStr);
//		}
	}
}
