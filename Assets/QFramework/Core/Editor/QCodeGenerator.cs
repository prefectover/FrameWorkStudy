using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 代码生成工具
/// </summary>
namespace QFramework.Editor {
	/// <summary>
	/// 访问权限定义
	/// </summary>
	public enum AccessLimit {
		Public,
		Private,
		Internal,
	}

	/// <summary>
	/// 编译类型
	/// </summary>
	public enum CompileType {
		Const,
		Static,
		Member,
	}

	/// <summary>
	/// 类型
	/// </summary>
	public enum VariableType {
		String,
		Int,
		UInt,
		UShort,
		Short,
		Char,
		Float,
		Double,
	}

	/// <summary>
	/// 变量定义
	/// </summary>
	public class Variable {
		/// <summary>
		/// 访问权限
		/// </summary>
		public AccessLimit AccessLimit;

		/// <summary>
		/// 类型
		/// </summary>
		public CodeTypeReference Type;

		/// <summary>
		/// 编译类型
		/// </summary>
		public CompileType CompileType = CompileType.Member;

		/// <summary>
		/// 变量名
		/// </summary>
		public string Name;

		/// <summary>
		/// 值
		/// </summary>
		public string Value;

		public Variable(AccessLimit accessLimit,CompileType compileType,VariableType type,string name,string value) {
			this.AccessLimit = accessLimit;
			this.CompileType = compileType;
			this.Name = name;
			this.Value = value;

			this.CompileType = compileType;
			switch (type) {
			case VariableType.Char:
				this.Type = new CodeTypeReference (typeof(System.Char));
				break;
			case VariableType.Double:
				this.Type = new CodeTypeReference (typeof(System.Double));
				break;
			case VariableType.Float:
				this.Type = new CodeTypeReference (typeof(System.Decimal));
				break;
			case VariableType.Int:
				this.Type = new CodeTypeReference (typeof(System.Int32));
				break;
			case VariableType.UInt:
				this.Type = new CodeTypeReference (typeof(System.UInt32));
				break;
			case VariableType.Short:
				this.Type = new CodeTypeReference (typeof(System.Int16));
				break;
			case VariableType.UShort:
				this.Type = new CodeTypeReference (typeof(System.UInt16));
				break;
			case VariableType.String:
				this.Type = new CodeTypeReference (typeof(System.String));
				break;
			}
		}
	}

	/// <summary>
	/// 类定义
	/// </summary>
	public class ClassDefine {
		/// <summary>
		/// 命名空间
		/// </summary>
		public string NameSpace;

		/// <summary>
		/// 类名字
		/// </summary>
		public string Name;
		/// <summary>
		/// 变量
		/// </summary>
		public List<Variable> Variables = new List<Variable>();

		/// <summary>
		/// 生成的文件名
		/// </summary>
		public string FileName;

		/// <summary>
		/// 生成的路径
		/// </summary>
		public string GenerateDir;

	}

	public class QCodeGenerator {


		public static void WriteClass(ClassDefine classDefine) {

			if (!Directory.Exists (classDefine.GenerateDir)) {
				Directory.CreateDirectory (classDefine.GenerateDir);
			}

			var compileUnit = new CodeCompileUnit ();
			var codeNameSpace = new CodeNamespace (classDefine.NameSpace);
			compileUnit.Namespaces.Add (codeNameSpace);


			var codeType = new CodeTypeDeclaration (classDefine.Name);
			codeNameSpace.Types.Add (codeType);


			foreach (var variable in classDefine.Variables) {
				CodeMemberField nameField = new CodeMemberField ();

				switch (variable.AccessLimit) {
				case AccessLimit.Public:
					nameField.Attributes = MemberAttributes.Public;
					break;
				case AccessLimit.Private:
					nameField.Attributes = MemberAttributes.Private;
					break;
				}

				switch (variable.CompileType) {
				case CompileType.Const:
					nameField.Attributes |= MemberAttributes.Const;
					break;
				case CompileType.Static:
					nameField.Attributes |= MemberAttributes.Static;
					break;
				}
				nameField.Name = variable.Name;
				nameField.Type = variable.Type;
				nameField.InitExpression = new CodePrimitiveExpression (variable.Value);

				codeType.Members.Add (nameField);
			}

			var provider = new CSharpCodeProvider ();
			var options = new CodeGeneratorOptions ();
			options.BlankLinesBetweenMembers = false;

			StreamWriter writer = new StreamWriter(File.Open (Path.GetFullPath (classDefine.GenerateDir + Path.DirectorySeparatorChar + classDefine.FileName), FileMode.Create));

			provider.GenerateCodeFromCompileUnit (compileUnit, writer, options);
			writer.Close ();
			AssetDatabase.Refresh ();
		}
	}
}