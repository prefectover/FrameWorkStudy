using UnityEngine;
using System.Collections;

namespace QFramework.PRIVATE {

	/// <summary>
	/// 框架层的消息
	/// </summary>
	public enum QFrameworkMsg {
		Began = 60000,
		DISPATCH_MESSAGE,  // 派发消息
		UPDATE_MESSAGE,	   // 更新消息
		UPDATE_EXTRACT,    // 更新解包
		UPDATE_DOWNLOAD,   // 更新下载
		UPDATE_PROGRESS,   // 更新进度
		End,
	}

	public enum QSoundEvent {
		Began = QMgrID.Sound,
		SoundSwitch,
		End,
	}
}
