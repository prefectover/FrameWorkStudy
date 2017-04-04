using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QAssetBundle;
using ToDoList;

public enum UIToDoListPageEvent {
	Began = QMgrID.UI,
	CreateNewItem,
	ModifiedItem,
	DeleteItem,
	Ended
}

public enum UIEvent {
	Began = UIToDoListPageEvent.Ended,
	UpdateView,
	Ended
}


public class UIToDoListPage : QUIBehaviour
{
	Dictionary<string,UIToDoListItem> todoListItemDict;

	protected override void InitUI(object uiData = null)
	{
		mUIComponents = mIComponents as UIToDoListPageComponents;
		//please add init code here

		RegisterSelf (this, new ushort[] {
			(ushort)UIToDoListPageEvent.CreateNewItem,
			(ushort)UIToDoListPageEvent.ModifiedItem,
			(ushort)UIToDoListPageEvent.DeleteItem,
			(ushort)UIEvent.UpdateView
		});
		todoListItemDict = new Dictionary<string, UIToDoListItem> ();

		UpdateView ();
	}

	public override void ProcessMsg (QMsg msg)
	{
		switch (msg.msgId) {
			case (ushort)UIToDoListPageEvent.CreateNewItem:
				CreateNewItemMsg createNewItemMsg = msg as CreateNewItemMsg;
				createNewItemMsg.msgId = (ushort)ToDoListEvent.CreateNewItem;
				createNewItemMsg.NewItemData.Description ();
				this.SendMsg (createNewItemMsg);
				break;
			case (ushort)UIToDoListPageEvent.ModifiedItem:
				ModifiedItemMsg modifiedItemMsg = msg as ModifiedItemMsg;
				modifiedItemMsg.msgId = (ushort)ToDoListEvent.ModifiedItem;
				modifiedItemMsg.ItemData.Description ();
				this.SendMsg (modifiedItemMsg);
				break;
			case (ushort)UIToDoListPageEvent.DeleteItem:
				DeleteItemMsg deleteItemMsg = msg as DeleteItemMsg;
				deleteItemMsg.msgId = (ushort)ToDoListEvent.DeleteItem;
				this.SendMsg (deleteItemMsg);
				break;
			case (ushort)UIEvent.UpdateView:
				UpdateView ();
				break;
		}
	}

	void UpdateView() {

		var todoListItemData = ToDoListManager.Instance.CurCachedData;

		Debug.Log (todoListItemDict.Count);
		foreach (var itemPair in todoListItemDict) {
			Destroy (itemPair.Value.gameObject);
		}

		todoListItemDict.Clear ();
		Debug.Log (todoListItemDict.Count);

		foreach (var dataPair in todoListItemData) {
			var prefab = mUIComponents.UIToDoItemPrefab_Transform;

			Transform obj = GameObject.Instantiate (prefab);
			var itemScript = obj.gameObject.GetComponent<UIToDoListItem> ();
			obj.gameObject.SetActive (true);

			itemScript.ToDoListItemData = dataPair.Value;

			todoListItemDict.Add (dataPair.Key, itemScript);

			obj.transform.SetParent (mUIComponents.Items_Transform);
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
		}

		var contentRectTrans =  mUIComponents.Items_Transform.GetComponent<RectTransform> ();
		var groupLayout = mUIComponents.Items_Transform.GetComponent<GridLayoutGroup> ();
		var sizeDelta = contentRectTrans.sizeDelta;

		groupLayout.CalculateLayoutInputVertical ();
		Debug.Log (groupLayout.preferredHeight);

		sizeDelta.y = todoListItemDict.Count * (groupLayout.spacing.y + groupLayout.cellSize.y ) + groupLayout.padding.top + groupLayout.padding.bottom;
		contentRectTrans.sizeDelta = sizeDelta;

		Debug.Log (todoListItemDict.Count);

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