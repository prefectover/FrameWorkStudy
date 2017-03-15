using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace QFramework {

	public class LanguageData {
		public int Index = 0;
		public string AppName = "";
		public LanguageData() {}
		public LanguageData(int index,string appName) {
			Index = index;
			AppName = appName;
		}
	}


	public class AppNameConfigData  {

		const string COUNTRY_PREFIX = "PTLocalizeAppName";

		public static string [] LanguageDef = new string[]{
			"None",
			"English",
			"Frence",
			"German",
			"Chinese(Simplified)",
			"Chinese(Traditional)",
			"Japanese",
			"Spanish",
			"Spanish(Mexico)",
			"Italian",
			"Dutch",
			"Korean",
			"Portuguese(Brazil)",
			"Portuguese(Portugal)",
			"Danish",
			"Finnish",
			"Norwegian Bokmal",
			"Swedish",
			"Russian",
			"Polish",
			"Turkish",
			"Arabic",
			"Thai",
			"Czech",
			"Hungarian",
			"Catalan",
			"Croatian",
			"Greek",
			"Hebrew",
			"Romanian",
			"Slovak",
			"Ukrainian",
			"Indonesian",
			"Malay",
			"Vietnamese"
		};

		public string[] LanguageDefAndroid = new string[] {
			"None",
			"en",
			"fr",
			"de",
			"zh-rCN",
			"zh",
			"ja",
			"es",
			"es-rUS",
			"it",
			"nl",
			"ko",
			"pt-rBR",
			"pt-rPT",
			"da",
			"fi",
			"nb",
			"sv",
			"ru",
			"pl",
			"tr",
			"ar",
			"th",
			"cs",
			"hu",
			"ca",
			"hr",
			"el",
			"he",
			"ro",
			"sk",
			"uk",
			"id",
			"ms",
			"vi"
		};

		public string[] LanguageDefiOS = new string[] {
			"None",
			"en",
			"fr",
			"de",
			"zh-Hans",
			"zh-Hant",
			"ja",
			"es",
			"es-MX",
			"it",
			"nl",
			"ko",
			"pt-BR",
			"pt-PT",
			"da",
			"fi",
			"nb",
			"sv",
			"ru",
			"pl",
			"tr",
			"ar",
			"th",
			"cs",
			"hu",
			"ca",
			"hr",
			"el",
			"he",
			"ro",
			"sk",
			"uk",
			"id",
			"ms",
			"vi"
		};


		public List<LanguageData> SupportedLanguageItems = new List<LanguageData>();

		public static AppNameConfigData Load() {
			var retConfigData = new AppNameConfigData ();
			string keyPrefix = COUNTRY_PREFIX + Application.productName + Application.companyName;

			for (int i = 0;i < LanguageDef.Length; i++) {
				string appName = EditorPrefs.GetString (keyPrefix + LanguageDef[i], "");
				if (!string.IsNullOrEmpty (appName)) {
					retConfigData.SupportedLanguageItems.Add (new LanguageData(i, appName));
				}
			}

			return retConfigData;
		}

		public void Save() {

			string keyPrefix = COUNTRY_PREFIX + Application.productName + Application.companyName;

			for (int i = 0; i < LanguageDef.Length; i++) {
				if (EditorPrefs.HasKey (keyPrefix + LanguageDef[i])) {
					EditorPrefs.DeleteKey (keyPrefix + LanguageDef[i]);
				}
			}

			foreach (LanguageData languageData in SupportedLanguageItems) {
				EditorPrefs.SetString (keyPrefix + LanguageDef[languageData.Index], languageData.AppName);
				Debug.Log (keyPrefix + LanguageDef[languageData.Index] + ":" + languageData.AppName);
			}
		}
	}

}