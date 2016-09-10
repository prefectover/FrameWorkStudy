using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.PRIVATE
{
	public  class QABCodeGenerator
	{

		public static void WriteClass (TextWriter writer, string ns, List<QABItemInfo> assetBundleInfos)
		{
			var compileUnit = new CodeCompileUnit ();
			var codeNamespace = new CodeNamespace (ns);

			compileUnit.Namespaces.Add (codeNamespace);

			for(int i=0;i<assetBundleInfos.Count;i++){
				QABItemInfo assetBundleInfo = assetBundleInfos [i];
				string className = assetBundleInfo.name.ToUpper(); 

				var codeType = new CodeTypeDeclaration (className);
				codeNamespace.Types.Add (codeType);

				CodeMemberField bundleNameField = new CodeMemberField();
				bundleNameField.Attributes = MemberAttributes.Public|MemberAttributes.Const;
				bundleNameField.Name = "BUNDLENAME";
				bundleNameField.Type = new CodeTypeReference(typeof(System.String));
				codeType.Members.Add(bundleNameField);
				bundleNameField.InitExpression  =new CodePrimitiveExpression(assetBundleInfo.absPath.ToLowerInvariant());

				foreach (var asset in assetBundleInfo.assets) {
					CodeMemberField assetField = new CodeMemberField();
					assetField.Attributes = MemberAttributes.Public|MemberAttributes.Const;

					string content = Path.GetFileNameWithoutExtension (asset).ToUpperInvariant();
					assetField.Name = content;
					assetField.Type = new CodeTypeReference(typeof(System.String));
					codeType.Members.Add(assetField);
					assetField.InitExpression  =new CodePrimitiveExpression(content.ToLowerInvariant());
				}
			}

			var provider = new CSharpCodeProvider ();

			var options = new CodeGeneratorOptions ();

			options.BlankLinesBetweenMembers = false;

			provider.GenerateCodeFromCompileUnit (compileUnit, writer, options);
		}
	}
}