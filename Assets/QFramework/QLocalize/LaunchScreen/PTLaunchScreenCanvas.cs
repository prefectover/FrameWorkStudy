using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace PTGame.Localize {
	public class PTLaunchScreenCanvas : MonoBehaviour {

		Image mLaunchScreenEnImage;
		Image mLaunchScreenCnImage;
		void Awake() {
			mLaunchScreenCnImage = transform.Find ("LaunchScreenCn").GetComponent<Image> ();
			mLaunchScreenEnImage = transform.Find ("LaunchScreenEn").GetComponent<Image> ();
		}

		IEnumerator Start() {
			Image curLaunchScreenImage;
			if (PTGame.Localize.PTLanguageManager.Instance.IsChinese) {
				curLaunchScreenImage = mLaunchScreenCnImage;
			}
			else {
				curLaunchScreenImage = mLaunchScreenEnImage;
			}
			curLaunchScreenImage.gameObject.SetActive (true);
			PTAlphaTween.alpha (curLaunchScreenImage.GetComponent<RectTransform> (), 1.0f, 1.0f);
			yield return new WaitForSeconds (2.0f);
			PTAlphaTween.alpha (curLaunchScreenImage.GetComponent<RectTransform> (), 0.0f, 1.0f);
			yield return new WaitForSeconds (1.0f);
			SceneManager.LoadScene (1);
		}
	}

}