using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using QFramework;
using UnityEngine.UI;

namespace QFramework.UI {
	public abstract class QUIBehaviour : QMonoBehaviour {

		protected override void SetupMgr ()
		{
			mCurMgr = QUGUIMgr.Instance;
		}

		void OnDestroy()
		{
			DestroyUI();
			if (mUIComponentsDic != null)
			{
				mUIComponentsDic.Clear();
			}
			if (mIComponents != null)
			{
				mIComponents.Clear();
			}

			if (mMsgIds != null)
			{
				UnRegisterSelf(this,mMsgIds);
			}
			Debug.Log(name + " unLoad Success");
		}

		public void Init(object uiData = null)
		{
			InnerInit(uiData);
			RegisterUIEvent();
		}

		public Transform Get(string behaivourName)
		{
			if (mUIComponentsDic.ContainsKey(behaivourName))
			{
				return mUIComponentsDic[behaivourName];
			}
			return null;
		}

		public void Close()
		{
			SetVisible(false);
			OnClose();
			if (mUnloadAll == false)
			{
				//                PTResourceManager.UnloadAssetBundle(this.name.ToLower(), false);
			}
		}

		public void SetVisible(bool visible)
		{
			this.gameObject.SetActive(visible);
			if(visible)
			{
				OnShow();
			}
		}

		void InnerInit(object uiData = null)
		{
			FindAllCanHandleWidget(this.transform);
			mIComponents = QUIFactory.Instance.CreateUIComponents(this.name);
			mIComponents.InitUIComponents();
			InitUI(uiData);
			SetVisible(true);
		}

		protected virtual void OnAwake() { }
		protected virtual void OnStart() { }
		protected virtual void InitUI(object uiData = null) { }
		protected virtual void RegisterUIEvent() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnFixedUpdate() { }
		protected virtual void OnClose() { }
		protected virtual void DestroyUI() { }

		protected void SetUIBehavior(IUIComponents uiChild)
		{
			mIComponents = uiChild;
			mIComponents.InitUIComponents();
		}
			

		void FindAllCanHandleWidget(Transform trans)
		{
			for (int i = 0; i < trans.childCount; i++)
			{
				Transform childTrans = trans.GetChild(i);
				QUIMark uiMark = childTrans.GetComponent<QUIMark>();
				if (null != uiMark)
				{
					if (mUIComponentsDic.ContainsKey(childTrans.name))
					{
						Debug.LogError("Repeat Id: " + childTrans.name);
					}
					else
					{
						mUIComponentsDic.Add(childTrans.name, childTrans);
					}
				}
				FindAllCanHandleWidget(childTrans);
			}
		}

		protected virtual bool mUnloadAll
		{
			get { return false; }
		}
		protected ushort[] mMsgIds = null;
		protected IUIComponents mIComponents = null;
		private Dictionary<string, Transform> mUIComponentsDic = new Dictionary<string, Transform>();

	

		public void AddButtonListener(UnityAction action)
		{
			if (null != action)
			{
				Button btn = transform.GetComponent<Button> ();

				btn.onClick.AddListener (action);
			}
		}


		public void RemoveButtonListener(UnityAction action)
		{
			if (null != action) 
			{
				Button btn = transform.GetComponent<Button> ();

				btn.onClick.RemoveListener (action);
			}
		}


		public void AddSliderListener(UnityAction<float> action)
		{
			if (null != action) 
			{
				Slider slider = transform.GetComponent<Slider> ();

				slider.onValueChanged.AddListener (action);
			}
		}

		public void RemoveSliderListener(UnityAction<float> action)
		{
			if (null != action) 
			{
				Slider slider = transform.GetComponent<Slider> ();

				slider.onValueChanged.RemoveListener (action);
			}
		}

		public void AddInputListener(UnityAction<string> action)
		{
			if (null != action) 
			{
				InputField btn = transform.GetComponent<InputField> ();

				btn.onValueChanged.AddListener (action);
			}
		}

		public override void ProcessMsg (QMsg msg)
		{
			throw new System.NotImplementedException ();
		}


		public void RegisterSelf(QMonoBehaviour behaviour,ushort[] msgs)
		{
			QUGUIMgr.Instance.RegisterMsg(behaviour,mMsgIds);
		}

		public void UnRegisterSelf(QMonoBehaviour behaviour,ushort[] msg)
		{
			QUGUIMgr.Instance.UnRegisterMsg(behaviour,mMsgIds);
		}

		public void SendMsg(QMsg msg)
		{
			QUGUIMgr.Instance.SendMsg(msg);
		}



		#region 原来自己的框架
		public void Enter(object uiData)
		{
			gameObject.SetActive (false);
			OnEnter (uiData);
		}

		/// <summary>
		/// 资源加载之后用
		/// </summary>
		protected virtual void OnEnter(object uiData)
		{
			Debug.LogWarning ("On Enter:" + name);
		}


		public void Show()
		{
			OnShow ();
		}

		/// <summary>
		/// 显示时候用,或者,Active为True
		/// </summary>
		protected virtual void OnShow()
		{
			gameObject.SetActive (true);
			Debug.LogWarning ("On Show:" + name);
		}


		public void Hide()
		{
			OnHide ();
		}

		/// <summary>
		/// 隐藏时候调用,即将删除 或者,Active为False
		/// </summary>
		protected virtual void OnHide()
		{
			gameObject.SetActive (false);
			Debug.LogWarning ("On Hide:" + name);
		}

		public void Exit()
		{
			OnExit ();
		}

		/// <summary>
		/// 删除时候调用
		/// </summary>
		protected virtual void OnExit()
		{
			Debug.LogWarning ("On Exit:" + name);
		}
		#endregion

	}

}