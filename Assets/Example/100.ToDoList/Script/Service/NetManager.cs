using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeanCloud;
using ToDoList;
using System.Threading.Tasks;
using QFramework;

/// <summary>
/// https://leancloud.cn/docs/unity_guide.html
/// </summary>
public class AVOSItemData {
	public string ClassName = "ToDoListData";
	public string ObjectId;
	public string Title;
	public string Content;
}

public class NetManager : QMgrBehaviour {

	public static NetManager Instance {
		get {
			return QMonoSingletonComponent<NetManager>.Instance;
		}
	}


	protected override void SetupMgrId ()
	{
		mMgrId = (ushort)QMgrID.Network;
	}

	public override void ProcessMsg (QMsg msg)
	{
		
	}

	public void ModifiedItemUpload(string title,ToDoListItemData itemData) {
		DeleteItemUpload (title, delegate {
			NewItemUpload(itemData);
		});
	}

	public void DeleteItemUpload(string title,System.Action callback = null) {
		new AVQuery<AVObject> ("ToDoListItemData").WhereEqualTo ("Title", title).FindAsync ().ContinueWith (t => {
			foreach(var obj in t.Result) {
				obj["Deleted"] = true;
				obj.SaveAsync().ContinueWith(delegate {
					if (null != callback) {
						callback();
					}
				});
			}
		});
	}

	/// <summary>
	/// News the item upload.
	/// </summary>
	/// <param name="itemData">Item data.</param>
	public void NewItemUpload(ToDoListItemData itemData) {
		AVObject toDoListItemData = new AVObject ("ToDoListItemData");
		toDoListItemData ["Title"] = itemData.Title;
		toDoListItemData ["Content"] = itemData.Content;
		toDoListItemData ["Complete"] = itemData.Complete;
		Task saveTask = toDoListItemData.SaveAsync ();
	}


	/// <summary>
	/// 获取所有的数据
	/// </summary>
	public void Query(System.Action<List<ToDoListItemData>> queryCallback) {
		StartCoroutine (QueryAll (queryCallback));
	}

	IEnumerator QueryAll(System.Action<List<ToDoListItemData>> queryCallback) {
		AVQuery<AVObject> query = new AVQuery<AVObject> ("ToDoListItemData");

		var list = new List<ToDoListItemData>();

		bool querySucceed = false;
		query.FindAsync().ContinueWith(t=>{
			foreach(var obj in t.Result) {
				var itemData = new ToDoListItemData();
				itemData.Title = obj["Title"] as string;
				itemData.Complete = bool.Parse(obj["Complete"].ToString());
				itemData.Content = obj["Content"] as string;
				list.Add(itemData);
			}
			querySucceed = true;
		});

		while (!querySucceed) {
			yield return new WaitForEndOfFrame ();
		}
		queryCallback(list);
	}
}
