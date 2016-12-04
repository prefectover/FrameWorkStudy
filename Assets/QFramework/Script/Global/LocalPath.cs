using UnityEngine;
using System.Collections;

/// <summary>
/// 本地路径
/// </summary>
namespace QFramework {
	public class LocalPath {

		// AssetBundle中资源路径格式
		public static string AssetBundleFormation = "assets/resources/{0}";

		#if UNITY_ANDROID
		public static string AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
		#else
		public static string AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
		#endif

	}
}
