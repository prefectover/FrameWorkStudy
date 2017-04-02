using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using QFramework.AB;
using System;

/// <summary>
/// TODO:
/// 1.Lite 删掉
/// 2.Page之间继承
/// 3.发地址
/// 4.
/// </summary>
namespace QFramework {

	public enum QUILevel
	{
		Bg,          			//背景层UI
		Common,              	//普通层UI
		PopUI,                 	//弹出层UI
		Const,               	//持续存在层UI
		Toast,               	//对话框层UI
		Forward,              	//最高UI层用来放置UI特效和模型
	}

	//// <summary>
	/// UGUI UI界面管理器
	/// </summary>
	public class QUIManager : QMgrBehaviour{ 

		[SerializeField]
		Dictionary<string,QUIBehaviour> mAllUI = new Dictionary<string, QUIBehaviour> ();

		[SerializeField] Transform mBgTrans;
		[SerializeField] Transform mCommonTrans;
		[SerializeField] Transform mPopUITrans;
		[SerializeField] Transform mConstTrans;
		[SerializeField] Transform mToastTrans;
		[SerializeField] Transform mForwardTrans;
		[SerializeField] Camera mUICamera;
		[SerializeField] Canvas mCanvas;
		[SerializeField] CanvasScaler mCanvasScaler;

		static GameObject mGo;
		public static QUIManager Instance {
			get {
				if (mGo) {
				} else {
					mGo = GameObject.Find ("PTUIManager");
					if (mGo) {
					} else {
						mGo = GameObject.Instantiate (Resources.Load ("PTUIManager")) as GameObject;
					}
					mGo.name = "PTUIManager";
				}

				return PTMonoSingletonComponent<QUIManager>.Instance;
			}
		}

		void Awake() {
			DontDestroyOnLoad (this);
			// DefaultResolution is 1024 768
			SetResolution (768, 1024);
			SetMatchOnWidthOrHeight (0.0f);
		}

		public void SetResolution(int width,int height) {
			mCanvasScaler.referenceResolution = new Vector2 (width, height);
		}

		public void SetMatchOnWidthOrHeight(float heightPercent) {
			mCanvasScaler.matchWidthOrHeight = heightPercent;
		}

		/// <summary>
		/// Create&ShowUI
		/// </summary>
		public T OpenUI<T>(QUILevel canvasLevel,string bundleName,object uiData = null) where T : QUIBehaviour
		{
			string behaviourName = typeof(T).ToString();

			QUIBehaviour ui;
			if (!mAllUI.TryGetValue(behaviourName, out ui))
			{
				ui = CreateUI<T>(canvasLevel,bundleName, uiData);
			}
			ui.Show();
			return ui as T;
		}

		public Transform Get<T>(string strUIName)
		{
			string strDlg = typeof(T).ToString();
			if (mAllUI.ContainsKey(strDlg))
			{
				return mAllUI[strDlg].Get(strUIName);
			}
			else
			{
				Debug.LogError(string.Format("panel={0},ui={1} not exist!", strDlg, strUIName));
			}
			return null;
		}

		/// <summary>
		/// 增加UI层
		/// </summary>
		public T CreateUI<T>(QUILevel level,string bundleName,object initData = null) where T : QUIBehaviour
		{
			string behaviourName = typeof(T).ToString();

			QUIBehaviour ui;
			if (mAllUI.TryGetValue(behaviourName, out ui))
			{
				Debug.LogWarning(behaviourName + ": already exist");
				//直接返回，不要再调一次Init(),Init()应该只能调用一次
				return ui as T;
			}

			GameObject	prefab = QResourceManager.Instance.LoadAsset<GameObject> (bundleName,behaviourName);

			GameObject mUIGo = Instantiate (prefab);
			switch (level) {
				case QUILevel.Bg:
					mUIGo.transform.SetParent (mBgTrans);
					break;
				case QUILevel.Common:
					mUIGo.transform.SetParent (mCommonTrans);
					break;
				case QUILevel.PopUI:
					mUIGo.transform.SetParent (mPopUITrans);
					break;
				case QUILevel.Const:
					mUIGo.transform.SetParent (mConstTrans);
					break;
				case QUILevel.Toast:
					mUIGo.transform.SetParent (mToastTrans);
					break;
				case QUILevel.Forward:
					mUIGo.transform.SetParent (mForwardTrans);
					break;
			}

			var uiGoRectTrans = mUIGo.GetComponent<RectTransform> ();
			uiGoRectTrans.offsetMin = Vector2.zero;
			uiGoRectTrans.offsetMax = Vector2.zero;
			uiGoRectTrans.anchoredPosition3D = Vector3.one;
			uiGoRectTrans.anchorMin = Vector2.zero;
			uiGoRectTrans.anchorMax = Vector2.one;

			mUIGo.transform.localScale = Vector3.one;


			mUIGo.gameObject.name = behaviourName;

			Debug.Log(behaviourName + " Load Success");

			ui = mUIGo.AddComponent<T>();
			mAllUI.Add(behaviourName, ui);
			ui.Init(initData);

			return ui as T;
		}

		/// <summary>
		/// 显示UI层
		/// </summary>
		/// <param name="layerName">Layer name.</param>
		public void ShowUI<T>()
		{
			string behaviourName = typeof(T).ToString();

			if (mAllUI.ContainsKey(behaviourName))
			{
				mAllUI[behaviourName].Show ();
			}
		}

		/// <summary>
		/// 隐藏UI层
		/// </summary>
		/// <param name="layerName">Layer name.</param>
		public void HideUI<T>()
		{
			string behaviourName = typeof(T).ToString();

			if (mAllUI.ContainsKey (behaviourName)) 
			{
				mAllUI [behaviourName].Hide ();
			}
		}

		/// <summary>
		/// 删除所有UI层
		/// </summary>
		public void CloseAllUI()
		{
			foreach (var layer in mAllUI) 
			{
				layer.Value.Close ();
				GameObject.Destroy (layer.Value);
			}

			mAllUI.Clear ();
		}

		/// <summary>
		/// 删除掉层
		/// </summary>
		public void CloseUI<T>()
		{
			string behaviourName = typeof(T).ToString();

			if (mAllUI.ContainsKey (behaviourName)) 
			{
				mAllUI [behaviourName].Close ();
				GameObject.Destroy (mAllUI [behaviourName].gameObject);
				mAllUI.Remove (behaviourName);
			}
		}

		/// <summary>
		/// 获取UIBehaviour
		public T GetUI<T>()
		{
			string behaviourName = typeof(T).ToString();

			if (mAllUI.ContainsKey (behaviourName)) 
			{
				return mAllUI [behaviourName].GetComponent<T> ();
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

		protected override void SetupMgrId ()
		{
			mMgrId = (ushort)QMgrID.UI;
		}

		protected override void SetupMgr ()
		{

		}

		public void OnDestroy()
		{
			QMonoSingletonComponent<QUIManager>.Dispose ();
		}
	}
}