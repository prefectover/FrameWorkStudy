using UnityEngine;
using System.Collections;
using QFramework;

namespace QFramework {
	public enum QAppMode {
		Developing,	// 开发版本,为了快速快发,而写的测试入口。
		QA,			// 发布版本,跑整个游戏
		Release		// 发布版本,跑整个游戏
	}

	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// </summary>
	public class Framework : QMgrBehaviour
	{

		protected override void SetupMgrId ()
		{
			mMgrId = 0;
		}

		protected override void SetupMgr ()
		{

		}

		public static IEnumerator Init()
		{
			yield return QMsgCenter.Instance.Init ();

			//-----------------初始化管理器-----------------------
			var a = QTimerMgr.Instance;
			var c = QResourceManager.Instance;
			var f = GameManager.Instance;
		}

		/// <summary>
		/// 组合的方式实现单例的模板
		/// </summary>
		/// <value>The instance.</value>
		public static Framework Instance {
			get {
				return QMonoSingletonComponent<Framework>.Instance;
			}
		}

		public QAppMode mode = QAppMode.Developing;

		private Framework() {}

		void Awake()
		{
			// 确保不被销毁
			DontDestroyOnLoad(gameObject);

			// 进入欢迎界面
			Application.targetFrameRate = 60;
		}

		IEnumerator Start()
		{
			var log = QLog.Instance;
			var console = QConsole.Instance;

			switch (Framework.Instance.mode) {
			case QAppMode.Developing:
				{
//					yield return GetComponent<ITestEntry> ().Launch ();
				}
				break;
			case QAppMode.QA:
				{
				}
				break;

			case QAppMode.Release:
					
				yield return GameManager.Instance.Launch ();

				break;
			}
		}

		#region 全局生命周期回调
		public delegate void LifeCircleCallback();

		public LifeCircleCallback onUpdate = delegate{};
		public LifeCircleCallback onFixedUpdate = delegate{};
		public LifeCircleCallback onLatedUpdate = delegate{};
		public LifeCircleCallback onGUI = delegate {};
		public LifeCircleCallback onDestroy = delegate {};
		public LifeCircleCallback onApplicationQuit = delegate {};

		void Update()
		{
			if (this.onUpdate != null)
				this.onUpdate();
		}

		void FixedUpdate()
		{
			if (this.onFixedUpdate != null)
				this.onFixedUpdate ();

		}

		void LatedUpdate()
		{
			if (this.onLatedUpdate != null)
				this.onLatedUpdate ();
		}

		void OnGUI()
		{
			if (this.onGUI != null)
				this.onGUI();
		}

		protected  void OnDestroy() 
		{
			QMonoSingletonComponent<Framework>.Dispose ();

			if (this.onDestroy != null)
				this.onDestroy();
		}

		void OnApplicationQuit()
		{
			if (this.onApplicationQuit != null)
				this.onApplicationQuit();
		}
		#endregion
	}
}
