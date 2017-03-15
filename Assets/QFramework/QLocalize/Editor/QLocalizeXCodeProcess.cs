using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace QFramework {
	public static class QLocalizeXCodeProcess 
	{
		[PostProcessBuild(1002)]
		public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
		{
			if (target != BuildTarget.iOS) {
				Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
				return;
			}
			

			#region 写入Plist
			Debug.Log("Run mine ----------------------------------------------------------------- ");

			string plistPath = pathToBuiltProject + "/Info.Plist";
			Dictionary<string,object> dict = (Dictionary<string,object>)PlistCS.Plist.readPlist (plistPath);

			dict["LSHasLocalizedDisplayName"] = true;
			PlistCS.Plist.writeXml(dict, plistPath);
			#endregion

			string strPermissionName = EditorPrefs.GetString (PermissionDesEditorWindow.StrPermissionName, "");
			Debug.Log (strPermissionName);

			if (strPermissionName.EndsWith (",")) {
				strPermissionName = strPermissionName.Remove (strPermissionName.Length - 1);
			}
			Debug.Log (strPermissionName);

			string[] strPermissionIndexes = strPermissionName.Split (',');
			int[] permissionIndexes = new int[strPermissionIndexes.Length];
			for (int i = 0; i < permissionIndexes.Length; i++) {
				permissionIndexes [i] = int.Parse (strPermissionIndexes [i]);
			}

			#region 生成资源文件 

			// Create a new project object from build target
			XCProject project = new XCProject( pathToBuiltProject );

			var variantGroup = project.variantGroups;
			var rootGroup = project.rootGroup;
			var buildFiles = project.buildFiles;
			var resourcesBuildPhases = project.resourcesBuildPhases;

			var variant = new PBXVariantGroup("InfoPlist.strings", null, "GROUP");
			// mark variants
			variantGroup.Add(variant);
			// add variant to project
			rootGroup.AddChild(variant);

			// add variant in build process

			var appNameConfigData = AppNameConfigData.Load ();

			Dictionary<string,string> language = new Dictionary<string, string> ();
			foreach (var languageData in appNameConfigData.SupportedLanguageItems) {
				language.Add (appNameConfigData.LanguageDefiOS [languageData.Index], languageData.AppName);
			}
			// English  en    french fr

			foreach (KeyValuePair<string,string> entry in language)
			{
				string folder = project.projectRootPath + "/" + entry.Key + ".lproj";
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}
				string filePath = folder + "/InfoPlist.strings";
				string content = "\"CFBundleDisplayName\" = \"" + entry.Value + "\";\n";

				List<PermissionDescription> permissionDescriptions = GetLanguagePermissionDescriptions(entry.Key,permissionIndexes);

				foreach (PermissionDescription permissionDescription in permissionDescriptions)  {
					content += "\""+ permissionDescription.PermissionName  + "\" = \"" + permissionDescription.Description + "\";\n";
				}

				File.WriteAllText(filePath, content);
				var result = project.AddFile(filePath,(PBXGroup)variant,"SOURCE_ROOT",  true,  false);
//				var result = project.AddFile(filePath,"", (PBXGroup)variant,"SOURCE_ROOT",  true,  false);
				string firstKey = "";

				foreach (KeyValuePair<string,object> resultEntry in result) {
					firstKey = resultEntry.Key;
					break;
				}
				PBXFileReference fileReference = (PBXFileReference)result[firstKey];
				fileReference.Remove("name");
				fileReference.Add("name", entry.Key);

				PBXBuildFile buildFile = new PBXBuildFile(fileReference);
				buildFiles.Add(buildFile);
			}

			project.Save();
			#endregion
		}

		public class PermissionDescription {
			public string PermissionName;
			public string Description;
			public PermissionDescription() {}
			public PermissionDescription(string permissionName,string description) {
				PermissionName = permissionName;
				Description = description;
			}
		}

		static List<PermissionDescription> GetLanguagePermissionDescriptions(string language,int[] permissionIndexes) {
			List<PermissionDescription> retList = new List<PermissionDescription> ();

			XmlDocument xmlDoc = new XmlDocument ();
			Debug.Log (language);
			if (File.Exists (Application.dataPath + "/QFramework/QLocalize/Permission/iOSData/" + language + ".xml")) {
				Debug.Log (language + ": exists");
				xmlDoc.Load (Application.dataPath + "/QFramework/QLocalize/Permission/iOSData/" + language + ".xml");
			}
			else {
				Debug.Log (language + ": not exists");
				xmlDoc.Load (Application.dataPath + "/PTUGame/PTLocalize/Permission/iOSData/en.xml");
			}

			XmlNodeList nodeList = xmlDoc.SelectSingleNode ("Config").ChildNodes;
			List<XmlElement> elementList = new List<XmlElement> ();
			foreach (XmlElement element in nodeList) {
				elementList.Add (element);
			}

			for (int i = 0; i < permissionIndexes.Length; i++) {
				XmlElement supportedElement = elementList[permissionIndexes [i]];
				retList.Add(new PermissionDescription(supportedElement.GetAttribute("PermissionKey"),supportedElement.GetAttribute("DescriptionString")));
			}

			return retList;
		}
	}
}