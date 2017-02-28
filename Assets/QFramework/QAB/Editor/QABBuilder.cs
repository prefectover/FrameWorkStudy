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
using QFramework;
using QFramework.AB;
using QFramework.Editor;

using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using QFramework.PRIVATE;
using PTGame.AssetBundles;

namespace QFramework.PRIVATE
{
	public class QABItemInfo{
		public string name = "";
		public string path = "";
		public QABItemInfo(string name,string path){
			this.name = name;
			this.path = path;
		}
		public string[] assets;
	}

	public class QABBuilder
	{

		public static string overloadedDevelopmentServerURL = "";

		private static string projectTag = "qframework";

		public static void SetProjectTag(){

			string projectMark = "_project_";
			var allAbPaths = AssetDatabase.GetAllAssetPaths ();
			foreach (string path in allAbPaths) {
				AssetImporter ai = AssetImporter.GetAtPath (path);
				if (!string.IsNullOrEmpty(ai.assetBundleName)) {
					string abName = ai.assetBundleName;
					int lastIndex = abName.LastIndexOf (projectMark);
					if (lastIndex >= 0) {
						abName = abName.Remove (lastIndex, abName.Length -lastIndex );
					}
					Debug.Log (abName+":abname:"+projectTag+":bbb:"+string.IsNullOrEmpty (projectTag));
					if (string.IsNullOrEmpty (projectTag)) {
						ai.assetBundleName = abName;
					} else {
						ai.assetBundleName = abName + projectMark + projectTag;
					}

				}
				AssetDatabase.SaveAssets ();

			}
		}


		public static void BuildAssetBundles(BuildTarget buildTarget,string inputProjectTag)
		{

			if (string.IsNullOrEmpty (inputProjectTag)) {
				projectTag = "qframework";
			} else {
				projectTag = inputProjectTag;
			}

			SetProjectTag ();

			// Choose the output path according to the build target.
			string outputPath = Path.Combine(QPlatform.ABundlesOutputForderName,  QPlatform.GetPlatformName());
//			outputPath = outputPath + "/" + projectTag;

			if (Directory.Exists (outputPath)) {
				Directory.Delete (outputPath,true);
			}
			Directory.CreateDirectory (outputPath);

			BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.None,buildTarget);

			List<string> finalzips = PackZips (outputPath);
			List<string> finalFiles = PackPTFiles (outputPath);

			GenerateVersionConfig (outputPath, finalzips, finalFiles);

			if(Directory.Exists(QPath.StreamingAssetsABDir)){
				Directory.Delete (QPath.StreamingAssetsABDir,true);
			}
			Directory.CreateDirectory (QPath.StreamingAssetsABDir);
			FileUtil.ReplaceDirectory (QPlatform.ABundlesOutputForderName,Application.streamingAssetsPath+"/QAB");
			AssetDatabase.Refresh ();

			string finalDir = Application.streamingAssetsPath+"/AssetBundles/"+ QPlatform.GetPlatformName()+"/"+projectTag;
			if(Directory.Exists(finalDir)){
				Directory.Delete(finalDir,true);
			}

			Directory.CreateDirectory (finalDir);
			FileUtil.ReplaceDirectory (outputPath,finalDir);
			AssetDatabase.Refresh ();
		}

