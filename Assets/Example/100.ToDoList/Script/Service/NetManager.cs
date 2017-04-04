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
				obj.DeleteAsync().ContinueWith(delegate {
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

	public void Query() {
		AVQuery<AVObject> query = new AVQuery<AVObject> ("ToDoListItemData");
		query.FindAsync().ContinueWith(t=>{
			foreach(var obj in t.Result) {
				Debug.Log(obj["Title"]);
			}
		});
	}
}
