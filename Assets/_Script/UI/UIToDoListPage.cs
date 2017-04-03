using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QAssetBundle;
using ToDoList;

public class UIToDoListPage : QUIBehaviour
{
	Dictionary<string,UIToDoListItem> todoListItemDict;

	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIToDoListPageComponents;
		//please add init code here



	}
	public override void ProcessMsg (QMsg msg)
	{
		throw new System.NotImplementedException ();
	}
	protected override void RegisterUIEvent()
	{
		mUIComponents.BtnAddToDoItem_Button.onClick.AddListener (delegate {
			UIEditPanelData editPanelData = new UIEditPanelData();
			editPanelData.isNew = true;

			QUIManager.Instance.OpenUI<UIEditPanel>(QUILevel.PopUI,UIPREFAB.BUNDLE_NAME,editPanelData);
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

	protected override void OnClose()
	{
		base.OnClose();
	}

	void ShowLog(string content)
	{
		Debug.Log("[ UIToDoListPage:]" + content);
	}

	UIToDoListPageComponents mUIComponents = null;
}