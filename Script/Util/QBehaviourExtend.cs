using UnityEngine;
using System.Collections;

namespace QFramework.Extend {
	
	public static class QBehaviourExtend  {

		public static void OnClick(this MonoBehaviour behaviour,QVoidDelegate.WithVoid callback)
		{
			QFramework.UI.UGUIEventListener.Get (behaviour.gameObject);

			var listener = QFramework.UI.UGUIEventListener.CheckAndAddListener (behaviour.gameObject);
			listener.onClick += callback;
		}


	}

}
