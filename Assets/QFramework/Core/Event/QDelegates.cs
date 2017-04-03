using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace QFramework {
	/// <summary>
	/// 返回空类型的回调定义
	/// </summary>
	public class QVoidDelegate{

		public delegate void WithVoid();

		public delegate void WithGo(GameObject go);

		public delegate void WithParams(params object[] paramList);

		public delegate void WithEvent(BaseEventData data);

		public delegate void WithObj(Object obj);

		public delegate void WithBool(bool value);
	}


	public delegate void DTableOnParse(byte[] data);
	public delegate void Run();
	public delegate void Run<T>(T v);
	public delegate void Run<T, K>(T v1, K v2);

}
