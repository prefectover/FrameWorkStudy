using UnityEngine;
using System.Collections;


/// <summary>
/// 消息体
/// </summary>
public class QMsg {

	// 表示 65535个消息 占两个字节
	public ushort msgId;

	public QMgrID GetMgrID()
	{
		int tmpId = msgId / QMsgSpan.Count;

		return (QMgrID)(tmpId * QMsgSpan.Count);
	}

	public QMsg() {}

	public QMsg(ushort msg)
	{
		msgId = msg;
	}
		
}

public class QSoundMsg : QMsg {
	public bool soundOn;

	public QSoundMsg(ushort msgId,bool soundOn) :base(msgId)
	{
		this.soundOn = soundOn;
	}
}
	

public class MsgTransform :QMsg
{
	public Transform value;

	public MsgTransform(ushort msgId,Transform tmpTrans) : base(msgId)
	{
		this.value = tmpTrans;
	}
}