using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
public class UIHomePageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnQuitGame_Button = QUIManager.Instance.Get<UIHomePage>("BtnQuitGame").GetComponent<Button>();
		BtnStart_Button = QUIManager.Instance.Get<UIHomePage>("BtnStart").GetComponent<Button>();
		BtnAbout_Button = QUIManager.Instance.Get<UIHomePage>("BtnAbout").GetComponent<Button>();
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
