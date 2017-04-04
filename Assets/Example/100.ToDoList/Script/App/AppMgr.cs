using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using DG.Tweening;
using QAssetBundle;

namespace ToDoList {

	[QMonoSingletonAttribute("[App]/AppMgr")]
	public class AppMgr : AbstractApplicationMgr<AppMgr> {

		protected override void InitThirdLibConfig()
		{
			DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
		}

		protected override void InitAppEnvironment()
		{
			Application.targetFrameRate = 30;
			Application.runInBackground = true;
			var consoleInit = QConsole.Instance;
			QResourceManager.Instance.Init ();
			QResourceManager.Instance.LoadAB (UIPREFAB.BUNDLE_NAME);

			QUIManager.Instance.SetResolution (640, 1136);
			QUIManager.Instance.SetMatchOnWidthOrHeight (1);
		}

		protected override void StartGame()
		{
			QUIManager.Instance.OpenUI<UIToDoListPage> (QUILevel.Common, UIPREFAB.BUNDLE_NAME);
		}
	}
}
