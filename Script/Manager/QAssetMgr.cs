using UnityEngine;
using System.Collections;
using QFramework;
using System.Collections.Generic;

public class QAssetMgr : QMgrBehaviour {

	public static QAssetMgr Instance = null;

	protected override void SetupMgr ()
	{
		
	}

	void Awake()
	{
		Instance = this;
	}

	protected override void SetupMgrId ()
	{
		mMgrId = (ushort)QMgrID.AB;
	}



	public GameObject GetGameObject(string name)
	{
		if (sonMembers.ContainsKey(name))
		{
			return sonMembers [name];
		}

		return null;
	}
	public void RegisterGameObject(string name,GameObject obj)
	{
		if (!sonMembers.ContainsKey(name))
		{
			sonMembers.Add(name,obj);
		}
	}

	public void UnRegisterGameObject(string name)
	{
		if (!sonMembers.ContainsKey(name))
		{
			sonMembers.Remove(name);
		}
	}

	// 规定了 开发方式 消耗内存 换区速度和方便。
	Dictionary<string,GameObject> sonMembers = new Dictionary<string,GameObject>();
}