		private static void GenerateVersionConfig(string outputPath,List<string> finalZips,List<string> finalFiles){
		//		string abManifestFile;

			string abManifestFile = Path.Combine (outputPath, QPlatform.GetPlatformName ());
			if (projectTag != "")
				abManifestFile = Path.Combine (outputPath, projectTag);
			else
				abManifestFile = Path.Combine (outputPath, QPlatform.GetPlatformName ());


			AssetBundle ab = AssetBundle.LoadFromFile (abManifestFile);
			AssetBundleManifest abMainfest = (AssetBundleManifest)ab.LoadAllAssets () [0];
			string[] allABNames = abMainfest.GetAllAssetBundles ();
			XmlDocument xmlDoc = new XmlDocument ();
			XmlElement xmlRoot = xmlDoc.CreateElement ("config");
			xmlRoot.SetAttribute ("res_version", PTAssetBundleBuilder.resVersion);

			xmlDoc.AppendChild (xmlRoot);
			mABInfos.Clear ();
			for (int i = 0; i < allABNames.Length; i++) {
				Hash128 hash = abMainfest.GetAssetBundleHash (allABNames [i]);

				XmlElement xmlItem = CreateConfigItem(xmlDoc,Path.Combine (outputPath, allABNames [i]),allABNames [i],allABNames [i]);
				xmlRoot.AppendChild (xmlItem);

				AssetBundle assetBundle = AssetBundle.LoadFromFile (Path.Combine (outputPath, allABNames [i]));
				QABItemInfo abInfo= new QABItemInfo ( allABNames [i],Path.Combine (outputPath, allABNames [i]));
				abInfo.assets = assetBundle.GetAllAssetNames ();
				mABInfos.Add (abInfo);
				assetBundle.Unload (true);

			}
			// 这里要加上平台相关的xml
			string platformBundleName = QPlatform.GetPlatformName  ();
			XmlElement platformItem ;
			if (projectTag == "") {
				platformItem = CreateConfigItem(xmlDoc,abManifestFile,platformBundleName,platformBundleName);
			} else {
				platformItem = CreateConfigItem(xmlDoc,abManifestFile,projectTag,projectTag);
			}
			xmlRoot.AppendChild (platformItem);

			foreach(var zipPath in finalZips){
				XmlElement zipItem = CreateConfigItem(xmlDoc,zipPath,Path.GetFileName(zipPath),Path.GetFileName(zipPath));
				xmlRoot.AppendChild (zipItem);
			}
			foreach(var filePath in finalFiles){
				XmlElement fileItem = CreateConfigItem(xmlDoc,filePath,Path.GetFileName(filePath),Path.GetFileName(filePath));
				xmlRoot.AppendChild (fileItem);
			}

			ab.Unload (true);

			platformItem = CreateConfigItem (xmlDoc, Path.Combine (outputPath, QPlatform.GetPlatformName ()), QPlatform.GetPlatformName (),QPlatform.GetPlatformName ());

			xmlRoot.AppendChild (platformItem);


			xmlDoc.Save (outputPath + "/resitems.xml");

			AssetDatabase.Refresh ();

			if (PTAssetBundleBuilder.isEnableGenerateClass) {
				if (!Directory.Exists (Application.dataPath +  Path.DirectorySeparatorChar + "QData")) {
					Directory.CreateDirectory (Application.dataPath +  Path.DirectorySeparatorChar + "QData");
				}

				var path = Path.GetFullPath (Application.dataPath + Path.DirectorySeparatorChar + "QData/QAB/QAssets.cs");
				StreamWriter writer = new StreamWriter(File.Open (path, FileMode.Create));
				QABCodeGenerator.WriteClass (writer,"QAB",mABInfos);
				writer.Close ();
				AssetDatabase.Refresh ();
			}
		}
		private static XmlElement CreateConfigItem(XmlDocument xmlDoc,string filePath,string fileName,string finalPath){
			XmlElement platformItem = xmlDoc.CreateElement ("item");
			platformItem.SetAttribute ("name", fileName);
			platformItem.SetAttribute ("path", finalPath);
			byte[] platformFileBytes = QEditorUtil.GetFileBytes(filePath);
			platformItem.SetAttribute ("hash", QEditorUtil.MD5(platformFileBytes));
			platformItem.SetAttribute ("size", QEditorUtil.Size(platformFileBytes));
			return platformItem;

		}

		private static List<QABItemInfo> mABInfos = new List<QABItemInfo>();

