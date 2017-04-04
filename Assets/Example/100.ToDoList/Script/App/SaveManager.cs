using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace ToDoList {
	
	public class SaveManager  {
		/// <summary>
		/// ToDoList 的Key
		/// </summary>
		const string TODO_LIST_TITLES_KEY = "TODO_LIST_TITLES_KEY";

		/// <summary>
		/// 是否完成的前缀
		/// </summary>
		const string TODO_LIST_COMPLETE_SUFFIX_KEY = "COMPLETE_";

		/// <summary>
		/// 是否被删除掉
		/// </summary>
		const string TODO_LIST_COMPLETE_DELETED_KEY = "DELETED_";

		/// <summary>
		/// 字符串区分用的码
		/// </summary>
		const string SPLIT_CODE = "@!@#$%";

		/// <summary>
		/// 读取
		/// </summary>
		public static List<ToDoListItemData> Load() {

			List<ToDoListItemData> retList = new List<ToDoListItemData> ();

			string titlesStamp = PlayerPrefs.GetString (TODO_LIST_TITLES_KEY, "");
			Debug.Log (titlesStamp);
			string[] titles = titlesStamp.Split (new string[]{SPLIT_CODE},System.StringSplitOptions.None);
			for (int i = 0; i < titles.Length; i++) {
				if (!string.IsNullOrEmpty (titles [i])) {

					ToDoListItemData itemData = new ToDoListItemData ();
					itemData.Title = titles [i];
					itemData.Content = PlayerPrefs.GetString (titles [i]);
					itemData.Complete = PlayerPrefs.GetInt (TODO_LIST_COMPLETE_SUFFIX_KEY + titles [i]) == 1 ? true : false;
					itemData.Deleted = PlayerPrefs.GetInt (TODO_LIST_COMPLETE_DELETED_KEY + titles [i]) == 1 ? true : false;
					if (!itemData.Deleted) {
						retList.Add (itemData);
					}
					itemData.Description ();
				}
			}

			return retList;
		}

		/// <summary>
		/// 保存
		/// </summary>
		public static void Save(List<ToDoListItemData> itemList) {

			string titleStamp = "";

			for (int i = 0;i < itemList.Count - 1;i++) {
				ToDoListItemData item = itemList[i];
				titleStamp += item.Title + SPLIT_CODE;

				PlayerPrefs.SetString (item.Title, item.Content);
				PlayerPrefs.SetInt (TODO_LIST_COMPLETE_SUFFIX_KEY + item.Title, item.Complete ? 1 : 0);
				PlayerPrefs.SetInt (TODO_LIST_COMPLETE_DELETED_KEY + item.Title, item.Deleted ? 1 : 0);
			}

			if (itemList.Count > 1) {
				titleStamp += itemList [itemList.Count - 1].Title;

				PlayerPrefs.SetString (itemList [itemList.Count - 1].Title, itemList [itemList.Count - 1].Content);
				PlayerPrefs.SetInt (TODO_LIST_COMPLETE_SUFFIX_KEY + itemList [itemList.Count - 1].Title, itemList [itemList.Count - 1].Complete ? 1 : 0);
				PlayerPrefs.SetInt (TODO_LIST_COMPLETE_DELETED_KEY + itemList [itemList.Count - 1].Title, itemList [itemList.Count - 1].Deleted ? 1 : 0);
			}

			PlayerPrefs.SetString (TODO_LIST_TITLES_KEY, titleStamp);
		}
	}
}