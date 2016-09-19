using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Text;
using System;
using QFramework.AB;
using QFramework.Editor;

namespace QFramework.PRIVATE
{
	public class QABItemInfo{

		public string name = "";
		public string absPath = "";
		public QABItemInfo(string name,string absPath){
			this.name = name;
			this.absPath = absPath;
		}
		public string[] assets;

	}
	public class QABBuilder
	{
		public static string overloadedDevelopmentServerURL = "";

		public static void BuildAssetBundles(BuildTarget buildTarget)
		{
			string outputPath = Path.Combine(QPlatform.ABundlesOutputPath,  QPlatform.GetPlatformName());

			if (Directory.Exists (outputPath)) {
				Directory.Delete (outputPath,true);
			}
			Directory.CreateDirectory (outputPath);

			BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.None,buildTarget);

			GenerateVersionConfig (outputPath);
			if(Directory.Exists(Application.streamingAssetsPath+"/QAB")){
				Directory.Delete (Application.streamingAssetsPath+"/QAB",true);
			}
			Directory.CreateDirectory (Application.streamingAssetsPath+"/QAB");
			FileUtil.ReplaceDirectory (QPlatform.ABundlesOutputPath,Application.streamingAssetsPath+"/QAB");
			AssetDatabase.Refresh ();
		}

		private static void GenerateVersionConfig(string outputPath){
			string abManifestFile = Path.Combine (outputPath, QPlatform.GetPlatformName ());
			AssetBundle ab = AssetBundle.LoadFromFile (abManifestFile);
			AssetBundleManifest abMainfest = (AssetBundleManifest)ab.LoadAllAssets () [0];
			string[] allABNames = abMainfest.GetAllAssetBundles ();
			XmlDocument xmlDoc = new XmlDocument ();
			XmlNode xmlRoot = xmlDoc.CreateElement ("config");
			xmlDoc.AppendChild (xmlRoot);
			mABInfos.Clear ();
			for (int i = 0; i < allABNames.Length; i++) {
				Hash128 hash = abMainfest.GetAssetBundleHash (allABNames [i]);
				byte[] fileBytes = QEditorUtil.GetFileBytes (Path.Combine (outputPath, allABNames [i]));

				string md5 = QEditorUtil.MD5(fileBytes);
				string size = QEditorUtil.Size (fileBytes);
				fileBytes = null;

				XmlElement xmlItem = xmlDoc.CreateElement ("item");
				string abName = QABConfigMgr.Instance.markItems4AbsPath [allABNames [i]].name;
				string absPath = allABNames [i];
				xmlItem.SetAttribute ("name", abName);
				xmlItem.SetAttribute ("abspath", absPath);
				xmlItem.SetAttribute ("md5", md5);
				xmlItem.SetAttribute ("size", size);
				xmlRoot.AppendChild (xmlItem);


				AssetBundle assetBundle = AssetBundle.LoadFromFile (Path.Combine (outputPath, allABNames [i]));
				QABItemInfo abInfo= new QABItemInfo (abName,absPath);
				abInfo.assets = assetBundle.GetAllAssetNames ();
				mABInfos.Add (abInfo);

			}
			ab.Unload (true);

			byte[] platformBytes = QEditorUtil.GetFileBytes (Path.Combine (outputPath, QPlatform.GetPlatformName ()));

			string platformMD5 = QEditorUtil.MD5 (platformBytes);
			string platformSize = QEditorUtil.Size (platformBytes);
			platformBytes = null;

			XmlElement platformItem = xmlDoc.CreateElement ("item");
			platformItem.SetAttribute ("name", QPlatform.GetPlatformName());
			platformItem.SetAttribute ("abspath", QPlatform.GetPlatformName());
			platformItem.SetAttribute ("md5", platformMD5);
			platformItem.SetAttribute ("size", platformSize);
			xmlRoot.AppendChild (platformItem);


			xmlDoc.Save (outputPath + "/resitems.xml");

			AssetDatabase.Refresh ();

			if (!Directory.Exists (Application.dataPath +  Path.DirectorySeparatorChar + "QData")) {
				Directory.CreateDirectory (Application.dataPath +  Path.DirectorySeparatorChar + "QData");
			}
			var path = Path.GetFullPath (Application.dataPath + Path.DirectorySeparatorChar + "QData/QAB/QAssets.cs");
			StreamWriter writer = new StreamWriter(File.Open (path, FileMode.Create));
			QABCodeGenerator.WriteClass (writer,"QAB",mABInfos);
			writer.Close ();
			AssetDatabase.Refresh ();
		}

		private static List<QABItemInfo> mABInfos = new List<QABItemInfo>();

		public static void WriteServerURL()
		{
			string downloadURL;
			if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
			{
				downloadURL = overloadedDevelopmentServerURL;
			}
			else
			{
				IPHostEntry host;
				string localIP = "";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIP = ip.ToString();
						break;
					}
				}
				downloadURL = "http://"+localIP+":7888/";
			}

			string assetBundleManagerResourcesDirectory = "Assets/AssetBundleManager/Resources";
			string assetBundleUrlPath = Path.Combine (assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
			Directory.CreateDirectory(assetBundleManagerResourcesDirectory);
			File.WriteAllText(assetBundleUrlPath, downloadURL);
			AssetDatabase.Refresh();
		}

	
			

		static void CopyAssetBundlesTo(string outputPath)
		{
			// Clear streaming assets folder.
			FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
			Directory.CreateDirectory(outputPath);

			string outputFolder = QPlatform.GetPlatformName();

			// Setup the source folder for assetbundles.
			var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, QPlatform.ABundlesOutputPath), outputFolder);
			if (!System.IO.Directory.Exists(source) )
				Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

			// Setup the destination folder for assetbundles.
			var destination = System.IO.Path.Combine(outputPath, outputFolder);
			if (System.IO.Directory.Exists(destination) )
				FileUtil.DeleteFileOrDirectory(destination);

			FileUtil.CopyFileOrDirectory(source, destination);
		}

		static string[] GetLevelsFromBuildSettings()
		{
			List<string> levels = new List<string>();
			for(int i = 0 ; i < EditorBuildSettings.scenes.Length; ++i)
			{
				if (EditorBuildSettings.scenes[i].enabled)
					levels.Add(EditorBuildSettings.scenes[i].path);
			}

			return levels.ToArray();
		}
	}
}