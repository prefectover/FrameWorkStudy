using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using QFramework.AB;
using QFramework.UI;
namespace QFramework {

	public enum CanvasLevel
	{
		Top,
		Middle,
		Bottom,
		Root,
		MainCamera,
	}
		
	//// <summary>
	/// UGUI UI界面管理器
	/// </summary>
	public class QUGUIMgr : QMgrBehaviour{ 

		protected override void SetupMgrId ()
		{
			mMgrId = (ushort)QMgrID.UI;
		}

		protected override void SetupMgr ()
		{
			
		}
		public QUIBehaviour OpenUI<T>(CanvasLevel canvasLevel,string bundleName,object uiData = null) where T : QUIBehaviour
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
				//                DebugUtils.Log(strDlg + " UnLoad Success");
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
			
		public void InternalRemoveMenu(QUIBehaviour _handler)
		{
			System.Type type = _handler.GetType();
			string key = type.ToString();
			if (mAllUI.ContainsKey(key))
			{
				mAllUI.Remove(key);
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
			


		public static QUGUIMgr Instance {
			get {
				return QMonoSingletonComponent<QUGUIMgr>.Instance;
			}
		}

		public void OnDestroy()
		{
			QMonoSingletonComponent<QUGUIMgr>.Dispose ();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public static IEnumerator Init() {
			if (GameObject.Find ("QUGUIMgr")) {

			} else {
				GameObject.Instantiate (Resources.Load ("QUGUIMgr"));
			}

			var init =QUGUIMgr.Instance;


			yield return null;
		}

	

		[SerializeField]
		Dictionary<string,QUIBehaviour> mAllUI = new Dictionary<string, QUIBehaviour> ();

		[SerializeField] Transform mCanvasTopTrans;
		[SerializeField] Transform mCanvasMidTrans;
		[SerializeField] Transform mCanvasBottomTrans;
		[SerializeField] Transform mCanvasTrans;
		[SerializeField] Transform mCanvasGuideTrans;
		[SerializeField] Camera mUICamera;
			
		/// <summary>
		/// 增加UI层
		/// </summary>
		public QUIBehaviour CreateUI<T>  (CanvasLevel level,string bundleName,object uiData = null) where T : QUIBehaviour {

			string behaviourName = typeof(T).ToString();

			if (mAllUI.ContainsKey (behaviourName)) {

                Debug.LogWarning(behaviourName + ": already exist");

				mAllUI [behaviourName].transform.localPosition = Vector3.zero;
				mAllUI [behaviourName].transform.localEulerAngles = Vector3.zero;
				mAllUI [behaviourName].transform.localScale = Vector3.one;

				mAllUI [behaviourName].Enter (uiData);


			} else {
				GameObject prefab = QResMgr.Instance.LoadAsset<GameObject> (bundleName, behaviourName.ToLower ());

				GameObject mUIGo = Instantiate (prefab);
				switch (level) {
				case CanvasLevel.Top:
					mUIGo.transform.SetParent (mCanvasTopTrans);
					break;
				case CanvasLevel.Middle:
					mUIGo.transform.SetParent (mCanvasMidTrans);
					break;
				case CanvasLevel.Bottom:
					mUIGo.transform.SetParent (mCanvasBottomTrans);
					break;
				case CanvasLevel.Root:
					mUIGo.transform.SetParent (transform);
					break;
				case CanvasLevel.MainCamera:
					mUIGo.transform.SetParent (Camera.main.transform);
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
	}
}