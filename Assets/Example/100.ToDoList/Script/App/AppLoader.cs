using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCFramework;

namespace ToDoList {

	public class AppLoader : MonoBehaviour {

		private void Awake()
		{
			Log.i("Init[{0}]", AppMgr.Instance.GetType().Name );
		}

		private void Start()
		{
			Destroy(gameObject);
		}
	}

}