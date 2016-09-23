using UnityEngine;
using System.Collections;

/// <summary>
/// 命令模式
/// </summary>
namespace QFramework {
	public interface ICmd {
		void Execute();
	}
}