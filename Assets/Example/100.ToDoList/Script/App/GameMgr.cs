using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using SCFramework;

namespace ToDoList {
	[QMonoSingletonAttribute("[Game]/GameMgr")]
	public class GameMgr : AbstractModuleMgr, ISingleton
	{
		private static GameMgr s_Instance;

		public static GameMgr S
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = MonoSingleton.CreateMonoSingleton<GameMgr>();
				}
				return s_Instance;
			}
		}

		public void InitGameMgr()
		{
			Log.i("Init[GameMgr]");
		}

		public void OnSingletonInit()
		{

		}

		protected override void OnActorStart()
		{
			StartProcessModule module = AddMonoCom<StartProcessModule>();

			module.SetFinishListener(OnStartProcessFinish);
		}

		protected void OnStartProcessFinish()
		{
			//			PTUIManager.Instance.OpenUI<UIMainPage> (UILevel.Common);
		}
	}
}