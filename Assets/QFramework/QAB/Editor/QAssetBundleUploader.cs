using UnityEngine;

using UnityEditor;
using System.Net;
using System.IO;
using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace QFramework
{
	public class QAssetBundleUploader : LoomEditorWindow
	{

		static string dataPath;

		[MenuItem ("PuTaoTool/AssetBundles/AssetBundleUploader")]
		public static void ExecuteAssetBundle ()
		{
			QAssetBundleUploader window = (QAssetBundleUploader)GetWindow (typeof(QAssetBundleUploader), true);
			Debug.Log (Screen.width + " screen width*****");
			window.position = new Rect (100, 100, 500, 400);
			dataPath = Application.dataPath;
			Debug.Log (dataPath+":::::dataPath");
			window.Show ();
		}



		private bool isOnline = false;

		private string gameName = "";
		//		private string localPath = "";
		private string appVersion = "";
		private bool isAndroid = false;
		private bool isIOS = true;


		void OnGUI ()
		{
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("game name");
			gameName = EditorGUILayout.TextField (gameName);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("app version");
			appVersion = EditorGUILayout.TextField (appVersion);
			GUILayout.EndHorizontal ();

//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("local path");
//			localPath = EditorGUILayout.TextField (localPath);
//			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			bool result1 = GUILayout.Toggle (isAndroid, "Android");
			bool result2 = GUILayout.Toggle (isIOS, "iOS");
			if (result1 != isAndroid) {
				if (result1) {
					isAndroid = true;
					isIOS = false;
				} else {
					isAndroid = false;
					isIOS = true;
				}
			} else if (result2 != isIOS) {
				if (result2) {
					isAndroid = false;
					isIOS = true;
				} else {
					isAndroid = true;
					isIOS = false;
				}
			}

			GUILayout.EndHorizontal ();

			//isOnline = GUILayout.Toggle (isOnline, "online");

			if (GUILayout.Button ("Upload")) {
				this.RunAsync (Upload);
		
			}
			GUILayout.EndVertical ();

		}



		public class FTPLoginInfo
		{
			public string hostName;
			public string userName;
			public string password;
			public string port;
			public string remoteRefPath;
		}

		private FTPLoginInfo loginInfoInner = new FTPLoginInfo () {
//			hostName = @"10.1.11.29",
//			port = "22",
//			userName = "root",
//			password = "putao123",
//			remoteRefPath = "/data/web/static.putao.com/source/"

			hostName = @"ftp://10.1.11.39",
			port = "22",
			userName = "wanzhengyu",
			password = "game@putao.com",
			remoteRefPath = ""
		};
		private FTPLoginInfo loginInfoOnline = new FTPLoginInfo () {
			hostName = @"ftp://183.131.150.200",
			userName = "game",
			password = "az15eXdxSl7jE",
			remoteRefPath = ""
		};

		public FTPLoginInfo GetLoginInfo (bool isOnline)
		{
			if (isOnline) {
				return loginInfoOnline;
			}
			return loginInfoInner;
		}

		private const string key_assetbundle_uploader_gamename = "key_assetbundle_uploader_gamename";
		private const string key_assetbundle_uploader_localpath = "key_assetbundle_uploader_localpath";
		private const string key_assetbundle_uploader_appversion = "key_assetbundle_uploader_appversion";
		private const string key_assetbundle_uploader_isios = "key_assetbundle_uploader_isios";

		public void OnDisable ()
		{

			EditorPrefs.SetString (key_assetbundle_uploader_gamename, gameName);
//			EditorPrefs.SetString (key_assetbundle_uploader_localpath, localPath);
			EditorPrefs.SetString (key_assetbundle_uploader_appversion, appVersion);
			EditorPrefs.SetBool (key_assetbundle_uploader_isios, isIOS);
			Debug.Log ("disable *******");
			dataPath = Application.dataPath;
		}

		public void OnEnable ()
		{

			gameName = EditorPrefs.GetString (key_assetbundle_uploader_gamename, "");
//			localPath = EditorPrefs.GetString (key_assetbundle_uploader_localpath, "");
			appVersion = EditorPrefs.GetString (key_assetbundle_uploader_appversion, "");
			isIOS = EditorPrefs.GetBool (key_assetbundle_uploader_isios, true);
			isAndroid = !isIOS;

			dataPath = Application.dataPath;


		}


		public static void  ListFiles (FileSystemInfo info, ref List<string> dirs, ref List<string> fls)
		{
			if (!info.Exists)
				return;

			DirectoryInfo dir = info   as   DirectoryInfo; 
			if (dir == null)
				return;
			Debug.Log (dir.FullName + ":dir****");
			dirs.Add (dir.FullName);
			FileSystemInfo[] files = dir.GetFileSystemInfos (); 
			for (int i = 0; i < files.Length; i++) { 
				FileInfo file = files [i]   as   FileInfo; 
				if (file != null) {
					if (file.Extension != ".meta") {
						Debug.Log (file.FullName); 
						fls.Add (file.FullName);
					}

				} else {
					ListFiles (files [i],ref dirs,ref fls);
				} 
			} 
		}


		private string GetRemoteResVersionDir ()
		{
			return "version_" + appVersion.Replace (".", "");
		}



		private void Upload ()
		{
		
			if (string.IsNullOrEmpty (appVersion)) {
				this.QueueOnMainThread (() => {
					EditorUtility.DisplayDialog ("error", "please input appversion", "ok");
				});
				return;
			}

			if (string.IsNullOrEmpty (gameName)) {
				this.QueueOnMainThread (() => {
					EditorUtility.DisplayDialog ("error", "please input game name", "ok");
				});
				return;
			}

			FTPLoginInfo loginInfo = GetLoginInfo (isOnline);

			IFTPInterface sftpHelper;
			if (isOnline) {
				sftpHelper = new FTPClient (loginInfo.hostName, loginInfo.userName, loginInfo.password);

			} else {
//				sftpHelper = new SFTPHelper (loginInfo.hostName, loginInfo.port, loginInfo.userName, loginInfo.password);
				sftpHelper = new FTPClient (loginInfo.hostName, loginInfo.userName, loginInfo.password);

			}

			this.QueueOnMainThread (() => {
				EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "conneting to  ftp server ...", 0);
			});
	
			bool result = sftpHelper.Connect ();
			if (!result) {
				this.QueueOnMainThread (() => {
					EditorUtility.ClearProgressBar ();
					EditorUtility.DisplayDialog ("error", " connect failed \n please check your network", "ok");
				});
				return;
			}



			this.QueueOnMainThread (() => {
				EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "checking server content...", 0);
			});
			string remoteGameDir = loginInfo.remoteRefPath + this.gameName;
			string destRemoteDir = remoteGameDir + "/" + GetRemoteResVersionDir ();
			if (sftpHelper.Exist (remoteGameDir, GetRemoteResVersionDir ())) {

				string timestamp = DateTime.Now.ToString ("yyyy-MM-dd-HH-mm");
				sftpHelper.Rename (destRemoteDir, destRemoteDir + "_" + timestamp);
			} 
			this.QueueOnMainThread (() => {
				
				EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "checking server finish ...", 0);
			});

			sftpHelper.MakeDir (destRemoteDir);
			string abDirectory = dataPath+"/StreamingAssets/AssetBundles/iOS";
			if (!isIOS) {
				abDirectory = dataPath+"/StreamingAssets/AssetBundles/Android";
			} 

			if (!Directory.Exists (abDirectory)) {
				this.QueueOnMainThread (() => {
					EditorUtility.ClearProgressBar ();
					EditorUtility.DisplayDialog ("error", "can not find local assetbundles", "ok");
				});

				return;
			}
			this.QueueOnMainThread (() => {
				EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "start upload content...", 0);
			});



			DirectoryInfo theFolder = new DirectoryInfo (abDirectory);
			Debug.Log (theFolder.FullName + ":HHHHHHH");

			List<string> dirs = new List<string> ();
			List<string> files = new List<string> ();
			ListFiles (theFolder, ref dirs, ref files);

			string premark = abDirectory+"/";
			Debug.Log (premark+":premark>>>>>>>>>");
			string failedFile=null;
			int totalCount = dirs.Count + files.Count;
			int counter = 0;
			foreach(string dir in dirs){
				if (dir.Length > premark.Length) {
					string finalDir = dir.Remove (0, premark.Length);
					result =sftpHelper.MakeDir (destRemoteDir + "/" +finalDir);
					counter++;
					this.QueueOnMainThread (() => {
						EditorUtility.DisplayProgressBar ("create directory on ftp server", "complete " + counter + "/" + totalCount, counter * 1.0f / totalCount);
					});
					if (!result) {
						failedFile = finalDir;
							break;
					}
				}
			}
			foreach (string filePath in files) {
				if(filePath.Length>premark.Length){
					string finalFile = filePath.Remove (0, premark.Length);
					result = sftpHelper.Upload (filePath, destRemoteDir + "/" + finalFile);
					counter++;
					this.QueueOnMainThread (() => {
						EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "complete " + counter + "/" + totalCount, counter * 1.0f / totalCount);
					});
					if (!result) {
						failedFile = finalFile;
						break;
					}
				}
			}



