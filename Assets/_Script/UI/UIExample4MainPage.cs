using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class UIExample4MainPage : QUIBehaviour,IMsgSender
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIExample4MainPageComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnStart_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMgrID.UI,Example4UIMsg.SEND_MSG_TO_EXAMPLE_4_UI_CTRL,
				new object[]{ Example4UIMsg.MAIN_PAGE_BTN_START_CLICK });
		});

		mUIComponents.BtnAbout_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMgrID.UI,Example4UIMsg.SEND_MSG_TO_EXAMPLE_4_UI_CTRL,
				new object[]{ Example4UIMsg.MAIN_PAGE_BTN_ABOUT_CLICK });
		});

		mUIComponents.BtnQuitGame_Button.onClick.AddListener (delegate {
			this.SendMsgByChannel(QMgrID.UI,Example4UIMsg.SEND_MSG_TO_EXAMPLE_4_UI_CTRL,
				new object[]{ Example4UIMsg.MAIN_PAGE_BTN_QUIT_CLICK });
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
		Debug.Log("[ UIExample4MainPage:]" + content);
	}

	UIExample4MainPageComponents mUIComponents = null;
}