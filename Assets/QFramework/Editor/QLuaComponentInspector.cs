using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using QFramework;

[CustomEditor(typeof(QLuaComponent))]
public class QLuaComponentInspectorEditor : Editor
{
	private SerializedObject obj;
//	private SerializedProperty luaPath = null;

	// 添加TestInspector组件的GameObject被选中时触发该函数
	void OnEnable()
	{
		obj = new SerializedObject(target);
//		luaPath = obj.FindProperty("LuaPath");
	}

	// 重写Inspector检视面板
	public override void OnInspectorGUI()
	{
//		obj.Update();
//		DrawDefaultInspector();
		QLuaComponent myTarget = (QLuaComponent)target;

//		obj.Update();
//		obj.Update();
//		string path = luaPath.stringValue;

		Rect sfxPathRect = EditorGUILayout.GetControlRect();


		string path = myTarget.LuaPath;
		path = (path != null ? path : "");

		string sfxPathText = EditorGUI.TextArea (sfxPathRect, path);
		if (
			(UnityEngine.Event.current.type == EventType.DragUpdated )
			&& sfxPathRect.Contains(UnityEngine.Event.current.mousePosition)
		) 
		{  
			//改变鼠标的外表  
			DragAndDrop.visualMode = DragAndDropVisualMode.Generic;  
			if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)  
			{  
				if (DragAndDrop.paths [0] != "") {
//				string path = DragAndDrop.paths[0];  
					string newpath = DragAndDrop.paths[0];
					string[] resultString = Regex.Split (newpath, "/Lua/", RegexOptions.IgnoreCase);
					newpath = resultString[1];

					newpath = newpath.Replace (".lua", "");

					newpath = newpath.Replace ("/", ".");
					myTarget.LuaPath = newpath; 
					AssetDatabase.SaveAssets ();
					Debug.Log (obj.ApplyModifiedProperties ());
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());
				}
			}  
		}  
//		obj.Update();
//		EditorGUILayout.PropertyField(luaPath);
//		obj.ApplyModifiedProperties ();
//		luaPath.stringValue = path;

//		if (obj.ApplyModifiedProperties ()) {
//			AssetDatabase.SaveAssets ();
//			return;
//		}
//		obj.Update();


	}
}