		public static void EncryptLua(string srcPath,string toPath)
		{
			DirectoryInfo srcDirInfo = new DirectoryInfo(srcPath);
			DirectoryInfo toDirInfo = new DirectoryInfo(toPath);

			if (toDirInfo.Exists)
			{
				Debug.Log("删除" + toPath);
				toDirInfo.Delete(true);

				Debug.Log("创建" + toPath);
				Directory.CreateDirectory(toPath);
			}

			//复制
			foreach (FileInfo luaFile in srcDirInfo.GetFiles("*.lua", SearchOption.AllDirectories))
			{
				string form = luaFile.FullName;
				string to = "";
				to = form.Replace("\\", "/");
				to = to.Replace(srcPath, toPath);

				string path = Path.GetDirectoryName(to);
				if (!Directory.Exists(path))
				{
					//Debug.LogWarning("创建" + path);
					Directory.CreateDirectory(path);
				}
				File.Copy(form, to, true);
				FileInfo fi = new FileInfo(to);

				if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1) 
				{
					fi.Attributes = FileAttributes.Normal; //去除只读属性
				}          
			}
		}

			//以下是进行lua脚本的加密,（加密:windows版l已OK（luac批处理）,ios的请高手提交代码; 解密:待完善,即让lua requrie函数使用c#函数来执行解密）
			/*
          //luac 
			runLuac();
			*/

			//			if (API.usingEncryptLua)
			//			{
			//				//rc4 lua files
			//				foreach (FileInfo luaFile in toDirInfo.GetFiles("*.lua", SearchOption.AllDirectories))
			//				{
			//					string allPath = luaFile.FullName;
			//					Debug.Log("加密" + allPath);
			//					EncryptFile(allPath, allPath,true); //进行RC4
			//				}
			//			}

			public static  List<string> PackZips(string outpath){
				List<string> finalZipFiles = new List<string>();
				var allDirs = AssetDatabase.GetAllAssetPaths ().Where(path=>(Directory.Exists(path)));
				List<string> zipdirs = new List<string> ();
				foreach(var k in allDirs){
					UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath (k,typeof(UnityEngine.Object));
					if (PTAssetBundleTool.HasPTABLabel (obj, PTAssetBundleTool.LABEL_ZIP)) {
						zipdirs.Add (k);
					}
				}
				foreach( var path in zipdirs){
					string tempDir = "temp_pt";
					if (Directory.Exists (tempDir)) {
						Directory.Delete (tempDir,true);
					}
					Directory.CreateDirectory (tempDir);
					AssetDatabase.Refresh ();

					DirectoryCopy(path,tempDir,true);
					//FileUtil.CopyFileOrDirectory(path,tempDir);
					cleanMeta (tempDir);
					PackFiles(outpath + "/"+Path.GetFileName(path)+".zip", tempDir);
					Directory.Delete (tempDir,true);
					finalZipFiles.Add (outpath + "/"+Path.GetFileName(path)+".zip");
				}

				return finalZipFiles;
			}
			public static List<string> PackPTFiles(	string outpath){

				List<string> finalZipFiles = new List<string>();
				var allFiles = AssetDatabase.GetAllAssetPaths ().Where(path=>(File.Exists(path)));
				List<string> files = new List<string> ();
				foreach(var k in allFiles){
					UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath (k,typeof(UnityEngine.Object));
					if (PTAssetBundleTool.HasPTABLabel (obj, PTAssetBundleTool.LABEL_FILE)) {
						files.Add (k);
						FileUtil.ReplaceFile (k,outpath+"/"+Path.GetFileName(k));
						finalZipFiles.Add (outpath + "/"+Path.GetFileName(k));
					}
				}

				return finalZipFiles;
			}

			//		public static void PackFiles(string outpath)
			//		{
			//			//本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
			//			//StreamingAssets是只读路径，不能写入
			//			//服务器下载：把最后生成的压缩包zip复制出来放到服务器上，客户端用www类进行下载。
			////			string assetPath = Application.dataPath + "/StreamingAssets/";
			////			string srcPath = Application.dataPath + "/Lua/";
			//
			//			if(!Directory.Exists(Application.dataPath+"/app")){
			//				return;
			//			}
			//			string srcPath = Application.dataPath + "/Data/";
			//			   
			//			//复制Lua
			//			EncryptLua(Application.dataPath + "/app/",srcPath + "Lua/app/");
			////			//压缩文件    
			//			//PackFiles(outpath + "/app.zip", srcPath + "/Lua/app/");
			////			if (PTAssetBundleBuilder.isUseFramework) {
			////				EncryptLua(Application.dataPath + "/PTUGame/PTLua/framework/", srcPath + "Lua/framework/");
			////				PackFiles(outpath + "/framework.zip", srcPath + "/Lua/framework/");
			////			}
			//
			//
			////			//清理,meta
			//			cleanMeta(srcPath);
			//
			//
			//			Directory.Delete (srcPath,true);
			//			AssetDatabase.Refresh();
			//
			//			Debug.Log("Data目录压缩打包成功，文件：" + outpath + " data.zip");
			//			Debug.Log("把这个文件上传到web更新服务器目录吧");
			//
			//		}

			public static void PackFiles(string filename, string directory)
			{
				try
				{
					FastZip fz = new FastZip();
					fz.RestoreAttributesOnExtract = false;
					fz.RestoreDateTimeOnExtract = false;
					fz.CreateEmptyDirectories = true;
					fz.CreateZip(filename, directory, true, "");
					fz = null;
				}
				catch (Exception)
				{
					throw;
				}
			}

			public static void cleanMeta(string path)
			{
				string[] names = Directory.GetFiles(path);
				string[] dirs = Directory.GetDirectories(path);
				foreach (string filename in names)
				{
					string ext = Path.GetExtension(filename);
					if (ext.Equals(".meta"))
					{
						File.Delete(@filename);
					}

					foreach (string dir in dirs)
					{
						cleanMeta(dir);
					}
				}
			}

			public static string GetPlatformName()
			{
				return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
			}
			private static string GetPlatformForAssetBundles(BuildTarget target)
			{
				switch(target)
				{
					case BuildTarget.Android:
						return "Android";
					case BuildTarget.iOS:
						return "iOS";
					case BuildTarget.WebGL:
						return "WebGL";
					case BuildTarget.WebPlayer:
						return "WebPlayer";
					case BuildTarget.StandaloneWindows:
					case BuildTarget.StandaloneWindows64:
						return "Windows";
					case BuildTarget.StandaloneOSXIntel:
					case BuildTarget.StandaloneOSXIntel64:
					case BuildTarget.StandaloneOSXUniversal:
						return "OSX";
						// Add more build targets for your own.
						// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
					default:
						return null;
				}
			}

			static void CopyAssetBundlesTo(string outputPath)
			{
				// Clear streaming assets folder.
				FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
				Directory.CreateDirectory(outputPath);

				string outputFolder = GetPlatformName();

				// Setup the source folder for assetbundles.
				var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, PTAssetBundleTool.AssetBundlesOutputPath), outputFolder);
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


			public static byte[] getFileBytes(string filePath)
			{
				FileStream fs = new FileStream(filePath, FileMode.Open);
				int len = (int)fs.Length;
				byte[] data = new byte[len];
				fs.Read(data, 0, len);
				fs.Close();
				return data;
			}

			public static string getMD5(byte[] data)
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] result = md5.ComputeHash(data);
				string fileMD5 = "";
				foreach (byte b in result)
				{
					fileMD5 += Convert.ToString(b, 16);
				}
				return fileMD5;   
				//			return " ";
			}

			public static string getSize(byte[] data)
			{
				return data.Length.ToString();
				//			return 10.ToString();
			}




			private static void GetDirectorys(string strPath, ref List<string> lstDirect)  
			{  
				DirectoryInfo diFliles = new DirectoryInfo(strPath);  
				DirectoryInfo[] diArr = diFliles.GetDirectories();  
				foreach (DirectoryInfo di in diArr)  
				{  
					try  
					{  
						lstDirect.Add(di.FullName);  
						GetDirectorys(di.FullName, ref lstDirect);  
					}  
					catch   
					{  
						continue;  
					}  
				}  
			}  

			private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
			{
				// Get the subdirectories for the specified directory.
				DirectoryInfo dir = new DirectoryInfo(sourceDirName);

				if (!dir.Exists)
				{
					throw new DirectoryNotFoundException(
						"Source directory does not exist or could not be found: "
						+ sourceDirName);
				}

				DirectoryInfo[] dirs = dir.GetDirectories();
				// If the destination directory doesn't exist, create it.
				if (!Directory.Exists(destDirName))
				{
					Directory.CreateDirectory(destDirName);
				}

				// Get the files in the directory and copy them to the new location.
				FileInfo[] files = dir.GetFiles();
				foreach (FileInfo file in files)
				{
					string temppath = Path.Combine(destDirName, file.Name);
					file.CopyTo(temppath, false);
				}

				// If copying subdirectories, copy them and their contents to new location.
				if (copySubDirs)
				{
					foreach (DirectoryInfo subdir in dirs)
					{
						string temppath = Path.Combine(destDirName, subdir.Name);
						DirectoryCopy(subdir.FullName, temppath, copySubDirs);
					}
				}
			}

			//		private static void AddAssetBundlePrefix(string projectTag) {
			//			List<FileInfo> lstFiles = new List<FileInfo>();  
			//			List<string> lstDirect = new List<string>();  
			//			lstDirect.Add(Application.dataPath);  
			//			DirectoryInfo diFliles = null;  
			//			GetDirectorys(Application.dataPath, ref lstDirect);  
			//			foreach (string str in lstDirect)  
			//			{  
			//				try  
			//				{  
			//					string head = "  assetBundleName: ";
			//					string tagString = "_project_";
			//					diFliles = new DirectoryInfo(str);  
			//					foreach(var file in diFliles.GetFiles()) {
			//						
			//						if (file.Extension == ".meta") {
			//							var writer = file.OpenRead();
			//							byte[] bytes = new byte[file.Length];
			//
			//							writer.Read(bytes,0,(int)file.Length);
			//							writer.Close();
			//							writer = file.OpenWrite();
			//
			//
			//							string buffer = System.Text.Encoding.Default.GetString(bytes);
			//
			//
			//							int begin = buffer.IndexOf(head);
			//
			//
			//							if (begin >= 0)
			//							{
			//								string lastString = buffer.Substring(begin);
			//								int end = lastString.IndexOf('\n');
			//								int last = buffer.IndexOf('\n',begin);
			//								string assetBundleName = lastString.Substring(0,end);
			//								if (assetBundleName != head){
			//									assetBundleName = assetBundleName.Substring(head.Length);
			//									//屏蔽回车
			//									if (assetBundleName == "\r"){
			//										continue;
			//									}
			//									int index = assetBundleName.IndexOf(tagString);
			//									if (index >= 0){
			//										if (assetBundleName.StartsWith(projectTag+tagString)){
			//											writer.Close();
			//											continue;
			//										}else{
			//											assetBundleName = assetBundleName.Substring(0, index);
			//										}
			//									}
			//									assetBundleName = head + assetBundleName + tagString + projectTag;
			//									buffer = buffer.Substring(0,begin) + assetBundleName + buffer.Substring(last);
			//									bytes = System.Text.Encoding.Default.GetBytes (buffer);
			//									writer.Write(bytes,0,bytes.Length);
			//								}
			//							}
			//							writer.Close();
			//
			//
			//						}
			//					}
			//				}  
			//				catch   
			//				{  
			//					continue;  
			//				}  
			//			}  
			//			return ;  
			//
			//		}
			//
	}
}

