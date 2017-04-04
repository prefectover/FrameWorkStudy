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


	void Awake()
	{
		DontDestroyOnLoad (this);
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
	private void ForwardMsg(QMsg msg)
	{
		QMgrID tmpId = msg.GetMgrID();

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
			case  QMgrID.Network:
				break;
			case  QMgrID.NPCManager:
				break;
			case  QMgrID.UI:
				QUIManager.Instance.SendMsg (msg);
				break;
			case QMgrID.Data:
				ToDoList.ToDoListManager.Instance.SendMsg (msg);
				break;
			default:
				break;
		}
	}
}

/// <summary>
/// 消息的扩展类
/// </summary>
public static class QMsgCenterExtention  {
	public static void SendMsgToCenter(this System.Object self,QMsg tmpMsg) {
		QMsgCenter.Instance.SendToMsg (tmpMsg);
	}
}