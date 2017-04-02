using UnityEngine;
using System.Collections;

namespace QFramework {
	/// <summary>
	/// 测试入口
	/// </summary>
	public interface ITestEntry {

		/// <summary>
		/// 启动
		/// </summary>
		IEnumerator Launch();
	}

}
