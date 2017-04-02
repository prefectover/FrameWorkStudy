using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework
{
	
	#region 枚举
	public enum APP_MODE
	{
		DebugMode,
		TestMode,
		ReleaseMode,
	}

	public enum eServerMode
	{
		kLocal,
		kRemote
	}
	#endregion

//	[System.Serializable]
//	public class PathItem : ScriptableObject {
//		public string name;
//		public string patn;
//	}

	[System.Serializable]
	public class PathDicionary : ScriptableObject
	{

        #region 数据区
		[Header("数据区")]
        #region 字段
		[SerializeField] private List<PathItem> mPath;
		#endregion

		#region 属性

		public List<PathItem> Path
		{
			get { return mPath; }
		}
		#endregion

		#endregion
	}
}

