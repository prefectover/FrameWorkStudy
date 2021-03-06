﻿using UnityEngine;
using System.Collections;

public class QMsgSpan {
	public const int Count = 3000;
}

public enum QMgrID
{
	Framework = 0,
	Game = 0,
	UI = QMsgSpan.Count, 			// 3000
	Sound = QMsgSpan.Count * 2,		// 6000
	NPCManager = QMsgSpan.Count * 3,
	CharactorManager = QMsgSpan.Count * 4,
	AB = QMsgSpan.Count * 5,
	Network = QMsgSpan.Count * 6,
	Data = QMsgSpan.Count * 7,
	Global = QMsgSpan.Count * 8
}
