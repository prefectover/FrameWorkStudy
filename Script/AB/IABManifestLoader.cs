using UnityEngine;
using System.Collections;


public class IABManifestLoader 
{
	public AssetBundleManifest assetManifest;

	public string manifestPath;

	public bool IsLoadFinish;


	public AssetBundle manifestLoader;


	public IABManifestLoader()
	{
		assetManifest = null;

		manifestLoader = null;

		IsLoadFinish = false;

		manifestPath = IPathTools.GetABPath ();
	}

	public IEnumerator LoadManifest()
	{
		WWW manifest = new WWW (manifestPath);

		yield return manifest;

		if (!string.IsNullOrEmpty (manifest.error)) {
			Debug.Log (manifest.error);
		} else {
			if (manifest.progress >= 1.0f) {
				manifestLoader = manifest.assetBundle;

				assetManifest = manifestLoader.LoadAsset ("AssetBundleManifest") as AssetBundleManifest;

				IsLoadFinish = true;
			}
		}
	}

	public string[] GetDepences(string name)
	{
		return assetManifest.GetAllDependencies (name);
	}

	public void UnloadManifest()
	{
		manifestLoader.Unload (true);
	}

	private static IABManifestLoader instance = null;

	public void SetManifestPath(string path)
	{
		manifestPath = path;
	}

	private static IABManifestLoader mInstance = null;

	public static IABManifestLoader Instance
	{
		get {
			if (mInstance == null) {
				mInstance = new IABManifestLoader ();
			}

			return mInstance;
		}
	}
}