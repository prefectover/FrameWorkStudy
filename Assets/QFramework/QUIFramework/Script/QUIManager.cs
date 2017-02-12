using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using QFramework.AB;
using QFramework.UI;

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
					mGo = GameObject.Find ("QUIManager");
					if (mGo) {

					} else {
						mGo = GameObject.Instantiate (Resources.Load ("QUIManager")) as GameObject;
					}
					mGo.name = "QUIManager";
				}

				return QMonoSingletonComponent<QUIManager>.Instance;
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

		public QUIBehaviour OpenUI<T>(QUILevel canvasLevel,string bundleName,object uiData = null) where T : QUIBehaviour
		{
			string behaviourName = typeof(T).ToString();
			if (!mAllUI.ContainsKey(behaviourName))
			{
				return CreateUI<T>(canvasLevel,bundleName,uiData);
			}
			mAllUI [behaviourName].Show ();
			return mAllUI[behaviourName];
		}

		public void CloseUI<T>() where T : QUIBehaviour
		{
			string strDlg = typeof(T).ToString();
			if (mAllUI.ContainsKey(strDlg))
			{
				mAllUI[strDlg].Close();
			}
		}

		public void CloseAllUI()
		{
			List<QUIBehaviour> listHandler = new List<QUIBehaviour>();
			foreach (string key in mAllUI.Keys)
			{
				listHandler.Add(mAllUI[key]);
			}

			for (int i = 0; i < listHandler.Count; i++)
			{
				listHandler[i].Close();
			}
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

		public void Close()
		{
			mAllUI.Clear();
		}

		/// <summary>
		/// 增加UI层
		/// </summary>
		public QUIBehaviour CreateUI<T>  (QUILevel level,string bundleName,object uiData = null) where T : QUIBehaviour {

			string behaviourName = typeof(T).ToString();

			if (mAllUI.ContainsKey (behaviourName)) {

				Debug.LogWarning(behaviourName + ": already exist");

				mAllUI [behaviourName].transform.localPosition = Vector3.zero;
				mAllUI [behaviourName].transform.localEulerAngles = Vector3.zero;
				mAllUI [behaviourName].transform.localScale = Vector3.one;

				mAllUI [behaviourName].Enter (uiData);

			} else {
				GameObject prefab = Resources.Load<GameObject> (behaviourName);

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

				mUIGo.transform.localPosition = Vector3.zero;
				mUIGo.transform.localEulerAngles = Vector3.zero;
				mUIGo.transform.localScale = Vector3.one;

				mUIGo.gameObject.name = behaviourName;

				Debug.Log(behaviourName + " Load Success");

				T t = mUIGo.AddComponent<T>();
				mAllUI.Add (behaviourName, t);
				t.Init (uiData);
			}

			return mAllUI [behaviourName];
		}

		/// <summary>
		/// 删除掉层
		/// </summary>
		public void DeleteUI<T>()
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
		public void DeleteAllUI()
		{
			foreach (var layer in mAllUI) 
			{
				layer.Value.Exit ();
				GameObject.Destroy (layer.Value);
			}

			mAllUI.Clear ();
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