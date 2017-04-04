using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
using ToDoList;

public class UIToDoListItem : QMonoBehaviour {

	protected override void SetupMgr ()
	{
		mCurMgr = QUIManager.Instance;
	}

	public override void ProcessMsg (QMsg msg) 
	{
		
	}

	[SerializeField] Text m_Title;
	[SerializeField] Button m_BtnComplete;
	[SerializeField] Button m_BtnDelete;

	ToDoListItemData m_ToDoListItemData;

	public ToDoListItemData ToDoListItemData {
		set {
			m_ToDoListItemData = value;
			UpdateView ();
		}
	}

	void Awake() {
		m_Title = transform.Find ("Title").GetComponent<Text> ();
		m_BtnComplete = transform.Find ("BtnComplete").GetComponent<Button> ();
		m_BtnDelete = trans.Find ("BtnDelete").GetComponent <Button> ();

		m_BtnComplete.onClick.AddListener (delegate {
			m_ToDoListItemData.Complete = true;
			this.SendMsg(new ModifiedItemMsg((ushort)UIToDoListPageEvent.ModifiedItem,m_ToDoListItemData.Title,m_ToDoListItemData));
		});


		m_BtnDelete.onClick.AddListener (delegate {
			m_ToDoListItemData.Deleted = true;
			this.SendMsg(new DeleteItemMsg((ushort)UIToDoListPageEvent.DeleteItem,m_ToDoListItemData.Title));
		});


		transform.Find ("Bg").gameObject.GetComponent<Button>().onClick.AddListener(delegate {
			UIEditPanelData editPanelData = new UIEditPanelData();
			editPanelData.isNew = false;
			editPanelData.ToDoListItemData = m_ToDoListItemData;
			QUIManager.Instance.OpenUI<UIEditPanel>(QUILevel.PopUI,QAssetBundle.UIPREFAB.BUNDLE_NAME,editPanelData);	
		});
		UpdateView ();
	}

	void UpdateView() {
		if (null != m_ToDoListItemData) {
			m_Title.text = m_ToDoListItemData.Title;
			m_BtnComplete.gameObject.SetActive (!m_ToDoListItemData.Complete);
		}
	}
}
