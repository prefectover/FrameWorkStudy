using UnityEngine;
using System.Collections;


namespace QFramework  {
	/// <summary>
	/// 场景定义 
	/// </summary>
	public interface IScene {

		/// <summary>
		/// 启动场景
		/// </summary>
		IEnumerator Launch();
	}

}
