using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace ToDoList {

	public class UIEditPanelData {
		
		public string Title;

		public string Content;
	
	}

	public class ToDoListManager : QMgrBehaviour {

		public Dictionary<string,ToDoListItemData> m_CachedData;

		protected override void SetupMgrId ()
		{
			mMgrId = (ushort)QMgrID.Data;
		}


		public static ToDoListManager Instance {
			get {
				return QMonoSingletonComponent<ToDoListManager>.Instance;
			}
		}

		private ToDoListManager() {
			var list = SaveManager.Load ();

			m_CachedData = new Dictionary<string, ToDoListItemData> ();
			foreach (var itemData in list) {
				m_CachedData.Add (itemData.Title, itemData);
			}
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