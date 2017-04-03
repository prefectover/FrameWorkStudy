using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using QFramework.Libs;

namespace QFramework.Editor
{
    public class IOPathEditor
    {
        [MenuItem("QFramework/IOPath/Gen Path Asset File")]
        public static void GenPathAssetFile()
        {
			AssetDatabase.SaveAssets ();

			PathConfig data = null;

			IOUtils.CreateDirIfNotExists (EditorPathManager.DefaultPathConfigGenerateForder);

			string newConfigPath = IOEditorPathConfig.IOGeneratorPath + "/NewPathConfig.asset";

			data = AssetDatabase.LoadAssetAtPath<PathConfig>(newConfigPath);
            if (data == null)
            {
				data = ScriptableObject.CreateInstance<PathConfig>();
				AssetDatabase.CreateAsset(data, newConfigPath);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
		}

		[MenuItem("QFramework/IOPath/Gen Path Script")]
		public static void GeneratePathScript() {
			AssetDatabase.SaveAssets ();

			IOUtils.CreateDirIfNotExists (EditorPathManager.DefaultPathScriptGenerateForder);

			string[] fullPathFileNames = Directory.GetFiles(EditorPathManager.DefaultPathConfigGenerateForder, "*PathConfig.asset", SearchOption.AllDirectories);

			foreach(string fullPathFileName in fullPathFileNames) {
				Debug.Log (fullPathFileName);
				if (!fullPathFileName.EndsWith (".meta")) {
					Debug.Log ("gen: " + fullPathFileName);

					PathConfig config = AssetDatabase.LoadAssetAtPath<PathConfig> (fullPathFileName);
					NameSpace nameSpace = new NameSpace ();
					nameSpace.Name = string.IsNullOrEmpty (config.NameSpace) ? "QFramework" : config.NameSpace;
					nameSpace.FileName = config.name + ".cs";
					nameSpace.GenerateDir = string.IsNullOrEmpty (config.ScriptGeneratePath) ? EditorPathManager.DefaultPathScriptGenerateForder : IOUtils.CreateDirIfNotExists ("Assets/" + config.ScriptGeneratePath);
					var classDefine = new ClassDefine ();
					classDefine.Comment = config.Description;
					classDefine.Name = config.name;
					nameSpace.Classes.Add (classDefine);
					Debug.Log (nameSpace.GenerateDir);
					foreach (var pathItem in config.List) {
						if (!string.IsNullOrEmpty(pathItem.Name)) {
							var variable = new Variable (AccessLimit.Private, CompileType.Const, VariableType.String,"m_" + pathItem.Name, pathItem.Path);
							classDefine.Variables.Add (variable);

							var property = new Property (AccessLimit.Public, CompileType.Static, VariableType.String, pathItem.Name, pathItem.PropertyGetCode, pathItem.Description);
							classDefine.Properties.Add (property);
						}
					}
					CodeGenerator.Generate (nameSpace);

					EditorUtility.SetDirty (config);
					Resources.UnloadAsset (config);

				}
					
			}
				
			AssetDatabase.SaveAssets();
		}
    }
}
