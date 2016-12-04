using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using QFramework;

/// <summary>
/// 外界无法访问的
/// </summary>
namespace QFramework.PRIVATE
{
	/// <summary>
	/// 资源项
	/// </summary>
	public class QABItem
	{
		/// <summary>
		/// ab包名
		/// </summary>
		public string name = ""; 

		/// <summary>
		/// 相对路径
		/// </summary>
		public string abspath = "";

		/// <summary>
		/// 在Project中的路径 assets/aa/bb
		/// </summary>
		public string assetpath = "";

		/// <summary>
		/// 在项目中存在
		/// </summary>
		public bool ExistInProject ()
		{
			if (AssetDatabase.LoadMainAssetAtPath (assetpath) != null) {
				return true;
			} else {
				return false;
			}
		}

		public void Description()
		{
			Debug.LogWarning ("name:" + name + " abspath:" + abspath + " assetpath:" + assetpath);
		}
	}

	/// <summary>
	/// 负责读写配置文件
	/// </summary>
	public class QABConfigMgr : QSingleton<QABConfigMgr>
	{
		public Dictionary<string,QABItem> markItems4AbsPath;

		private const string abDataDirPath = "Assets/QData/QAB";
		private const string srcABDirPath = "Assets/QArt/QAB";
		private string recordFilePath {
			get {
				return abDataDirPath + "/resitems.xml";
			}
		}

		protected QABConfigMgr ()
		{
			if (markItems4AbsPath == null) {
				markItems4AbsPath = new Dictionary<string,QABItem> ();
			} else {
				markItems4AbsPath.Clear ();
			}
			if (!File.Exists (recordFilePath)) {

				//如果不存在，则创建一个默认的路径
				if (!Directory.Exists (abDataDirPath)) {
					Directory.CreateDirectory (abDataDirPath);
				}

				OverrideConfigFile ();
			} 

			// 初始化时候读取配置文件
			TextAsset configFile = AssetDatabase.LoadAssetAtPath<TextAsset> (recordFilePath);
			XmlDocument xmlDoc = new XmlDocument ();
			xmlDoc.LoadXml (configFile.text);
			XmlNodeList nodes = xmlDoc.SelectSingleNode ("config").SelectNodes ("item");
			foreach (XmlNode node in nodes) {
				QABItem markItem = new QABItem ();
				markItem.name = node.Attributes ["name"].Value.ToString ();
				markItem.abspath = node.Attributes ["abspath"].Value.ToString ();
				markItem.assetpath = node.Attributes ["assetpath"].Value.ToString ();
				if (markItem.ExistInProject () && !markItems4AbsPath.ContainsKey(markItem.abspath)) {
					markItems4AbsPath.Add (markItem.abspath,markItem);
				} 
			}
		}

		public void Refresh ()
		{
//			List<string> removedKey = new List<string> ();
//			foreach (string key in markItems4AbsPath.Keys) {
//				if (!markItems4AbsPath [key].ExistInProject ()) {
//					removedKey.Add (key);
//				}
//			}
//
//			foreach (var key in removedKey) {
//				markItems4AbsPath.Remove (key);
//			}
			markItems4AbsPath.Clear();
		}

		public string AddItem (string abspath)
		{
			DirectoryInfo dir = new DirectoryInfo (abspath);

			QABItem item = new QABItem ();
			item.name = dir.Name;
			item.abspath = abspath;
			item.assetpath = srcABDirPath + "/" + abspath;
			item.Description ();
			if (markItems4AbsPath.ContainsKey (item.abspath)) {
				markItems4AbsPath[item.abspath] = item;
			} else {
				markItems4AbsPath.Add (item.abspath, item);
			}
			OverrideConfigFile ();
			return dir.Name;
		}


		public void RemoveItem (string abspath)
		{
			if (markItems4AbsPath.ContainsKey (abspath)) {
				markItems4AbsPath.Remove (abspath);
			}
			OverrideConfigFile ();
		}
			
		public void OverrideConfigFile ()
		{
			XmlDocument xmlDoc = new XmlDocument ();
			XmlNode configElement = xmlDoc.AppendChild (xmlDoc.CreateElement ("config"));
			foreach (string key in markItems4AbsPath.Keys) {
				QABItem item = markItems4AbsPath [key];
				if (item.ExistInProject ()) {
					XmlElement element = xmlDoc.CreateElement ("item");
					element.SetAttribute ("name", item.name);
					element.SetAttribute ("abspath", item.abspath);
					element.SetAttribute ("assetpath", item.assetpath);
					configElement.AppendChild (element);
				}
			}

			xmlDoc.Save (recordFilePath);
			AssetDatabase.Refresh ();
		}
	}
}