//			string[] localFiles = Directory.GetFiles (abDirectory, "*").Where (t => {
//				if (t.ToLower ().EndsWith (".meta")||t.ToLower().EndsWith(".manifest")) {
//					return false;
//				}
//				return true;
//			}).ToArray ();
//
//			int count = 0;
//			string failedFile = null;
//			foreach (var file in localFiles) {
//				this.QueueOnMainThread (() => {
//					EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "complete " + count + "/" + localFiles.Length, count * 1.0f / localFiles.Length);
//				});
//
//				Debug.Log ("******upload file :" + Path.GetFileName (file));
//				result = sftpHelper.Upload (file, destRemoteDir + "/" + Path.GetFileName (file));
//				Debug.Log ("******uplaod result is : " + result);
//				if (!result) {
//					failedFile = file;
//					break;
//				}
//				count++;
//			}

			sftpHelper.Disconnect ();

			this.QueueOnMainThread (() => {
				EditorUtility.ClearProgressBar ();
			});


//			if (string.IsNullOrEmpty (failedFile)) {
//				this.QueueOnMainThread (() => {
//					EditorUtility.DisplayDialog ("success", "upload assetbundles success!", "ok");
//				});
//
//			} else {
//				this.QueueOnMainThread (() => {
//					EditorUtility.DisplayDialog ("error", "upload failed :" + Path.GetFileName (failedFile), "ok");
//				});
//			}
		}

	}
}
