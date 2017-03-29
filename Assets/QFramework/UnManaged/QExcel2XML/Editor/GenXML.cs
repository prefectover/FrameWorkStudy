using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;
using System.Xml;

/// <summary>
/// Gen XM.
/// </summary>
[InitializeOnLoad]
public class GenXML :Editor {

	[MenuItem("PuTaoTool/Localize/GenXML")]
	static void ExcelGenXML() {


		FileStream stream = File.Open(Application.dataPath + "/PTUGame/PTExcel2XML/word.xlsx", FileMode.Open, FileAccess.Read);
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

		DataSet result = excelReader.AsDataSet();

		int columns = result.Tables[0].Columns.Count;
		int rows = result.Tables[0].Rows.Count;


		List<string> keys = new List<string> ();


		// 第一行读取
		for (int j = 0; j < columns; j++) {
			string  nvalue  = result.Tables[0].Rows[0][j].ToString();
			keys.Add (nvalue);
		}


		Dictionary<string,Dictionary<string,string>> readItemDict = new Dictionary<string, Dictionary<string, string>>();


		// 第二行之后
		for(int i = 1;  i< rows; i++)
		{
			Dictionary<string,string> textItem = new Dictionary<string,string> ();
			for(int j =0; j < columns; j++)
			{
				string  nvalue  = result.Tables[0].Rows[i][j].ToString();
				Debug.Log(nvalue);
				textItem.Add (keys [j], nvalue);
			}
			readItemDict.Add (textItem ["key"], textItem);
		}	

		WriteXML (readItemDict);
	}


	static void WriteXML(Dictionary<string,Dictionary<string,string>> readItemDict) {

		XmlDocument xmlDoc = new XmlDocument ();


		XmlElement rootNode = xmlDoc.CreateElement ("words");
		xmlDoc.AppendChild (rootNode);

		foreach (KeyValuePair<string,Dictionary<string,string>> textItem in readItemDict) {
			XmlElement wordNode = xmlDoc.CreateElement ("word");

			foreach(KeyValuePair<string,string> attributeItem in textItem.Value) {
				var attribute = xmlDoc.CreateAttribute (attributeItem.Key);
				attribute.Value = attributeItem.Value.ToString ();

				wordNode.Attributes.Append (attribute);
			}

			rootNode.AppendChild (wordNode);
		}

		xmlDoc.Save(Application.dataPath + "/ArtRes/ResourceAssetsBundle/configFile/word.xml");
		
	}

	const string compilingKey = "Compiling";
	static bool compiling;
	static int mBeginSecond = 0;
	static GenXML()
	{
		compiling = EditorPrefs.GetBool(compilingKey, false);
		EditorApplication.update += Update;
	}

	static void Update()
	{

		
//		Debug.Log ("1");
//
//		var e = Event.current;
//		Debug.Log (e);
//		Debug.Log (Input.anyKey);
//		if (e != null && e.keyCode == KeyCode.LeftCommand && e.keyCode == KeyCode.G) {
//			Debug.Log ("asd");
//		}
//
//		if(compiling && !EditorApplication.isCompiling)
//		{
//			Debug.Log(string.Format("Compiling DONE {0}", DateTime.Now));
//			//			Debug.Log(string.Format("Compiling Time {0} Seconds",DateTime.Now.Second - mBeginSecond));
//			compiling = false;
//			EditorPrefs.SetBool(compilingKey, false);
//		}
//		else if (!compiling && EditorApplication.isCompiling)
//		{
//			Debug.Log(string.Format("Compiling START {0}", DateTime.Now));
//			//			mBeginSecond = DateTime.Now.Second;
//			compiling = true;
//			EditorPrefs.SetBool(compilingKey, true);
//		}
	}
}