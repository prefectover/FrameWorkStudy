using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
public class UIExample5GamePageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnBack_Button = QUIManager.Instance.Get<UIExample5GamePage>("BtnBack").GetComponent<Button>();
	}

	public void Clear()
	{
		BtnBack_Button = null;
	}

	public Button BtnBack_Button;
}
