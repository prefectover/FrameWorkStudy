using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class UIExample5MainPage : QUIBehaviour,IMsgSender
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIExample5MainPageComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnStart_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMsgChannel.UI,Example5UIMsg.SEND_MSG_TO_EXAMPLE_5_UI_CTRL,
				new object[]{ Example5UIMsg.BTN_START_CLICK });
		});

		mUIComponents.BtnAbout_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMsgChannel.UI,Example5UIMsg.SEND_MSG_TO_EXAMPLE_5_UI_CTRL,
				new object[]{ Example5UIMsg.BTN_ABOUT_CLICK });
		});

		mUIComponents.BtnQuitGame_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMsgChannel.UI,Example5UIMsg.SEND_MSG_TO_EXAMPLE_5_UI_CTRL,
				new object[]{ Example5UIMsg.BTN_QUIT_CLICK });
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
		Debug.Log("[ UIExample5MainPage:]" + content);
	}

	UIExample5MainPageComponents mUIComponents = null;
}