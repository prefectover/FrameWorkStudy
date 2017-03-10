using UnityEngine;
using System.Collections;
using QFramework;
using QAssetBundle;

namespace QFramework {
	public class UIFrameworkCtrl : MonoBehaviour {

		IEnumerator Start () {
			QResourceManager.Instance.Init ();			
			QResourceManager.Instance.LoadAssetBundle (UIPREFAB.BUNDLE_NAME);

			QUIManager.Instance.SetResolution (1024, 768);
			QUIManager.Instance.SetMatchOnWidthOrHeight (0);

			QUIManager.Instance.OpenUI<UIHomePage> (QUILevel.Common,UIPREFAB.BUNDLE_NAME);
			yield return null;
		}
	}
}
