using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace QFramework {

	/// <summary>
	/// 多语言Text
	/// </summary>
	public class PTText : MonoBehaviour,ILocalizeView {


		/// <summary>
		/// 设置Key
		/// </summary>
		public static PTText Localize(GameObject go,string key) {
			var ptText = go.AddComponent<PTText> ();
			ptText.SetKey (key);
			return ptText;
		}
		public static PTText Localize(Text text,string key) {
			return Localize (text.gameObject, key);
		}
		public static PTText Localize(Transform trans,string key) {
			return Localize (trans.gameObject, key);
		}


		[SerializeField] string Key;
		/// <summary>
		/// 设置Key
		/// </summary>
		public void SetKey(string key) {
			Key = key;
			UpdateLocalizeView ();
		}

		void Awake() {
			PTLanguageManager.Instance.PushActiveLocalizeView (this);
			if (!string.IsNullOrEmpty(Key)) {
				UpdateLocalizeView ();
			}
		}
			
		public void UpdateLocalizeView() {
			GetComponent<Text> ().text = PTLanguageManager.Instance.Text4Key (Key.Trim());
		}

		void OnDestroy() {
			PTLanguageManager.Instance.RemoveActiveLocalizeView (this);
		}
	}

}
