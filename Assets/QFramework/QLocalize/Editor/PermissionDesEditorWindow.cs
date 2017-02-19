using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif

namespace PTGame.Localize
{    
    /// <summary>
    /// 对应xml中文件条目
    /// </summary>
    public class PTPermissionData
    {
        /// <summary>
        /// 权限下标
        /// </summary>
        public string permision_index;
        /// <summary>
        /// 权限名字
        /// </summary>
        public string permision_name;
        /// <summary>
        /// 权限描述
        /// </summary>
        public string permision_desc;

        public PTPermissionData() { }
        public PTPermissionData(string index,string name,string desc) 
        {
            this.permision_index = index;
            this.permision_name = name;
            this.permision_desc = desc;
        }
    }

    /// <summary>
    /// 组合选择信息
    /// </summary>
    public class PTChooseData
    {
        public bool choose;
        public PTPermissionData data;

        public PTChooseData() { }
        public PTChooseData(bool choose,PTPermissionData data)
        {
            this.choose = choose;
            this.data = data;
        }
    }

    public class PermissionDesEditorWindow:EditorWindow
    {
        public string xmlPermissionPath = string.Empty;
        /// <summary>
        /// 常量字符
        /// </summary>
        public const string StrIndex = "Index";
        public const string StrPermission = "PermissionKey";
        public const string StrDescription = "DescriptionString";
        public const string StrPermissionName = "PermissionChoose";
        /// <summary>
        /// xml
        /// </summary>
        private XmlDocument xmlPresmissionDoc = null;
        //private XmlDocument xmlChooseDoc = null;

        /// <summary>
        /// 现在选择的index值
        /// </summary>
        private List<string> usingIndexs = null;
        /// <summary>
        /// 组合选择和具体内容
        /// </summary>
        private List<PTChooseData> chooseDatas = null;

        /// <summary>
        /// 某个文件的权限描述
        /// </summary>
        //private Dictionary<string, PTPermissionData> permisionDatas = null;

        /// <summary>
        /// 便于xml查询节点
        /// </summary>
        private Dictionary<string, XmlElement> allPermissions = null;

        /// <summary>
        /// 所有语言文件名
        /// </summary>
        private string[] allLanguageFileFullName = null;
        private string[] allLanguageFileSingleName = null;
        /// <summary>
        /// 当前选择的语言
        /// </summary>
        private int CurChooseLanguageIndex = 0;

        /// <summary>
        /// 创建配置窗口
        /// </summary>
		[MenuItem("PuTaoTool/Localize/Permission")]
        public static void CreateWindow()
        {
			PermissionDesEditorWindow window = (PermissionDesEditorWindow)EditorWindow.GetWindow(typeof(PermissionDesEditorWindow),true);
            window.titleContent = new  GUIContent("Config PermissionDes");
			window.Show();
			window.xmlPermissionPath = Application.dataPath + "/PTUGame/PTLocalize/Permission/iOSData";

            window.init();
        }
        /// <summary>
        /// 生成界面
        /// </summary>
        public void init()
        {
            this.initLanguageEnumPopup();

            xmlPresmissionDoc = new XmlDocument();
            //xmlChooseDoc = new XmlDocument();

            this.ReadChooseData();
            this.ReadXmlData();
        }        
        
        /// <summary>
        /// 初始化语言选项
        /// </summary>
        private void initLanguageEnumPopup()
        {
            string _base = this.xmlPermissionPath;
            this.allLanguageFileFullName = Directory.GetFiles(_base, "*.xml");
            this.allLanguageFileSingleName = new string[this.allLanguageFileFullName.Length];
            for (int i = 0; i < this.allLanguageFileFullName.Length; i++)
            {
                string _temp = this.allLanguageFileFullName[i];
                //去掉后缀，和目录信息
                this.allLanguageFileSingleName[i] = _temp.Substring(_base.Length + 1).Split('.')[0];
                //去掉\换为/
                this.allLanguageFileFullName[i] = _base + "/" + this.allLanguageFileSingleName[i] + ".xml";
            }
        }

