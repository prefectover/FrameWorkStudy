using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
using QFramework.Event;

public class UIMainPage : QUIBehaviour
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIMainPageComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnFirst_Button.onClick.AddListener (delegate {
			QUIManager.Instance.OpenUI<UIFirstPage> (QUILevel.Common, null, "这是第一关");
			QUIManager.Instance.DeleteUI<UIMainPage> ();
		});

		mUIComponents.BtnSecond_Button.onClick.AddListener(delegate {
			QUIManager.Instance.OpenUI<UIFirstPage> (QUILevel.Common, null, "这是第二关");
			QUIManager.Instance.DeleteUI<UIMainPage> ();
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
		Debug.Log("[ UIMainPage:]" + content);
	}

	UIMainPageComponents mUIComponents = null;
}