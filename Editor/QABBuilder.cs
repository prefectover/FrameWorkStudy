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
		public static void BuildAssetBundles(BuildTarget buildTarget)
		{
			string outputPath = Path.Combine(QPlatform.ABundlesOutputForderName,  QPlatform.GetPlatformName());

			if (Directory.Exists (outputPath)) {
				Directory.Delete (outputPath,true);
			}
			Directory.CreateDirectory (outputPath);

			BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.None,buildTarget);

			GenerateVersionConfig (outputPath);
			if(Directory.Exists(QPath.StreamingAssetsABDir)){
				Directory.Delete (QPath.StreamingAssetsABDir,true);
			}
			Directory.CreateDirectory (QPath.StreamingAssetsABDir);
			FileUtil.ReplaceDirectory (QPlatform.ABundlesOutputForderName,Application.streamingAssetsPath+"/QAB");
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
				assetBundle.Unload (true);

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
	}
}