        /// <summary>
        /// 读取选择到信息
        /// </summary>
        private void ReadChooseData()
        {
            if (this.usingIndexs == null)
            {
                this.usingIndexs = new List<string>();
            }
            this.CurChooseLanguageIndex = 0;
            string[] permissionChoose = UnityEditor.EditorPrefs.GetString("PermissionChoose").Split(',');
            for (int i = 0; i < permissionChoose.Length; i++)
            {
                this.usingIndexs.Add(permissionChoose[i]);
            }
        }

        /// <summary>
        /// 读取XML数据
        /// </summary>
        private void ReadXmlData()
        {
            //清理原有的
            xmlPresmissionDoc.RemoveAll();
            //根据路径将XML读取出来
            xmlPresmissionDoc.Load(this.allLanguageFileFullName[this.CurChooseLanguageIndex]);

            if (this.chooseDatas == null)
            {
                this.chooseDatas = new List<PTChooseData>();
                this.allPermissions = new Dictionary<string, XmlElement>();
            }
            else
            {
                this.chooseDatas.Clear();
                this.allPermissions.Clear();
            }
            //获取Config的所有子节点
            XmlNodeList nodeList = xmlPresmissionDoc.SelectSingleNode("Config").ChildNodes;
            foreach (XmlElement _xmlele in nodeList)
            {
                this.ShowAConfig(_xmlele, _xmlele.GetAttribute(StrIndex), _xmlele.GetAttribute(StrPermission), _xmlele.GetAttribute(StrDescription));
            }
        }
        /// <summary>
        /// 创建一个复选框+权限描述的条目
        /// </summary>
        private void ShowAConfig(XmlElement _element, string index, string _key, string _value)
        {
            this.chooseDatas.Add(new PTChooseData(this.usingIndexs.Contains(index), new PTPermissionData(index, _key, _value)));
            this.allPermissions.Add(_key, _element);
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        private void SaveChange()
        {
            string choosePermission = string.Empty;
            for (int i = 0; i < chooseDatas.Count; i++)
            {
                if (i != chooseDatas.Count-1)
                {
                    if (this.chooseDatas[i].choose)
                    {
                        choosePermission += (this.chooseDatas[i].data.permision_index + ",");
                    }
                }
                else
                {
                    if (this.chooseDatas[i].choose)
                    {
                        choosePermission += this.chooseDatas[i].data.permision_index;
                    }
                }
            }

            UnityEditor.EditorPrefs.SetString("PermissionChoose", choosePermission);
        }

        private void OnGUI()
        {
//            return;
            EditorGUILayout.LabelField("选择要申请的权限，默认全部申请");
            for (int i = 0; i < chooseDatas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                bool _oldChoose = this.chooseDatas[i].choose;
                this.chooseDatas[i].choose = EditorGUILayout.Toggle(this.chooseDatas[i].choose, GUILayout.Width(10));
                if (_oldChoose != this.chooseDatas[i].choose)
                {
                    Debug.Log("Now status is = " + this.chooseDatas[i].choose);
                    //这里把消息发出去
                }
                EditorGUILayout.LabelField(this.chooseDatas[i].data.permision_name);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("全选",GUILayout.Width(45)))
            {
                for (int i = 0; i < chooseDatas.Count; i++)
                {
                    this.chooseDatas[i].choose = true;
                }
            }
            if (GUILayout.Button("全不选", GUILayout.Width(45) ))
            {
                for (int i = 0; i < chooseDatas.Count; i++)
                {
                    this.chooseDatas[i].choose = false;
                }
            }
//             EditorGUILayout.Space();
//             EditorGUILayout.LabelField("这里选择语言");
//             int _oldChooseLanguage = this.CurChooseLanguageIndex;
//             this.CurChooseLanguageIndex = EditorGUILayout.Popup(this.CurChooseLanguageIndex, this.allLanguageFileSingleName);
//             if (_oldChooseLanguage!= this.CurChooseLanguageIndex)
//             {
//                 this.ChangeLanguageChoose(this.CurChooseLanguageIndex);
//             }
//             EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("保存配置", GUILayout.Width(65)))
            {
                this.SaveChange();
            }
        }

        /// <summary>
        /// 切换语言选项
        /// </summary>
        /// <param name="_index"></param>
        private void ChangeLanguageChoose(int _index)
        {
            this.ReadXmlData();
        }

        void OnDestroy()
        {
        }
    }
}