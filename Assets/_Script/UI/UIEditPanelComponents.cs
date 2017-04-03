using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
public class UIEditPanelComponents : IUIComponents
{
	public void InitUIComponents()
	{
		TitleInputField_InputField = QUIManager.Instance.Get<UIEditPanel>("TitleInputField").GetComponent<InputField>();
		ContentInputField_InputField = QUIManager.Instance.Get<UIEditPanel>("ContentInputField").GetComponent<InputField>();
		BtnSave_Button = QUIManager.Instance.Get<UIEditPanel>("BtnSave").GetComponent<Button>();
		BtnCancel_Button = QUIManager.Instance.Get<UIEditPanel>("BtnCancel").GetComponent<Button>();
	}

	public void Clear()
	{
		TitleInputField_InputField = null;
		ContentInputField_InputField = null;
		BtnSave_Button = null;
		BtnCancel_Button = null;
	}

	public InputField TitleInputField_InputField;
	public InputField ContentInputField_InputField;
	public Button BtnSave_Button;
	public Button BtnCancel_Button;
}
