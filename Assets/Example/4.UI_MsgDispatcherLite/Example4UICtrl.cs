using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using QFramework;
using QFramework;
using QAssetBundle;

/// <summary>
/// 可以接收的消息类型
/// </summary>
public class Example4UIMsg {

	public const string SEND_MSG_TO_EXAMPLE_4_UI_CTRL = "SendMsgToExample4UICtrl";

	public const string MAIN_PAGE_BTN_START_CLICK = "MainPageBtnStartClick";
	public const string MAIN_PAGE_BTN_ABOUT_CLICK = "MainPageBtnAboutClick";
	public const string MAIN_PAGE_BTN_QUIT_CLICK = "MainPageBtnQuitClick";

	public const string ABOUT_PAGE_BTN_BACK_CLICK = "AboutPageBtnBackClick";
	public const string GAME_PAGE_BTN_BACK_CLICK = "GamePageBtnBackClick";

	public const string DIALOG_BTN_SURE_CLICK = "DialogPageBtnSureClick";
	public const string DIALOG_BTN_CANCEL_CLICK = "DialogPageBtnCancelClick";
}


namespace QFramework {

	/// <summary>
	/// 控制器
	/// </summary>
	public class Example4UICtrl : MonoBehaviour,IMsgReceiver {


		// Use this for initialization
		void Start () {
			// 注册消息
			this.RegisterMsgByChannel(QMgrID.UI,Example4UIMsg.SEND_MSG_TO_EXAMPLE_4_UI_CTRL,ProcessEvent);

			QResourceManager.Instance.Init ();
			QResourceManager.Instance.LoadAssetBundle (UIPREFAB.BUNDLE_NAME);

			QUIManager.Instance.SetResolution (1024, 768);
			QUIManager.Instance.SetMatchOnWidthOrHeight (0);

			QUIManager.Instance.OpenUI<UIExample4MainPage> (QUILevel.Common, UIPREFAB.BUNDLE_NAME);
		}

		/// <summary>
		/// 处理消息
		/// </summary>
		void ProcessEvent(object[] paramList) {
			// TODO:写到一半 以后接着写
			string eventName = (string)paramList[0];

			switch (eventName) {
				case Example4UIMsg.MAIN_PAGE_BTN_START_CLICK:
					QUIManager.Instance.CloseUI<UIExample4MainPage> ();
					QUIManager.Instance.OpenUI<UIExample4GamePage> (QUILevel.Common, UIPREFAB.BUNDLE_NAME);
					break;
				case Example4UIMsg.MAIN_PAGE_BTN_ABOUT_CLICK:
					QUIManager.Instance.CloseUI<UIExample4MainPage> ();
					QUIManager.Instance.OpenUI<UIExample4AboutPage> (QUILevel.Common, UIPREFAB.BUNDLE_NAME);
					break;
				case Example4UIMsg.MAIN_PAGE_BTN_QUIT_CLICK:
					QUIManager.Instance.OpenUI<UIExample4Dialog> (QUILevel.Forward, UIPREFAB.BUNDLE_NAME);
					break;
				case Example4UIMsg.ABOUT_PAGE_BTN_BACK_CLICK:
					QUIManager.Instance.CloseUI<UIExample4AboutPage> ();
					QUIManager.Instance.OpenUI<UIExample4MainPage> (QUILevel.Common, UIPREFAB.BUNDLE_NAME);
					break;
				case Example4UIMsg.GAME_PAGE_BTN_BACK_CLICK:
					QUIManager.Instance.CloseUI<UIExample4GamePage> ();
					QUIManager.Instance.OpenUI<UIExample4MainPage> (QUILevel.Common, UIPREFAB.BUNDLE_NAME);
					break;

				case Example4UIMsg.DIALOG_BTN_SURE_CLICK:
					UnityEditor.EditorApplication.isPlaying = false;
					break;
				case Example4UIMsg.DIALOG_BTN_CANCEL_CLICK:
					QUIManager.Instance.CloseUI<UIExample4Dialog> ();
					break;
			}
		}

		/// <summary>
		/// 要注销消息
		/// </summary>
		void OnDestroy() {
			this.UnRegisterMsgByChannel (QMgrID.UI, Example4UIMsg.SEND_MSG_TO_EXAMPLE_4_UI_CTRL);
		}

	}

}
