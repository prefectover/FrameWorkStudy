using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {

	public class QMsgNode
	{
		// 当前的Behaviour
		public QMonoBehaviour behaviour;

		// 下一个节点
		public QMsgNode next;

		public QMsgNode(QMonoBehaviour behaviour)
		{
			this.behaviour = behaviour;
			this.next = null;
		}
	}

	/// <summary>
	/// manager基类
	/// </summary>
	public abstract class QMgrBehaviour : QMonoBehaviour 
	{
		// 存储注册消息 key value
		public Dictionary<ushort,QMsgNode> msgDic = new Dictionary<ushort,QMsgNode>();

		protected ushort mMgrId = 0;

		protected abstract void SetupMgrId ();

		protected override void SetupMgr ()
		{
			mCurMgr = this;
		}


		protected QMgrBehaviour() {
			SetupMgrId ();
		}

		// mono:要注册的脚本   
		// msgs:每个脚本可以注册多个脚本
		public void RegisterMsg(QMonoBehaviour behaviour,ushort[] msgs)
		{
			for (int i = 0;i < msgs.Length;i++)
			{
				QMsgNode msgNode = new QMsgNode(behaviour);

				RegisterMsg(msgs[i],msgNode);
			}
		}

		// 根据: msgid
		// node链表
		public void RegisterMsg(ushort msgId,QMsgNode node)
		{
			// 数据链路里 没有这个消息id
			if(!msgDic.ContainsKey(msgId))
			{
				msgDic.Add(msgId,node);
			}
			else 
			{
				QMsgNode tmp = msgDic[msgId];

				// 找到最后一个车厢
				while(tmp.next != null)
				{
					tmp = tmp.next;
				}

				// 直接挂上
				tmp.next = node;
			}
		}

		// params 可变数组 参数
		// 去掉一个脚本的若干的消息
		public void UnRegisterMsg(QMonoBehaviour monoBase,params ushort[] msgs)
		{
			for (int i = 0;i < msgs.Length;i++)
			{
				UnRegistMsg(msgs[i],monoBase);
			}
		}

		// 释放 中间,尾部。
		public void UnRegistMsg(ushort msgId,QMonoBehaviour behaviour)
		{
			if (!msgDic.ContainsKey(msgId))
			{
				Debug.LogWarning("not contain id ==" + msgId);
				return;
			}
			else 
			{
				QMsgNode msgNode = msgDic[msgId];

				if (msgNode.behaviour == behaviour) // 去掉头部 包含两种情况
				{
					QMsgNode header = msgNode;

					// 已经存在这个消息
					// 头部
					if (header.next != null)
					{
						msgDic [msgId] = msgNode.next; // 直接指向下一个
						header.next = null;

						header.behaviour = msgNode.next.behaviour;
						header.next = msgNode.next.next;
					}
					else // 后面没有节点的情况 
					{
						header.next = null;
						msgDic.Remove(msgId);
					}
				}
				else // 去掉尾部 和中间的节点 
				{
					while(msgNode.next != null && msgNode.next.behaviour != behaviour) // 下一个不是我要找的 node 就一直遍历
					{
						msgNode = msgNode.next;
					} // 表示已经找到了 该节点

					// 没有引用 会自动释放
					if (msgNode.next.next != null) // 去掉中间的
					{
						QMsgNode curNode = msgNode.next; // 保存一下

						msgNode.next = curNode.next;
						//					tmp.next = tmp.next.next;
						curNode.next = null; // 把相关联的指针释放
					}
					else // 去掉尾部的
					{
						// tmp表示要找的节点的上一个节点
						msgNode.next = null;
					}
				}
			}
		}

		public void SendMsg(QMsg msg)
		{
			if ((ushort)msg.GetMgrID() == mMgrId)
			{
				ProcessMsg(msg);
			}
			else 
			{
				QMsgCenter.Instance.SendToMsg(msg);
			}
		}

		// 来了消息以后,通知整个消息链
		public override void ProcessMsg(QMsg msg)
		{
			if (!msgDic.ContainsKey(msg.msgId))
			{
				Debug.LogError("msg not found:" + msg.msgId);
				return;
			}
			else 
			{
				QMsgNode msgNode = msgDic[msg.msgId];

				// 进行广播
				do 
				{	
					msgNode.behaviour.ProcessMsg(msg);

					msgNode = msgNode.next;
				} 
				while (msgNode != null);
			}
		}

	}

}