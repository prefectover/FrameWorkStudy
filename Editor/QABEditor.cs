using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

using QFramework;
using QFramework.Editor;
using QFramework.AB;

namespace QFramework.PRIVATE {
	public class QABEditor
	{

		const string kDevMode = "QFramework/AB/Dev Mode";

		[MenuItem(kDevMode)]
		public static void ToggleSimulationMode ()
		{
			QABMgr.DevABModeInEditor = !QABMgr.DevABModeInEditor;
		}

		[MenuItem(kDevMode, true)]
		public static bool ToggleSimulationModeValidate ()
		{
			Menu.SetChecked(kDevMode, QABMgr.DevABModeInEditor);
			return true;
		}


		[MenuItem("QFramework/AB/Build iOS")]
		public static void BuildABiOS()
		{
			string outputPath = QPath.ABBuildOutPutDir (RuntimePlatform.IPhonePlayer);

			QIO.CreateDirIfNotExists (outputPath);

			QABBuilder.BuildAssetBundles (BuildTarget.iOS);

			AssetDatabase.Refresh ();		
		}

		[MenuItem("QFramework/AB/Build Android")]
		public static void BuildABAndroid()
		{
			string outputPath = QPath.ABBuildOutPutDir (RuntimePlatform.Android);

			QIO.CreateDirIfNotExists (outputPath);

			QABBuilder.BuildAssetBundles (BuildTarget.Android);

			AssetDatabase.Refresh ();
		}

		[MenuItem("QFramework/AB/Build OSX")]
		public static void BuildABOSX()
		{

			string outputPath = QPath.ABBuildOutPutDir (RuntimePlatform.OSXEditor);

			QIO.CreateDirIfNotExists (outputPath);

			QABBuilder.BuildAssetBundles (BuildTarget.StandaloneOSXIntel64);

			AssetDatabase.Refresh ();
		}

		[MenuItem("QFramework/AB/Build Windows")]
		public static void BuildABWindows()
		{
			string outputPath = QPath.ABBuildOutPutDir (RuntimePlatform.WindowsEditor);

			QIO.CreateDirIfNotExists (outputPath);

			QABBuilder.BuildAssetBundles (BuildTarget.StandaloneWindows64);

			AssetDatabase.Refresh ();
		}


		[MenuItem("QFramework/AB/Mark")]
		public static void MarkAssetBundle()
		{
			AssetDatabase.RemoveUnusedAssetBundleNames ();

			string srcAssetPath = QPath.SrcABDir;

			QIO.CreateDirIfNotExists (srcAssetPath);

			DirectoryInfo dir = new DirectoryInfo (srcAssetPath);

			FileSystemInfo[] fileInfos = dir.GetFileSystemInfos ();

			for (int i = 0; i < fileInfos.Length; i++) 
			{
				FileSystemInfo tmpFile = fileInfos[i];

				// TODO:先过滤掉Lua
				if (tmpFile.Name != "Lua" && tmpFile is DirectoryInfo) 
				{
					string tmpPath = Path.Combine (srcAssetPath,tmpFile.Name);

					MarkABDir (tmpPath);
				}
			}

			AssetDatabase.Refresh ();
		}

		/// <summary>
		/// 标记ABDir
		/// </summary>
		public static void MarkABDir(string scenePath)
		{
			Dictionary<string,string> readDict = new Dictionary<string, string> ();

			ChangeHead (scenePath, readDict);

			QABConfigMgr.Instance.Refresh ();

			foreach (string key in readDict.Keys) 
			{
				if (!string.IsNullOrEmpty (key)) {

					QABConfigMgr.Instance.AddItem (readDict[key]);
				}
			}

			QABConfigMgr.Instance.OverrideConfigFile ();
		}
			
		/// <summary>
		/// 截取相对路径
		/// </summary>
		public static void ChangeHead(string fullPath,Dictionary<string,string> theWriter)
		{
			int count = fullPath.IndexOf ("Assets");

			int length = fullPath.Length;

			string replacePath = fullPath.Substring (count);

			DirectoryInfo dir = new DirectoryInfo (fullPath);

			if (null != dir) 
			{
				ListFiles (dir, replacePath, theWriter);
			} 
			else 
			{
				Debug.Log ("this path is null");
			}
		}
			
		public static void ListFiles(FileSystemInfo info, string replacePath,Dictionary<string,string> theWriter)
		{
			if (!info.Exists) {
				Debug.Log (" is not exist");
				return;
			}

			DirectoryInfo dir = info as DirectoryInfo;
			FileSystemInfo[] files = dir.GetFileSystemInfos ();

			for (int i = 0; i < files.Length; i++) {
				FileInfo file = files [i] as FileInfo;

				// 对于文件的操作
				if (file != null) 
				{
					ChangeMark (file, replacePath, theWriter);
				} 
				else 	// 对于目录的操作
				{
					ListFiles (files[i], replacePath, theWriter);
				}
			}
		}

		public static string GetBundlePath(FileInfo file,string replacePath)
		{
			string tmpPath = file.FullName;

			int assetCount = tmpPath.IndexOf (replacePath);

			assetCount += replacePath.Length + 1;

			int nameCount = tmpPath.LastIndexOf (file.Name);

			int tmpCount = replacePath.LastIndexOf ("/");

			string sceneHead = replacePath.Substring (tmpCount + 1, replacePath.Length - tmpCount - 1);

			int tmpLength = nameCount - assetCount;

			if (tmpLength > 0) {
				string substring = tmpPath.Substring (assetCount, tmpPath.Length - assetCount);

				string[] result = substring.Split ("/".ToCharArray ());

				return sceneHead + "/" + result [0];
			} else {
				return sceneHead;
			}

		}

		public static void ChangeAssetMark(FileInfo tmpFile,string markStr,Dictionary<string,string> theWriter)
		{
			string fullPath = tmpFile.FullName;

			int assetCount = fullPath.IndexOf ("Assets");


			string assetPath = fullPath.Substring (assetCount, fullPath.Length - assetCount);


			AssetImporter importer = AssetImporter.GetAtPath (assetPath);

			importer.assetBundleName = markStr;

			string modelName = "";

			string[] subMark = markStr.Split ("/".ToCharArray ());
			if (subMark.Length > 1) {
				modelName = subMark [1];	
			} else {
				modelName = markStr;
			}

			string modelPath = markStr.ToLower ();

			if (!theWriter.ContainsKey (modelName)) {
				theWriter.Add (modelName,modelPath);
			}
		}

		public static void ChangeMark(FileInfo tmpFile,string replacePath,Dictionary<string,string> theWriter)
		{
			if (tmpFile == null ||  tmpFile.Extension == ".meta" || tmpFile.Extension == ".sublime-project" || tmpFile.Extension == ".sublime-workspace" || tmpFile.Extension == ".lua") {
				return;
			}

			string markSr = GetBundlePath (tmpFile, replacePath);

			ChangeAssetMark (tmpFile, markSr, theWriter);
		}
	}
}
