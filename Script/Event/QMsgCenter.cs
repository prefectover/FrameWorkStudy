using UnityEngine;
using System.Collections;
using QFramework;

public class QMsgCenter : MonoBehaviour 
{
	public static QMsgCenter Instance {
		get {
			return QMonoSingletonComponent<QMsgCenter>.Instance;
		}
	}

	public void OnDestroy()
	{
		QMonoSingletonComponent<QMsgCenter>.Dispose ();
	}

	public IEnumerator Init()
	{
		Debug.Log ("QMsgCenter Init");
		yield return null;
	}

	public void SendToMsg(QMsg tmpMsg)
	{
		ForwardMsg(tmpMsg);
	}

	/// <summary>
	/// 转发消息
	/// </summary>
	private void ForwardMsg(QMsg tempMsg)
	{
		QMgrID tmpId = tempMsg.GetMgrID();

		switch (tmpId)
		{
		case QMgrID.AB:
			break;
		case QMgrID.Sound:
			break;
		case  QMgrID.CharactorManager:
			break;
		case  QMgrID.Game:
			break;
		case  QMgrID.NetManager:
			break;
		case  QMgrID.NPCManager:
			break;
		case  QMgrID.UI:
			break;
		default:
			break;
		}
	}
}
