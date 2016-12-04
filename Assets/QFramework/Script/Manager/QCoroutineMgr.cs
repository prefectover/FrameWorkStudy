using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 协程管理
/// 分两种
/// </summary>
namespace QFramework {
	
	public class QCoroutineMgr  {

		public static QCoroutineMgr Instance {
			get {
				return QSingletonComponent<QCoroutineMgr>.Instance;
			}
		}

		public static void Dispose()
		{
			QSingletonComponent<QCoroutineMgr>.Dispose ();
		}


		protected QCoroutineMgr() {}



		public Coroutine StartCoroutine(IEnumerator routine) {
			return QApp.Instance.StartCoroutine (routine);
		}

		Dictionary<string,List<Coroutine>> mCoroutineDic = new Dictionary<string, List<Coroutine>>();
	


		public virtual void Init()
		{

		}
			

		public void StopCoroutine(IEnumerator enumerator)
		{
			QApp.Instance.StopCoroutine (enumerator);
		}
		/// <summary>
		/// Executes the coroutine.
		/// </summary>
		public void ExecuteCoroutine(IEnumerator enumarator,string name)
		{
			var coroutine = QApp.Instance.StartCoroutine (enumarator);

			if (mCoroutineDic.ContainsKey (name)) {
				mCoroutineDic [name].Add (coroutine);
			} else {
				mCoroutineDic.Add (name, new List<Coroutine> ());
			}

			Log ("ExecuteCoroutine", name);
		}

		/// <summary>
		/// Stops the name of the all coroutine with.
		/// </summary>
		public void StopAllCoroutineWithName(string name)
		{
			if (mCoroutineDic.ContainsKey (name)) {

				var coList = mCoroutineDic [name];

				for (int i = 0; i < coList.Count; i++) {

					if (null != coList [i]) {
						QApp.Instance.StopCoroutine (coList [i]);
						coList [i] = null;
					}
				}

				coList.Clear ();
			}

			Log ("StopAllCoroutineWithName", name);
		}

		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		void OnDestroy()
		{
			QApp.Instance.StopAllCoroutines ();
		}

		void Log(string actionName,string content)
		{
			Debug.LogWarning ("CoroutineMgr @@@@ " + actionName + ":" + content);
		}
	}



}
