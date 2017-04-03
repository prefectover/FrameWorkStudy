using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace QFrameworkAB
{
	public  class QBundleInfoGenerator
	{

		public static void WriteClass (TextWriter writer, string ns, List<AssetBundleInfo> assetBundleInfos,string projectTag=null)
		{

			var compileUnit = new CodeCompileUnit ();
			var codeNamespace = new CodeNamespace (ns);
			compileUnit.Namespaces.Add (codeNamespace);

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

				var codeType = new CodeTypeDeclaration (className);
				codeNamespace.Types.Add (codeType);

				CodeMemberField bundleNameField = new CodeMemberField();
				bundleNameField.Attributes = MemberAttributes.Public|MemberAttributes.Const;
				bundleNameField.Name = "BUNDLE_NAME";
				bundleNameField.Type = new CodeTypeReference(typeof(System.String));
				codeType.Members.Add(bundleNameField);
				bundleNameField.InitExpression  =new CodePrimitiveExpression(bundleName.ToUpperInvariant());

				foreach (var asset in assetBundleInfo.assets) {
					CodeMemberField assetField = new CodeMemberField();
					assetField.Attributes = MemberAttributes.Public|MemberAttributes.Const;

					string content = Path.GetFileNameWithoutExtension (asset).ToUpperInvariant();
					assetField.Name = content.Replace("@","_").Replace("!","_");
					assetField.Type = new CodeTypeReference(typeof(System.String));
					codeType.Members.Add(assetField);
					assetField.InitExpression  =new CodePrimitiveExpression(content);
				}
			}

			var provider = new CSharpCodeProvider ();
			var options = new CodeGeneratorOptions ();
			options.BlankLinesBetweenMembers = false;

			provider.GenerateCodeFromCompileUnit (compileUnit, writer, options);

		}


	}
}