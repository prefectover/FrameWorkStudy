using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
public class UIFirstPageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnBack_Button = QUIManager.Instance.Get<UIFirstPage>("BtnBack").GetComponent<Button>();
		ContentText_Text = QUIManager.Instance.Get<UIFirstPage>("ContentText").GetComponent<Text>();
	}

	public void Clear()
	{
		BtnBack_Button = null;
		ContentText_Text = null;
	}

	public Button BtnBack_Button;
	public Text ContentText_Text;
}
