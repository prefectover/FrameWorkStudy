using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.UI;
public class UIUnityMsgRootPageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		Title_Text = QUIManager.Instance.Get<UIUnityMsgRootPage>("Title").GetComponent<Text>();
		BtnOpenChildPage_Button = QUIManager.Instance.Get<UIUnityMsgRootPage>("BtnOpenChildPage").GetComponent<Button>();
		ReceivedMsg_Text = QUIManager.Instance.Get<UIUnityMsgRootPage>("ReceivedMsg").GetComponent<Text>();
	}

	public void Clear()
	{
		Title_Text = null;
		BtnOpenChildPage_Button = null;
		ReceivedMsg_Text = null;
	}

	public Text Title_Text;
	public Button BtnOpenChildPage_Button;
	public Text ReceivedMsg_Text;
}
