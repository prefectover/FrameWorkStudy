using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
public class UIToDoListPageComponents : IUIComponents
{
	public void InitUIComponents()
	{
		BtnAddToDoItem_Button = QUIManager.Instance.Get<UIToDoListPage>("BtnAddToDoItem").GetComponent<Button>();
		ToDoList_Image = QUIManager.Instance.Get<UIToDoListPage>("ToDoList").GetComponent<Image>();
		Items_Transform = QUIManager.Instance.Get<UIToDoListPage>("Items").GetComponent<Transform>();
		UIToDoItemPrefab_Transform = QUIManager.Instance.Get<UIToDoListPage>("UIToDoItemPrefab").GetComponent<Transform>();
	}

	public void Clear()
	{
		BtnAddToDoItem_Button = null;
		ToDoList_Image = null;
		Items_Transform = null;
		UIToDoItemPrefab_Transform = null;
	}

	public Button BtnAddToDoItem_Button;
	public Image ToDoList_Image;
	public Transform Items_Transform;
	public Transform UIToDoItemPrefab_Transform;
}
