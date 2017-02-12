using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
using QFramework.Event;

public class UIFirstPage : QUIBehaviour
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIFirstPageComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
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
		Debug.Log("[ UIFirstPage:]" + content);
	}

	UIFirstPageComponents mUIComponents = null;
}