using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class UIExample5AboutPage : QUIBehaviour,IMsgSender
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIExample5AboutPageComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnBack_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMgrID.UI,Example5UIMsg.SEND_MSG_TO_EXAMPLE_5_UI_CTRL,
				new object[] { Example5UIMsg.BTN_BACK_CLICK});	
		});
	}
	protected override void OnShow()
	{
		base.OnShow();
	}

	protected override void OnHide()
	{
		base.OnHide();
	}

	void ShowLog(string content)
	{
		Debug.Log("[ UIExample5AboutPage:]" + content);
	}

	UIExample5AboutPageComponents mUIComponents = null;
}