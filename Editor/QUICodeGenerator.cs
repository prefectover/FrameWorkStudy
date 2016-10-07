using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class QUICodeGenerator
{
    [MenuItem("Assets/Create UICode")]
    static public void CreateUICode()
    {
		m_Instance.CreateCode(Selection.activeGameObject);
    }

    private void CreateCode(GameObject go)
    {
        if (null != go && go.name.StartsWith("UI"))
        {
            m_SelectGameObject = go;

            PrefabType prefabType = PrefabUtility.GetPrefabType(go);
            if (PrefabType.Prefab != prefabType)
            {
                return;
            }
            GameObject clone = PrefabUtility.InstantiatePrefab(go) as GameObject;
            if (null == clone)
            {
                return;
            }
            m_dicNameToTrans = new Dictionary<string, Transform>();
            FindAllMarkTrans(clone.transform);
            CreateUIComponentsCode();

            GameObject.DestroyImmediate(clone);
        }
    }

    private void FindAllMarkTrans(Transform trans)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform childTrans = trans.GetChild(i);
			QFramework.UI.QUIMark uiMark = childTrans.GetComponent<QFramework.UI.QUIMark>();
            if (null != uiMark)
            {
                if (!m_dicNameToTrans.ContainsKey(childTrans.name))
                {
                    m_dicNameToTrans.Add(childTrans.name, childTrans);
                }
                else
                {
                    Debug.LogError("Repeat key: " + childTrans.name);
                }
            }

            FindAllMarkTrans(childTrans);
        }
    }

    private void CreateUIComponentsCode()
    {
        if (null != m_SelectGameObject)
        {
            string strDlg = m_SelectGameObject.name;
            string strFilePath = string.Format("{0}/{1}.cs", GetScriptsPath(), strDlg);
            if (File.Exists(strFilePath) == false)
            {
                StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
                StringBuilder strBuilder = new StringBuilder();

                strBuilder.AppendLine("using System;");
                strBuilder.AppendLine("using System.Collections.Generic;");
                strBuilder.AppendLine("using UnityEngine;");
                strBuilder.AppendLine("using UnityEngine.UI;");
				strBuilder.AppendLine ("using QFramework;");
				strBuilder.AppendLine("using QFramework.UI;");
				strBuilder.AppendLine("using QFramework.Event;");
				strBuilder.AppendLine("using QFramework.Extend;").AppendLine();

                strBuilder.AppendFormat("public class {0} : QUIBehaviour", strDlg);
                strBuilder.AppendLine();
                strBuilder.AppendLine("{");
				strBuilder.Append("\t").AppendLine("protected override void InitUI(object uiData = null)");
                strBuilder.Append("\t").AppendLine("{");
				strBuilder.Append("\t").Append("\t").AppendLine("mUIComponents = mIComponents as " + strDlg + "Components;");
                strBuilder.Append("\t").Append("\t").AppendLine("//please add init code here");
                strBuilder.Append("\t").AppendLine("}");
				strBuilder.Append("\t").AppendLine("public override void ProcessMsg (QMsg msg)");
				strBuilder.Append("\t").AppendLine("{");
				strBuilder.Append("\t\t").AppendLine("throw new System.NotImplementedException ();");
				strBuilder.Append("\t").AppendLine("}");
                strBuilder.Append("\t").AppendLine("protected override void RegisterUIEvent()");
                strBuilder.Append("\t").AppendLine("{");
                strBuilder.Append("\t").AppendLine("}");
                strBuilder.Append("\t").AppendLine("protected override void OnShow()");
				strBuilder.Append("\t\t").AppendLine("base.OnShow();");
                strBuilder.Append("\t").AppendLine("{");
                strBuilder.Append("\t").AppendLine("}").AppendLine();
                strBuilder.Append("\t").AppendLine("protected override void OnHide()");
				strBuilder.Append("\t\t").AppendLine("base.OnHide();");
                strBuilder.Append("\t").AppendLine("{");
                strBuilder.Append("\t").AppendLine("}").AppendLine();
                strBuilder.Append("\t").AppendLine("void ShowLog(string content)");
                strBuilder.Append("\t").AppendLine("{");
				strBuilder.Append("\t\t").AppendFormat("Debug.Log(\"[ {0}:]\" + content);",strDlg).AppendLine();
                strBuilder.Append("\t").AppendLine("}").AppendLine();

                //CreateUIObjectCode(ref strBuilder); //add properties
				strBuilder.Append("\t").AppendFormat("{0}Components mUIComponents = null;", strDlg).AppendLine();
                strBuilder.Append("}");

                sw.Write(strBuilder);
                sw.Flush();
                sw.Close();
            }

            CreateUIBehaviorCode(strDlg);

            CreateUIFactory();

            AssetDatabase.Refresh();
            Debug.Log("Success Create UIObject Code");
        }
    }

    private void CreateUIFactory()
    {
		string strFilePath = string.Format("{0}/{1}.cs", Application.dataPath + "/QFramework/Script/UI", "QUIFactory");
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.AppendLine("using System;");
        strBuilder.AppendLine("using QFramework;").AppendLine();

		strBuilder.AppendLine("namespace QFramework.UI");
        strBuilder.AppendLine("{");
        strBuilder.Append("\tpublic class QUIFactory");
        strBuilder.AppendLine();
        strBuilder.AppendLine("\t{");
		strBuilder.Append ("\t\t").AppendLine ("private QUIFactory() {}").AppendLine();
		strBuilder.Append ("\t\t").AppendLine ("public static QUIFactory Instance {");
		strBuilder.Append ("\t\t\t").AppendLine ("get {");
		strBuilder.Append ("\t\t\t\t").AppendLine ("return QSingletonComponent<QUIFactory>.Instance;");
		strBuilder.Append ("\t\t\t").AppendLine ("}");
		strBuilder.Append ("\t\t").AppendLine ("}");

		strBuilder.Append ("\t\t").AppendLine ("public static void Dispose()");
		strBuilder.Append ("\t\t").AppendLine ("{");
		strBuilder.Append ("\t\t\t").AppendLine ("QSingletonComponent<QUIFactory>.Dispose ();");
		strBuilder.Append ("\t\t").AppendLine ("}");

        strBuilder.Append("\t\t").AppendLine("public IUIComponents CreateUIComponents(string strUI)");
        strBuilder.Append("\t\t").AppendLine("{");

		strBuilder.Append("\t\t\t").AppendLine("IUIComponents retComponents = null;");
        strBuilder.Append("\t\t\t").AppendLine("switch (strUI)");
        strBuilder.Append("\t\t\t").AppendLine("{");


		QFramework.QIO.CreateDirIfNotExists (GetUIPrefabPath ());

        string[] files = Directory.GetFiles(GetUIPrefabPath(), "UI*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            strBuilder.Append("\t\t\t\t").AppendLine("case \"" + Path.GetFileNameWithoutExtension(files[i]) + "\":");
			strBuilder.Append("\t\t\t\t\t").AppendLine("retComponents = new " + Path.GetFileNameWithoutExtension(files[i]) + "Components();");
            strBuilder.Append("\t\t\t\t\t").AppendLine("break;");
        }

        strBuilder.Append("\t\t\t").AppendLine("}");
		strBuilder.Append("\t\t\t").AppendLine("return retComponents;");
        strBuilder.Append("\t\t").AppendLine("}");

        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

	private void CreateUIBehaviorCode(string behaviourName)
    {
		string strFilePath = string.Format("{0}/{1}.cs", GetScriptsPath(), behaviourName + "Components");
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.AppendLine("using System;");
        strBuilder.AppendLine("using System.Collections.Generic;");
        strBuilder.AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
		strBuilder.AppendLine("using QFramework;");
		strBuilder.AppendLine("using QFramework.UI;");

		strBuilder.AppendFormat("public class {0}Components : IUIComponents", behaviourName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("{");

		strBuilder.Append("\t").AppendLine("public void InitUIComponents()");
        strBuilder.Append("\t").AppendLine("{");
        foreach (KeyValuePair<string, Transform> p in m_dicNameToTrans)
        {
            string strUIType = GetUIType(p.Value);
			strBuilder.AppendFormat("\t\t{0}_{1} = QUGUIMgr.Instance.Get<{2}>(\"{3}\").GetComponent<{4}>();\r\n",
				p.Key,strUIType, behaviourName, p.Key, strUIType);
        }
        strBuilder.Append("\t").AppendLine("}").AppendLine();

        strBuilder.Append("\t").AppendLine("public void Clear()");
        strBuilder.Append("\t").AppendLine("{");
        foreach (KeyValuePair<string, Transform> p in m_dicNameToTrans)
        {
            string strUIType = GetUIType(p.Value);
            strBuilder.AppendFormat("\t\t{0}_{1} = null;\r\n",
				p.Key,strUIType);
        }
        strBuilder.Append("\t").AppendLine("}").AppendLine();

        foreach (KeyValuePair<string, Transform> p in m_dicNameToTrans)
        {
            string strUIType = GetUIType(p.Value);
            strBuilder.AppendFormat("\tpublic {0} {1}_{2};\r\n",
				strUIType,p.Key,strUIType);
        }

        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

    private string GetUIType(Transform trans)
    {
        if (null != trans.GetComponent<Text>())
            return "Text";
        else if (null != trans.GetComponent<Button>())
            return "Button";
        else if (null != trans.GetComponent<Image>())
            return "Image";
        else if (null != trans.GetComponent<RawImage>())
            return "RawImage";
        else if (null != trans.GetComponent<Toggle>())
            return "Toggle";
        else if (null != trans.GetComponent<InputField>())
            return "Input";
        else if (null != trans.GetComponent<Slider>())
            return "Slider";
        else if (null != trans.GetComponent<Scrollbar>())
            return "Scrollbar";
        else
            return "Transform";
    }

    private string GetScriptsPath()
    {
        return Application.dataPath + "/_Script/UI";
    }

    private string GetScriptsTempPath()
    {
        return Application.dataPath + "/../../";
    }

    private string GetUIPrefabPath()
    {
        return Application.dataPath + "/QArt/QAB/UIPrefab";
    }

    private GameObject m_SelectGameObject = null;
    private Dictionary<string, Transform> m_dicNameToTrans = null;
    static private QUICodeGenerator m_Instance = new QUICodeGenerator();
}
