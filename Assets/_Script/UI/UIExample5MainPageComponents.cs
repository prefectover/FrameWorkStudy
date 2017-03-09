using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
public class UIExample5MainPageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnQuitGame_Button = QUIManager.Instance.Get<UIExample5MainPage>("BtnQuitGame").GetComponent<Button>();
		BtnStart_Button = QUIManager.Instance.Get<UIExample5MainPage>("BtnStart").GetComponent<Button>();
		BtnAbout_Button = QUIManager.Instance.Get<UIExample5MainPage>("BtnAbout").GetComponent<Button>();
	}

	public void Clear()
	{
		BtnQuitGame_Button = null;
		BtnStart_Button = null;
		BtnAbout_Button = null;
	}

	public Button BtnQuitGame_Button;
	public Button BtnStart_Button;
	public Button BtnAbout_Button;
}
