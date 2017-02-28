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
			mCurMgr = QUIManager.Instance;
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
			Debug.Log(name + " remove Success");
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

		protected virtual void InitUI(object uiData = null) { }
		protected virtual void RegisterUIEvent() { }
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
		protected IUIComponents mIComponents = null;
		private Dictionary<string, Transform> mUIComponentsDic = new Dictionary<string, Transform>();

		public override void ProcessMsg (QMsg msg)
		{
			throw new System.NotImplementedException ();
		}


		public void RegisterSelf(QMonoBehaviour behaviour,ushort[] msgs = null)
		{
			if (null != msgs) {
				mMsgIds = msgs;
				QUIManager.Instance.RegisterMsg (behaviour, msgs);
			} else {
				QUIManager.Instance.RegisterMsg (behaviour, mMsgIds);
			}
		}

		public void UnRegisterSelf(QMonoBehaviour behaviour)
		{
			QUIManager.Instance.UnRegisterMsg(behaviour,mMsgIds);
		}

		public void SendMsg(QMsg msg)
		{
			QUIManager.Instance.SendMsg(msg);
		}

		#region 原来自己的框架
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
		#endregion
	}
}