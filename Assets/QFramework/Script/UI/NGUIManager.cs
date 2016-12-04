//#define NGUI

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if NGUI 
namespace QFramework.UI {
		
	//// <summary>
	/// UGUI UI界面管理器
	/// </summary>
	public class NGUIManager : QMonoSingleton<NGUIManager>{ 

		/// <summary>
		/// 初始化
		/// </summary>
		public static IEnumerator Init() {
			if (GameObject.Find ("NGUIManager")) {

			} else if (null == mInstance){
				GameObject.Instantiate (Resources.Load ("NGUIManager"));
			}

			NGUIManager.Instance ();

			yield return null;
		}
			
		void Awake()
		{
			mInstance = this;
			DontDestroyOnLoad (this);
		}


		[SerializeField]
		Dictionary<string,UILayer> mAllLayers = new Dictionary<string, UILayer> ();

		[SerializeField] Transform mCanvasTopTrans;
		[SerializeField] Transform mCanvasMidTrans;
		[SerializeField] Transform mCanvasBottomTrans;
		[SerializeField] Transform mCanvasTrans;
		[SerializeField] Transform mCanvasGuideTrans;
		[SerializeField] Camera mUICamera;
		[SerializeField] Transform mParentTrans;

		/// <summary>
		/// 增加UI层
		/// </summary>
		public UILayer AddLayer(string layerName,float nextZ,object uiData = null) {

			if (mAllLayers.ContainsKey (layerName)) {

                Debug.LogWarning(layerName + ": already exist");

				mAllLayers [layerName].transform.localPosition = Vector3.zero;
				mAllLayers [layerName].transform.localEulerAngles = Vector3.zero;
				mAllLayers [layerName].transform.localScale = Vector3.one;

				mAllLayers [layerName].Enter (uiData);


			} else {
				GameObject prefab = ResMgr.Instance ().LoadUIPrefabSync (layerName);

				GameObject uiLayer = NGUITools.AddChild (mParentTrans.gameObject,prefab, nextZ);

				uiLayer.transform.localPosition = Vector3.forward * nextZ;
				uiLayer.transform.localEulerAngles = Vector3.zero;
				uiLayer.transform.localScale = Vector3.one;

				uiLayer.gameObject.name = layerName;

				mAllLayers.Add (layerName, uiLayer.GetComponent<UILayer> ());
				uiLayer.GetComponent<UILayer> ().Enter (uiData);
			}


			return mAllLayers [layerName];
		}

		/// <summary>
		/// 删除掉层
		/// </summary>
		public void DeleteLayer(string layerName)
		{
			if (mAllLayers.ContainsKey (layerName)) 
			{
				Debug.LogWarning ("Remove Layer:" + layerName);
				mAllLayers [layerName].Exit ();
				NGUITools.Destroy (mAllLayers [layerName].gameObject);
				mAllLayers.Remove (layerName);
			}
		}


		/// <summary>
		/// 显示UI层
		/// </summary>
		/// <param name="layerName">Layer name.</param>
		public void ShowLayer(string layerName)
		{
			if (mAllLayers.ContainsKey(layerName))
			{
				mAllLayers[layerName].Show ();
			}
		}


		/// <summary>
		/// 隐藏UI层
		/// </summary>
		/// <param name="layerName">Layer name.</param>
		public void HideLayer(string layerName)
		{
			if (mAllLayers.ContainsKey (layerName)) 
			{
				mAllLayers [layerName].Hide ();
			}
		}


		/// <summary>
		/// 删除所有UI层
		/// </summary>
		public void DeleteAllLayer()
		{
			foreach (var layer in mAllLayers) 
			{
				layer.Value.Exit ();
				GameObject.Destroy (layer.Value);
			}

			mAllLayers.Clear ();
		}


		/// <summary>
		/// 获取所有UI层
		public T GetLayer<T>(string layerName)
		{
			if (mAllLayers.ContainsKey (layerName)) 
			{
				return mAllLayers [layerName].GetComponent<T> ();
			}
			return default(T);
		}

        /// <summary>
        /// 获取UI相机
        /// </summary>
        /// <returns></returns>
        public Camera GetUICamera() 
        {
            return mUICamera;
        }
	}
}
#endif