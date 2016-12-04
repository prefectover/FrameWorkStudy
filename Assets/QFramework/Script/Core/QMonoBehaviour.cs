using UnityEngine;
using System.Collections;

namespace QFramework {


	/// <summary>
	/// 可以根据Futile的QNode来写,添加消息机制,替代SendMessage
	/// </summary>
	public abstract class QMonoBehaviour : MonoBehaviour {

		public abstract void ProcessMsg (QMsg msg);



		protected abstract void SetupMgr ();
		private QMgrBehaviour mPrivateMgr = null;
		protected QMgrBehaviour mCurMgr {
			get {
				if (mPrivateMgr == null ) {
					SetupMgr ();
				}
				if (mPrivateMgr == null) {
					Debug.LogError ("没有设置Mgr");
				}

				return mPrivateMgr;
			}

			set {
				mPrivateMgr = value;
			}
		}
		/// <summary>
		/// 短变量,指针而已
		/// </summary>
		/// <value>The trans.</value>


		private Transform mCachedTrans;
		private GameObject mCachedGameObj;

		public Transform trans {
			get {
				if (mCachedTrans == null) {
					mCachedTrans = transform;
				}
				return mCachedTrans;
			}
		}

		public GameObject gameObj {
			get {
				if (mCachedGameObj == null) {
					mCachedGameObj = gameObject;
				}
				return mCachedGameObj;
			}
		}

		public QMonoBehaviour parent;	 				// 父节点 

		public void setPosition(Vector3 vec3)
		{
			transform.localPosition = vec3;
		}

		public void setPosition(float x,float y)
		{
			transform.localPosition = new Vector3 (x, y, transform.localPosition.z);
		}

		public void addChild(QMonoBehaviour child)
		{
			child.parent = this;
			child.trans.parent = this.trans;
		}

		public void addTo(QMonoBehaviour parent)
		{
			this.transform.parent = parent.trans;
			this.parent = parent;
		}


		public void setName(string name)
		{
			gameObject.name = name;
		}

		public string getName()
		{
			return gameObject.name;
		}


		public void show()
		{
			gameObj.SetActive (true);
			onShow ();
		}


		public void hide()
		{
			gameObj.SetActive (false);
			onHide ();
		}


		/// <summary>
		/// 调用show时候触发
		/// </summary>
		protected virtual void onShow()
		{

		}

		/// <summary>
		/// 调用hide时候触发
		/// </summary>
		protected virtual void onHide()
		{

		}

		public void RegisterSelf(QMonoBehaviour mono,ushort[] msgs)
		{
			mCurMgr.RegisterMsg(mono,msgIds);
		}

		public void UnRegisterSelf(QMonoBehaviour mono,ushort[] msg)
		{
			mCurMgr.UnRegisterMsg(mono,msgIds);
		}

		public void SendMsg(QMsg msg)
		{
			mCurMgr.SendMsg(msg);
		}

		public ushort[] msgIds;

		void OnDestory()
		{
			if (msgIds != null)
			{
				UnRegisterSelf(this,msgIds);
			}
		}

	}
}
