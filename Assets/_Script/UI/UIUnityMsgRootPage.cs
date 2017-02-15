using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
using QFramework.Event;
using QFramework.UIFramework.Example2;

public class UIUnityMsgRootPage : QUIBehaviour,IMsgReceiver
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIUnityMsgRootPageComponents;
		//please add init code here
		this.RegisterGlobalMsg ("ReceiveMsgFromChild", delegate(object[] paramList) {
			mUIComponents.ReceivedMsg_Text.text = paramList[0] as string;	
		});

	}
	public override void ProcessMsg (QMsg msg)
	{
		switch (msg.msgId) {
		case (ushort)UIEvent.Hello:
			mUIComponents.ReceivedMsg_Text.text = ((QStrMsg)msg).strMsg;
			break;
		}
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnOpenChildPage_Button.onClick.AddListener (delegate {
			QUIManager.Instance.OpenUI<UIUnityMsgChildPanel> (QUILevel.PopUI,null,null);	
		});

		mMsgIds = new ushort[] {
			(ushort)UIEvent.Hello
		};
		this.RegisterSelf (this, null);
	}
	protected override void OnShow()
	{
		base.OnShow();
	}


	void ReceivedMsgFromChildPage(string msgContent) {
		mUIComponents.ReceivedMsg_Text.text = msgContent;
	}

	protected override void OnHide()
	{
		base.OnHide();
	}

	void ShowLog(string content)
	{
		Debug.Log("[ UIUnityMsgRootPage:]" + content);
	}

	UIUnityMsgRootPageComponents mUIComponents = null;
}