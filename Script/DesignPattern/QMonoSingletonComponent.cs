using UnityEngine;

/// <summary>
/// 需要使用组合方式实现Unity生命周期的单例模式
/// </summary>
namespace QFramework {

	public abstract class QMonoSingletonComponent<T> where T : MonoBehaviour
	{
		protected static T mInstance = null;

		public static T Instance
		{
			get {
				if (mInstance == null) {
					mInstance = MonoBehaviour.FindObjectOfType<T> ();

					if (MonoBehaviour.FindObjectsOfType<T> ().Length > 1) {
						Debug.LogError ("More than 1!");

						return mInstance;
					}

					if (mInstance == null) {
						string instanceName = typeof(T).Name;

						Debug.Log ("Instance Name: " + instanceName); 

						GameObject instanceGO = GameObject.Find (instanceName);

						if (instanceGO == null)
							instanceGO = new GameObject (instanceName);
						mInstance = instanceGO.AddComponent<T> ();

						Debug.Log ("Add New Singleton " + mInstance.name + " in Game!");

					} else {
						Debug.LogWarning ("Already exist: " + mInstance.name);
					}
				}

				return mInstance;

			}
		}
			
		public static void Dispose()
		{
			mInstance = null;
		}
	}

}