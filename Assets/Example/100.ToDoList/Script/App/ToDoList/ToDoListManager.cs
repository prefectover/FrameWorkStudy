using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace ToDoList {

	public class UIEditPanelData {
		
		public string Title;

		public string Content;
	
	}

	public enum ToDoListEvent {
		Began = (ushort)QMgrID.Data,
		ModifiedItem,
		CreateNewItem,
		DeleteItem,
		End
	}

	public class CreateNewItemMsg :QMsg {
		public ToDoListItemData NewItemData;
		public CreateNewItemMsg(ushort msgId,ToDoListItemData newItemData):base(msgId) {
			this.NewItemData = newItemData;
		}
	}

	public class ModifiedItemMsg : QMsg {
		public string SrcTitle;
		public ToDoListItemData ItemData;
		public ModifiedItemMsg(ushort msgId,string srcTitle,ToDoListItemData itemData):base(msgId) {
			this.SrcTitle = srcTitle;
			this.ItemData = itemData;
		}
	}

	public class DeleteItemMsg : QMsg {
		public string Title;
		public DeleteItemMsg(ushort msgId,string title):base(msgId) {
			this.Title = title;
		}
	}

	public class ToDoListManager : QMgrBehaviour {

		Dictionary<string,ToDoListItemData> m_CachedData;

		public Dictionary<string,ToDoListItemData> CurCachedData {
			get {
				return m_CachedData;
			}
		}

		protected override void SetupMgrId ()
		{
			mMgrId = (ushort)QMgrID.Data;
		}
			
		public override void ProcessMsg (QMsg msg)
		{
			switch (msg.msgId) {
				case (ushort)ToDoListEvent.ModifiedItem:
					ModifiedItemMsg modifiedMsg = msg as ModifiedItemMsg;
					m_CachedData.Remove (modifiedMsg.SrcTitle);
					modifiedMsg.ItemData.Description ();
					m_CachedData.Add (modifiedMsg.ItemData.Title,modifiedMsg.ItemData);
					break;
				case (ushort)ToDoListEvent.CreateNewItem:
					CreateNewItemMsg newItemMsg = msg as CreateNewItemMsg;
					newItemMsg.NewItemData.Description ();
					m_CachedData.Add (newItemMsg.NewItemData.Title,newItemMsg.NewItemData);
					break;
				case (ushort)ToDoListEvent.DeleteItem:
					DeleteItemMsg deleteItemMsg = msg as DeleteItemMsg;
					m_CachedData.Remove (deleteItemMsg.Title);
					break;
			}
		}


		public static ToDoListManager Instance {
			get {
				return QMonoSingletonComponent<ToDoListManager>.Instance;
			}
		}

		private ToDoListManager() {

		}

		void Awake() {
			Debug.Log ("ToDoList Manager Awake");
			var list = SaveManager.Load ();

			m_CachedData = new Dictionary<string, ToDoListItemData> ();
			foreach (var itemData in list) {
				m_CachedData.Add (itemData.Title, itemData);
			}

			RegisterSelf (this, new ushort[] {
				(ushort)ToDoListEvent.CreateNewItem,
				(ushort)ToDoListEvent.ModifiedItem,
				(ushort)ToDoListEvent.DeleteItem
			});
		}


		public void UpdateData(ToDoListItemData itemData) {
			m_CachedData [itemData.Title] = itemData;
		}

		void OnDestroy() {
			SaveManager.Save (new List<ToDoListItemData>(m_CachedData.Values));
		}

		void OnApplicationQuit() {
			SaveManager.Save (new List<ToDoListItemData>(m_CachedData.Values));
		}

		void OnApplicationPause(bool pause) {

			if(pause) {
				SaveManager.Save (new List<ToDoListItemData>(m_CachedData.Values));
			}
		}

		void OnApplicationFocus(bool focus) {
			if (!focus) {
				SaveManager.Save (new List<ToDoListItemData>(m_CachedData.Values));
			}
		}
	}

}