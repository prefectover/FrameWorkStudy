using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using QFramework;
using PTGame.AssetBundles;
using QAB;

/// <summary>
/// 可以接收的消息类型
/// </summary>
public class Example5UIMsg {
	public const string SEND_MSG_TO_EXAMPLE_5_UI_CTRL = "SendMsgToExample4UICtrl";
	public const string BTN_START_CLICK = "BtnStartClick";
	public const string BTN_ABOUT_CLICK = "BtnAboutClick";
	public const string BTN_QUIT_CLICK = "BtnQuitClick";
	public const string BTN_BACK_CLICK = "BtnBackClick";
	public const string BTN_SURE_CLICK = "BtnSureClick";
	public const string BTN_CANCEL_CLICK = "BtnCancelClick";
}
	
namespace QFramework {

	/// <summary>
	/// 控制器
	/// </summary>
	public class Example5UICtrl : MonoBehaviour,IMsgReceiver {

		QFSMLite mFSM = new QFSMLite();

		const string STATE_MAIN_PAGE = "MainPage";
		const string STATE_GAME_PAGE = "GamePage";
		const string STATE_ABOUT_PAGE = "AboutPage";
		const string STATE_QUIT_DIALOG = "Dialog";
		const string STATE_QUIT = "Quit";

		// Use this for initialization
		void Start () {
			// 注册消息
			this.RegisterMsgByChannel(QMsgChannel.UI,Example5UIMsg.SEND_MSG_TO_EXAMPLE_5_UI_CTRL,ProcessEvent);

			QResourceManager.Instance.Init ();
			QResourceManager.Instance.LoadAssetBundle (UIPREFAB.BUNDLENAME);

			QUIManager.Instance.SetResolution (1024, 768);
			QUIManager.Instance.SetMatchOnWidthOrHeight (0);


			mFSM.AddState (STATE_MAIN_PAGE);
			mFSM.AddState (STATE_GAME_PAGE);
			mFSM.AddState (STATE_ABOUT_PAGE);
			mFSM.AddState (STATE_QUIT_DIALOG);
			mFSM.AddState (STATE_QUIT);

			// main->game
			mFSM.AddTranslation (STATE_MAIN_PAGE, Example5UIMsg.BTN_START_CLICK, STATE_GAME_PAGE, delegate {
				QUIManager.Instance.DeleteUI<UIExample5MainPage> ();
				QUIManager.Instance.OpenUI<UIExample5GamePage> (QUILevel.Common, UIPREFAB.BUNDLENAME);
			});

			// main->about
			mFSM.AddTranslation (STATE_MAIN_PAGE, Example5UIMsg.BTN_ABOUT_CLICK, STATE_ABOUT_PAGE, delegate {
				QUIManager.Instance.DeleteUI<UIExample5MainPage> ();
				QUIManager.Instance.OpenUI<UIExample5AboutPage> (QUILevel.Common, UIPREFAB.BUNDLENAME);
			});

			// main->quit
			mFSM.AddTranslation (STATE_MAIN_PAGE, Example5UIMsg.BTN_QUIT_CLICK, STATE_QUIT_DIALOG, delegate {
				QUIManager.Instance.OpenUI<UIExample5Dialog> (QUILevel.Forward, UIPREFAB.BUNDLENAME);
			});

			// about->main
			mFSM.AddTranslation (STATE_ABOUT_PAGE, Example5UIMsg.BTN_BACK_CLICK, STATE_MAIN_PAGE, delegate {
				QUIManager.Instance.DeleteUI<UIExample5AboutPage> ();
				QUIManager.Instance.OpenUI<UIExample5MainPage> (QUILevel.Common, UIPREFAB.BUNDLENAME);
			});

			// game->main
			mFSM.AddTranslation (STATE_GAME_PAGE, Example5UIMsg.BTN_BACK_CLICK, STATE_MAIN_PAGE, delegate {
				QUIManager.Instance.DeleteUI<UIExample5GamePage> ();
				QUIManager.Instance.OpenUI<UIExample5MainPage> (QUILevel.Common, UIPREFAB.BUNDLENAME);
			});

			// quit->quitgame
			mFSM.AddTranslation (STATE_QUIT_DIALOG, Example5UIMsg.BTN_SURE_CLICK, STATE_QUIT, delegate {
				UnityEditor.EditorApplication.isPlaying = false;
			});

			// quit->main
			mFSM.AddTranslation (STATE_QUIT_DIALOG, Example5UIMsg.BTN_CANCEL_CLICK, STATE_MAIN_PAGE, delegate {
				QUIManager.Instance.DeleteUI<UIExample5Dialog> ();
			});

			// 设置好当前状态
			mFSM.Start (STATE_MAIN_PAGE);
			QUIManager.Instance.OpenUI<UIExample5MainPage> (QUILevel.Common, UIPREFAB.BUNDLENAME);
		}

		/// <summary>
		/// 处理消息
		/// </summary>
		void ProcessEvent(object[] paramList) {
			string eventName = (string)paramList[0];
			mFSM.HandleEvent (eventName);
		}

		/// <summary>
		/// 要注销消息
		/// </summary>
		void OnDestroy() {
			this.UnRegisterMsgByChannel (QMsgChannel.UI, Example5UIMsg.SEND_MSG_TO_EXAMPLE_5_UI_CTRL);
		}

	}

}
