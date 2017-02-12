using UnityEngine;
using System.Collections;

using QFramework;
using QFramework.UI;

namespace QFramework.Example.UIFramework {
	
	public class Main : MonoBehaviour {

		// Use this for initialization
		void Start () {
			QUIManager.Instance.OpenUI<UIMainPage> (QUILevel.Common, null);
		}
			
	}

}