using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using QFramework.Libs;

namespace QFramework.Editor
{
    public class IOEditor
    {
        [MenuItem("QFramework/IO/Gen Path File")]
        public static void BuildAppConfig()
        {
			EditorUtils.GetSelectedDirAssetsPath();
			PathConfig data = null;
			string ioFolderPath =  "Assets/QFrameworkData/IO";

			IOUtils.CreateDirIfNotExists (ioFolderPath);

			PathConfig pathConfig = AssetDatabase.LoadAssetAtPath<PathConfig>(ioFolderPath + "/IOEditorPathConfig.asset");

			Debug.Log (pathConfig ["IOGeneratorPath"].Path);

			string newConfigPath = "Assets/" + pathConfig ["IOGeneratorPath"].Path + "/NewPathConfig.asset";

			Resources.UnloadAsset (pathConfig);

			data = AssetDatabase.LoadAssetAtPath<PathConfig>(newConfigPath);
            if (data == null)
            {
				data = ScriptableObject.CreateInstance<PathConfig>();
				AssetDatabase.CreateAsset(data, newConfigPath);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
		}
    }
}
