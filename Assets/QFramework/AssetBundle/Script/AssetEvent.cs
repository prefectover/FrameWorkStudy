using UnityEngine;
using System.Collections;
using QFramework;
using QFramework;

public enum AssetEvent
{
	HunkRes = QMgrID.AB + 1,

	ReleaseSingleObj,
	ReleaseBundleObj,
	ReleaseSceneObj,

	ReleaseSingleBundle,
	ReleaseSceneBundle,
	ReleaseAll,
}

/// <summary>
/// 上层需要发给客户端的
/// </summary>
public class HunkAssetRes :QMsg
{
	public string sceneName;

	public string bundleName;

	public string resName;

	public ushort backMsgId;

	public bool isSingle;


	public HunkAssetRes(bool isSingle,ushort msgId,string sceneName,string bundleName,string resName,ushort backMsgId)
	{
		
		this.isSingle = isSingle;
		this.msgId = msgId;
		this.sceneName = sceneName;
		this.bundleName = bundleName;
		this.resName = resName;

		this.backMsgId = backMsgId;

	}
		
}

/// <summary>
/// 返回给上层的消息，上层消息
/// </summary>
public class HunkAssetResBack : QMsg
{
	public Object[] value;

	public HunkAssetResBack()
	{
		this.msgId = 0;
		this.value = null;
	}

	public void Changer(ushort msgId,params Object[] tmpValue)
	{
		this.msgId = msgId;
		this.value = tmpValue;
	}

	public void Changer(ushort msgId)
	{
		this.msgId = msgId;
	}

	public void Changer(params Object[] value)
	{
		this.value = value;
	}
}
