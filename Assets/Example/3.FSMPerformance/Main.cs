using UnityEngine;
using System.Collections;

using QFramework;
using QFramework.UI;

namespace QFramework.UIFramework.Example3 {

	public class Main : MonoBehaviour {

		void Awake() {
			QUIManager.Instance.OpenUI<UIUnityMsgRootPage> (QUILevel.Common, null, null);
		}
	}
}