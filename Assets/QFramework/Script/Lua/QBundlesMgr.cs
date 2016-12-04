using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace QFramework
{
	public class QBundlesMgr : MonoBehaviour {

		public Image image;
		private float percent = 0;

		public string[] bundles = {};

		void Awake(){
#if UNITY_EDITOR
			QLuaComponent.isFirstLaunch = false;
#endif
			if (LuaMain.nextClearBundles) {
				LuaMain.Dispose ();
				LuaMain.nextClearBundles = false;
			}
			if (LuaMain.loadBundles != null&&LuaMain.loadBundles.Length>0) {
				bundles = LuaMain.loadBundles;
			}
			for (int i = 0; i < bundles.Length; i++) {
				AddBundle (bundles [i]);
			}
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			QBundleLoader[]bundles = gameObject.GetComponents<QBundleLoader> ();
			int count = 0;
			for (int i = 0; i < bundles.Length; i++) {
//				bool load = bundles [i].IsLoaded;
				if(bundles [i].IsLoaded)count++;
			}

			percent += Time.deltaTime;
			if (percent > 0.9f&&bundles.Length>count) {
				percent = 0.9f;
			}

			if (image != null) {
				image.fillAmount = percent;
			}
			if(percent>=1.0f)
				OnLoaded ();
		}
		public void AddBundle(string name){
			QBundleLoader[] bundles = gameObject.GetComponents<QBundleLoader> ();
			for (int i = 0; i < bundles.Length; i++) {
				if (bundles[i].bundleName.Equals(name)) {
					return;
				}
			}

			QBundleLoader bundle = gameObject.AddComponent<QBundleLoader> ();
			bundle.bundleName = name;
		}

//		public void OnGUI(){
//			BundleLoader[]bundles = gameObject.GetComponents<BundleLoader> ();
//			int count = 0;
//			for (int i = 0; i < bundles.Length; i++) {
//				//				bool load = bundles [i].IsLoaded;
//				if(bundles [i].IsLoaded)count++;
//			}
//			GUI.TextField (new Rect (0, 0, 200, 100), "已加载" + count);
//		}



		private bool isLoaded = false;
		public void OnLoaded(){
//			if (!isLoaded) {
//				isLoaded = true;
//				StartCoroutine (LoadScene());
//			}

			string sceneName = LuaMain.NextSceneName;
			gameObject.SetActive (false);
			UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
		}

		public IEnumerator LoadScene(){
			string sceneName = LuaMain.NextSceneName;
			string frontName = sceneName.Split ('_') [0];

			string url =  "file://" + Application.streamingAssetsPath+"/SceneAssetBundles/"+frontName + "/Scene.unity3d";  
			WWW www = WWW.LoadFromCacheOrDownload (url,1);  
			yield return www;  
			if (www.error != null) {  
				Debug.Log ("下载失败");  
			} else {  
				AssetBundle bundle = www.assetBundle;  
				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);  
				print ("跳转场景");  
				// AssetBundle.Unload(false)，释放AssetBundle文件内存镜像，不销毁Load创建的Assets对象  
				// AssetBundle.Unload(true)，释放AssetBundle文件内存镜像同时销毁所有已经Load的Assets内存镜像  
				bundle.Unload (false);  
			}  

			// 中断正在加载过程中的WWW  
			www.Dispose ();  
		}
			
	}
}
