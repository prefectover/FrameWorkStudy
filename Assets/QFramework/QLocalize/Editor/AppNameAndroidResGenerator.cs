using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Xml;
namespace QFramework {
	/// <summary>
	/// 生成Android资源文件
	/// </summary>
	public class AppNameAndroidResGenerator  {
		public static void Generate(AppNameConfigData curConfigData) {

			string androidResPath = Application.dataPath + "/Plugins/Android/res";

			if (!Directory.Exists (androidResPath)) {
				Directory.CreateDirectory (androidResPath);
			}

			CreateLanguageResFiles ("", Application.productName);

			// 全部先填写默认的名字
			foreach (string languageDef in curConfigData.LanguageDefAndroid) {
				string valueDirPath = Application.dataPath + "/Plugins/Android/res/values-" + languageDef;
				if (Directory.Exists (valueDirPath)) {
					CreateLanguageResFiles (languageDef, Application.productName);
				}
			}

			// 单独语言的名字
			Dictionary<string,string> languageDict = new Dictionary<string, string> ();
			foreach (var languageData in curConfigData.SupportedLanguageItems) {
				languageDict.Add (curConfigData.LanguageDefAndroid [languageData.Index], languageData.AppName);
			}
				
			foreach (KeyValuePair<string,string> languagePair in languageDict) {
				CreateLanguageResFiles (languagePair.Key, languagePair.Value);
			}

			AssetDatabase.Refresh ();
		}

		static void CreateLanguageResFiles(string language,string appName) {

			string valueDirPath = Application.dataPath + "/Plugins/Android/res/values" + (string.IsNullOrEmpty(language)?language:("-" + language));
			if (!Directory.Exists (valueDirPath)) {
				Directory.CreateDirectory (valueDirPath);
			}

			string xmlFilePath = valueDirPath + "/strings.xml";
			if (File.Exists (xmlFilePath)) {

			}
			else {
				string templateXmlFilePath = Application.dataPath + "/PTUGame/PTLocalize/Template/strings.xml";
				File.Copy (templateXmlFilePath, xmlFilePath);
			}

			XmlDocument xmlDoc = new XmlDocument ();
			xmlDoc.Load (xmlFilePath);

			XmlNodeList resourcesNodes = xmlDoc.SelectSingleNode ("resources").ChildNodes;

			foreach (XmlElement node in resourcesNodes) {
				if (string.Equals (node.GetAttribute ("name"), "app_name")) {
					node.InnerText = appName;
				}
			}
			xmlDoc.Save (xmlFilePath);
		}
	}
}
