using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.AssetBundles;
using System.Xml;

/// <summary>
/// 运行时的本地化管理器
/// TODO:
/// 1.监听系统语言切换事件,对注册其他语言的Component进行广播。
/// 2.
/// </summary>
namespace PTGame.Localize {

	public enum Language {
		Chinese 					= 6,
		English 					= 10,
		French 						= 14,
		Japanese					= 22,
		Russian						= 30,
		Spanish						= 34,
		ChineseSimplified 			= 40,
		ChineseTraditional 			= 41,
	}


//	-- Afrikaans, 0
//	-- Arabic,1
//	-- Basque,2
//	-- Belarusian,3
//	-- Bulgarian,4
//	-- Catalan,5
//	-- Chinese,6
//	-- Czech,7
//	-- Danish,8
//	-- Dutch,9
//	-- Estonian,11
//	-- Faroese,12
//	-- Finnish,13
//	-- German,15
//	-- Greek,16
//	-- Hebrew,17
//	-- Hugarian,18
//	-- Icelandic,19
//	-- Indonesian,20
//	-- Italian,21
//	-- Korean,23
//	-- Latvian,24
//	-- Lithuanian,25
//	-- Norwegian,26
//	-- Polish,27
//	-- Portuguese,28
//	-- Romanian,29
//	-- Russian,30
//	-- SerboCroatian,31
//	-- Slovak,32
//	-- Slovenian,33
//	-- Spanish,34
//	-- Swedish,35
//	-- Thai,36
//	-- Turkish,37
//	-- Ukrainian,38
//	-- Vietnamese,39
//
//	-- Unknown,42
//	-- Hungarian = 18
		
	public class PTLanguageManager : MonoBehaviour {
		/// <summary>
		/// 单例实现
		/// </summary>
		public static PTLanguageManager Instance {
			get {
				return PTMonoSingletonComponent<PTLanguageManager>.Instance;
			}
		}

		/// <summary>
		/// 临时支持跳转用的
		/// </summary>
		public bool IsChinese {
			get {
				return Application.systemLanguage == SystemLanguage.Chinese ||
				Application.systemLanguage == SystemLanguage.ChineseSimplified ||
				Application.systemLanguage == SystemLanguage.ChineseTraditional;
			}
		}


		/// <summary>
		/// 后缀
		/// </summary>
		/// <value>The suffix.</value>
		public string Suffix {
			get {
				string retValue = "cn";
				switch (Application.systemLanguage) {
					// TODO:考虑用map实现
					case SystemLanguage.Chinese:
					case SystemLanguage.ChineseSimplified:
					case SystemLanguage.ChineseTraditional:
						retValue = "cn";
						break;
					case SystemLanguage.English:
						retValue = "en";
						break;
				}
				if (DevelopmentSetting.TestFreeMode) {
					return "en";
				}
				return retValue;
			}
		}


		/// <summary>
		/// 解析
		/// </summary>
		public void Parse() {
			if (null == mTextItemDict) {
				mTextItemDict = new Dictionary<string, Dictionary<string, string>> ();

				PTResourceManager.Instance.LoadAssetBundle ("configFile");
				TextAsset textAsset = PTResourceManager.Instance.LoadAsset<TextAsset> ("configFile", "word");
//				Debug.LogError (textAsset.text);

				XmlDocument xmlDoc = new XmlDocument ();
				xmlDoc.LoadXml (textAsset.text);

				XmlNode root = xmlDoc.SelectSingleNode ("words");

				XmlNodeList childList = root.ChildNodes;
				foreach (XmlNode childNode in childList) {
					Dictionary<string,string> textItem = new Dictionary<string, string> ();

					string key = childNode.Attributes ["key"].Value;

					foreach (XmlAttribute valuePair in childNode.Attributes) {
						if (valuePair.Name == "key") {
						}
						else {
							textItem.Add (valuePair.Name, valuePair.Value);
						}
					}

					mTextItemDict.Add (key, textItem);
				}
					
				Resources.UnloadAsset (textAsset);
			}
		}

		/// <summary>
		/// 存储所有Text的字典
		/// </summary>
		Dictionary<string,Dictionary<string,string>> mTextItemDict = null;


		/// <summary>
		/// 根据Key查找文本
		/// </summary>
		public string Text4Key(string key) {
			Parse ();

			string retText = "";
			if (mTextItemDict.ContainsKey (key) && mTextItemDict[key].ContainsKey (Suffix)) {
				retText = mTextItemDict [key] [Suffix];
			}
			else {
				Debug.LogError ("No Key Name:" + key + " or No Nation " + Suffix + " Please Check The word.xml file");
			}
			return retText;
		}


		#if UNITY_EDITOR
		void OnGUI() {
			if (GUI.Button(new Rect(0,0,200,100),"Change Language")) {
				DevelopmentSetting.TestFreeMode = !DevelopmentSetting.TestFreeMode;
				ChangeLanguage ();
			}
		}
		#endif

		void ChangeLanguage() {

			List<GameObject> activeGos = new List<GameObject> ();

			foreach (var localizeView in mActiveLocalizeViewGos) {
				if (localizeView) {
					activeGos.Add (localizeView);
					localizeView.GetComponent<ILocalizeView> ().UpdateLocalizeView ();
				}
			}

			mActiveLocalizeViewGos.Clear ();
			mActiveLocalizeViewGos = activeGos;
		}

		List<GameObject> mActiveLocalizeViewGos = new List<GameObject>();

		public void PushActiveLocalizeView(MonoBehaviour localizeView) {
			mActiveLocalizeViewGos.Add (localizeView.gameObject);
		}

		public void RemoveActiveLocalizeView(MonoBehaviour localizeView) {
			mActiveLocalizeViewGos.Remove (localizeView.gameObject);
		}
	}
}