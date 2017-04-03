using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using QFramework;
using QFramework.Libs;

namespace QFramework.Editor
{
    public class AppConfigEditor
    {
        [MenuItem("Assets/QFrameork/Build GenPathConfig")]
        public static void BuildAppConfig()
        {

            PathDicionary data = null;
            string folderPath = EditorUtils.GetSelectedDirAssetsPath();
            string spriteDataPath = folderPath + "Path.asset";

            data = AssetDatabase.LoadAssetAtPath<PathDicionary>(spriteDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<PathDicionary>();
                AssetDatabase.CreateAsset(data, spriteDataPath);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
