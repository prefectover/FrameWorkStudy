using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace PTGame.Localize {
	public static class PTLocalizeXCodeProcess 
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
			//修改工程文件 内容
			//string projPath = PBXProject.GetPBXProjectPath(path);
			//PBXProject proj = new PBXProject();
			//proj.ReadFromString(File.ReadAllText(projPath));
			//string target = proj.TargetGuidByName("Unity-iPhone");

			// add extra framework(s) 添加额外的Framework
			//proj.AddFrameworkToProject(target, "AssetsLibrary.framework", false);

			// set code sign identity & provisioning profile 设置证书签名等
			//proj.SetBuildProperty(target, "CODE_SIGN_IDENTITY", "iPhone Distribution: _______________");
			//proj.SetBuildProperty(target, "PROVISIONING_PROFILE", "********-****-****-****-************");

			// rewrite to file
			//File.WriteAllText(projPath, proj.WriteToString());

			//修改Info.plist
			// Get plist

			// Get root
			//读取配置文件
//			string _path = Application.dataPath +
//				"/PTGameData/Editor/config_ptgame/ptconfig_XCodePermission.xml";
//			XmlDocument xmlDoc = new XmlDocument();
//			xmlDoc.Load(_path);
//			//获取Config的所有子节点
//			XmlNodeList nodeList = xmlDoc.SelectSingleNode("Config").ChildNodes;
//			foreach (XmlElement _xmlele in nodeList)
//			{
//				if (_xmlele.GetAttribute("Choose") == "True")
//				{
//					//				rootDict.SetString(_xmlele.GetAttribute("PermissionKey"), _xmlele.GetAttribute("DescriptionString"));
//				}
//			}
			//		PlistElementDict _https = rootDict.CreateDict ("NSAppTransportSecurity");
			//		_https.SetBoolean ("NSAllowsArbitraryLoads",true);
			// Change value of CFBundleDevelopmentRegion in Xcode plist //设置语言为中文
			//rootDict.SetString("CFBundleDevelopmentRegion", "zh_CN");

			//PlistElementArray urlTypes = rootDict.CreateArray("CFBundleURLTypes");

			// add weixin url scheme
			//PlistElementDict wxUrl = urlTypes.AddDict();
			//wxUrl.SetString("CFBundleTypeRole", "Editor");
			//wxUrl.SetString("CFBundleURLName", "weixin");
			//PlistElementArray wxUrlScheme = wxUrl.CreateArray("CFBundleURLSchemes");
			//wxUrlScheme.AddString("____________");

			// Write to file
			//		File.WriteAllText(plistPath, plist.WriteToString());
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
			if (File.Exists (Application.dataPath + "/PTUGame/PTLocalize/Permission/iOSData/" + language + ".xml")) {
				Debug.Log (language + ": exists");
				xmlDoc.Load (Application.dataPath + "/PTUGame/PTLocalize/Permission/iOSData/" + language + ".xml");
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