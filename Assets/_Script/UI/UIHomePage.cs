using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class UIHomePage : QUIBehaviour
{
	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIHomePageComponents;
		//please add init code here
	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnQuitGame_Button.onClick.AddListener (delegate {
			Debug.Log("BtnQuitGameClick");	
		});

		mUIComponents.BtnAbout_Button.onClick.AddListener (delegate {
			Debug.Log("BtnAboutClick");
		});

		mUIComponents.BtnStart_Button.onClick.AddListener (delegate {
			Debug.Log("BtnStartClick");
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
		Debug.Log("[ UIHomePage:]" + content);
	}

	UIHomePageComponents mUIComponents = null;
}