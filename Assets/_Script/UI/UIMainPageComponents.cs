using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;

public class UIMainPageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnFirst_Button = QUIManager.Instance.Get<UIMainPage>("BtnFirst").GetComponent<Button>();
		BtnSecond_Button = QUIManager.Instance.Get<UIMainPage>("BtnSecond").GetComponent<Button>();
	}

	public void Clear()
	{
		BtnFirst_Button = null;
		BtnSecond_Button = null;
	}

	public Button BtnFirst_Button;
	public Button BtnSecond_Button;
}