using UnityEngine;
using System.Collections;
using QFramework;
using QAB;

namespace QFramework {
	public class UIFrameworkCtrl : MonoBehaviour {

		IEnumerator Start () {
			QResourceManager.Instance.Init ();			
			QResourceManager.Instance.LoadAssetBundle (UIPREFAB.BUNDLENAME);

			QUIManager.Instance.SetResolution (1024, 768);
			QUIManager.Instance.SetMatchOnWidthOrHeight (0);

			QUIManager.Instance.OpenUI<UIHomePage> (QUILevel.Common,UIPREFAB.BUNDLENAME);
			yield return null;
		}
	}
}
