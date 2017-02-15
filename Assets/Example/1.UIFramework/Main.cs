using UnityEngine;
using System.Collections;

using QFramework;
using QFramework.UI;

namespace QFramework.UIFramework.Example1 {
	
	public class Main : MonoBehaviour {

		// Use this for initialization
		void Start () {
			QUIManager.Instance.OpenUI<UIMainPage> (QUILevel.Common, null);
		}
	}
}