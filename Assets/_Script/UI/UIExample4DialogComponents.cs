using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class UIExample4DialogComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnSure_Button = QUIManager.Instance.Get<UIExample4Dialog>("BtnSure").GetComponent<Button>();
		BtnCancel_Button = QUIManager.Instance.Get<UIExample4Dialog>("BtnCancel").GetComponent<Button>();
	}

	public void Clear()
	{
		BtnSure_Button = null;
		BtnCancel_Button = null;
	}

	public Button BtnSure_Button;
	public Button BtnCancel_Button;
}
