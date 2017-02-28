using UnityEngine;
using System.Collections;
using UnityEditor;

//namespace PTGame.AssetBundles
//{
[CustomEditor(typeof(DefaultAsset))] 
	public class PTCustomAssets:Editor
	{

//
//
//	private Object asset;
//		//在这里方法中就可以绘制面板。
//		public override void OnInspectorGUI() 
//		{
////			//得到Test对象
////			Test test = (Test) target;
////			//绘制一个窗口
////			test.mRectValue = EditoGUILayout.RectField("窗口坐标",
////				test.mRectValue);
////			//绘制一个贴图槽
////			test.texture =  EditorGUILayout.ObjectField("增加一个贴图",test.texture,typeof(Texture),true) as Texture;
////		Debug.Log("ttttt");
//		if (asset == null) {
//			asset = (Object)AssetDatabase.LoadAssetAtPath ("Assets/ArtRes",typeof(Object));
//			Debug.Log (asset);
//			AssetDatabase.SetLabels (asset,new string[]{"kkk1","ggg1"});
//		}
//
//		GUI.enabled = true;
//		   EditorGUILayout.LabelField ("CUSTOM");
//
//		string[] labels = AssetDatabase.GetLabels (asset);
//		if (labels.Length > 0) {
//			Debug.Log (labels[0]);
//		}
//
//			base.OnInspectorGUI ();
//		    
//		}
	}
//}
