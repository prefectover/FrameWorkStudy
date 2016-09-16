using UnityEngine;
using System.Collections;
using QFramework.UI;
using QFramework.AB;

namespace QFramework {
	
	public class Instance {

		public static IEnumerator Init()
		{
			yield return QMsgCenter.Instance.Init ();

			yield return QResMgr.Instance.Init ();

			yield return QSoundMgr.Instance.Init ();

			// TODO:要配置 以后支持NGUI
			yield return QUGUIMgr.Init ();

		}
	}
}
