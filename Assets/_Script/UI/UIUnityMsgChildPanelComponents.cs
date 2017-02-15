using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
public class UIUnityMsgChildPanelComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnSendMsg_Button = QUIManager.Instance.Get<UIUnityMsgChildPanel>("BtnSendMsg").GetComponent<Button>();
		BtnClose_Button = QUIManager.Instance.Get<UIUnityMsgChildPanel>("BtnClose").GetComponent<Button>();
		BtnSendMsgWithEnumMsg_Button = QUIManager.Instance.Get<UIUnityMsgChildPanel>("BtnSendMsgWithEnumMsg").GetComponent<Button>();
	}

	public void Clear()
	{
		BtnSendMsg_Button = null;
		BtnClose_Button = null;
		BtnSendMsgWithEnumMsg_Button = null;
	}

	public Button BtnSendMsg_Button;
	public Button BtnClose_Button;
	public Button BtnSendMsgWithEnumMsg_Button;
}
