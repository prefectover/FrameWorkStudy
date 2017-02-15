using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
using QFramework.Event;

public class UIUnityMsgChildPanel : QUIBehaviour,IMsgSender
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIUnityMsgChildPanelComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnClose_Button.onClick.AddListener (delegate {
			QUIManager.Instance.HideUI<UIUnityMsgChildPanel>();
		});

		mUIComponents.BtnSendMsg_Button.onClick.AddListener (delegate {
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();
			for (int i = 0;i < 10000;i++) {
				this.SendGlobalMsg("ReceiveMsgFromChild","hello");
			}
			watch.Stop();
			Debug.Log(watch.ElapsedMilliseconds);
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
		Debug.Log("[ UIUnityMsgChildPanel:]" + content);
	}

	UIUnityMsgChildPanelComponents mUIComponents = null;
}