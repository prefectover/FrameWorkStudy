using UnityEngine;
using System.Collections;
using QFramework.UI;
using System.Collections;

namespace QFrameworkLua {
	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// </summary>
	public class QLuaApp : MonoBehaviour {
				
		/// <summary>
		/// 组合的方式实现单例的模板
		/// </summary>
		/// <value>The instance.</value>
		public static QLuaApp Instance {
			get {
				return QFramework.QMonoSingletonComponent<QLuaApp>.Instance;
			}
		}
						
		private QLuaApp() {}

		public string luaFileName = "main.lua";

		void Awake()
		{
			// 进入欢迎界面
			Application.targetFrameRate = 60;
		}

		IEnumerator Start()
		{
			yield return QFrameworkLua.Instance.Init ();
		}

		#region 当前模块的生命周期回调
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

		void OnDestroy() 
		{
			if (this.onDestroy != null)
				this.onDestroy();
			QFramework.QMonoSingletonComponent<QLuaApp>.Dispose ();
		}

		void OnApplicationQuit()
		{	
			
			if (this.onApplicationQuit != null)
				this.onApplicationQuit();
		}
		#endregion
	}
}
