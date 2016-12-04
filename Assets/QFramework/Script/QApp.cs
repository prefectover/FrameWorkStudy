using UnityEngine;
using System.Collections;
using QFramework;
using QFramework.UI;

namespace QFramework {
	public enum QAppMode {
		Developing,
		QA,
		Release
	}
		
	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// </summary>
	public class QApp : MonoBehaviour
	{
		/// <summary>
		/// 组合的方式实现单例的模板
		/// </summary>
		/// <value>The instance.</value>
		public static QApp Instance {
			get {
				return QMonoSingletonComponent<QApp>.Instance;
			}
		}

		public QAppMode mode = QAppMode.Developing;

		private QApp() {}

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
			yield return QFramework.Instance.Init ();

			// 配置文件加载 类似PlayerPrefs
			QSetting.Load();

			switch (QApp.Instance.mode) {
			case QAppMode.Developing:
				{
					yield return GetComponent<ITestEntry> ().Launch ();
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
			QMonoSingletonComponent<QApp>.Dispose ();

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
