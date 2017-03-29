using UnityEngine;
using System.Collections;
using QFramework;

namespace QFramework {
	public class QConfigValue {
		string mValue;
		public int IntValue {
			get {
				return int.Parse (mValue);
			}
		}
		public float FloatValue {
			get {
				return float.Parse (mValue);
			}
		}
		public string StrValue {
			get {
				return mValue;
			}
		}

		public QConfigValue(string value) {
			mValue = value;
		}
	}
	/// <summary>
	/// 专门支持配置文件的管理器 只支持二级KeyValue类型的。
	/// TODO1.热加载
	/// </summary>
	public class QConfigManager : QMgrBehaviour {

		public static QConfigManager Instance {
			get {
				return QMonoSingletonComponent<QConfigManager>.Instance;
			}
		}
			
		public void LoadXML(string tableName,string xmlContent) {

		}

		public QConfigValue GetValue(string tableName,string itemKey,string attributeName) {
			
			return new QConfigValue("1");
		}

		protected override void SetupMgrId ()
		{
			
		}

		protected override void SetupMgr ()
		{
			mCurMgr = this;
		}
	}
}