using System.IO;
using Microsoft.CSharp;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using QFramework.Libs;
using QFrameworkAB;

namespace QFramework
{
	public  class QABCodeGenerator
	{
		public static void Generate (string ns, List<AssetBundleInfo> assetBundleInfos,string projectTag=null)
		{
			NameSpace nameSpace = new NameSpace ();
			nameSpace.Name = ns;
			nameSpace.GenerateDir = ABEditorPathConfig.ABCodeGeneratePath;
			nameSpace.FileName = "QAssets.cs";

			for(int i=0;i<assetBundleInfos.Count;i++){
				AssetBundleInfo assetBundleInfo = assetBundleInfos [i];
				string className = assetBundleInfo.name;
				string bundleName = className.Substring (0, 1).ToUpper () + className.Substring (1);
				className = className.Substring(0,1).ToUpper()+className.Substring(1).Replace("/","_").Replace("@","_").Replace("!","_");
				if (!string.IsNullOrEmpty (projectTag)) {
					className = className.Replace ("_project_" + projectTag, "");
					bundleName = bundleName.Replace ("_project_" + projectTag, "");
				}
				className = className.ToUpper ();

				ClassDefine classDefine = new ClassDefine ();
				nameSpace.Classes.Add (classDefine);
				classDefine.Name = className;

				Variable variable = new Variable (AccessLimit.Public,CompileType.Const,VariableType.String,"BUNDLE_NAME",bundleName.ToUpperInvariant ());
				classDefine.Variables.Add (variable);

				foreach (var asset in assetBundleInfo.assets) {
					string content = Path.GetFileNameWithoutExtension (asset).ToUpperInvariant();

					Variable assetVariable = new Variable (AccessLimit.Public,CompileType.Const,VariableType.String,content.Replace("@","_").Replace("!","_"),content);
					classDefine.Variables.Add (assetVariable);
				}
			}

			CodeGenerator.Generate(nameSpace);
		}

	}
}