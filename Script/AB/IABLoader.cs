using UnityEngine;
using System.Collections;
using System.Text;

/// <summary>
/// 每一帧的回调
/// </summary>
public  delegate void LoaderProgress(string bundle,float progress);

/// <summary>
/// 完成了 才回调
/// </summary>
public delegate void LoadFinish(string bundle);

public class IABLoader {

	string mBundleName;

	string mCommonBundlePath;

	WWW mCommonLoader;
	float mCommonResLoadProgress;

	LoaderProgress mLoadProgress;
	LoadFinish mLoadFinish;

	public IABLoader(LoaderProgress loadProgress,LoadFinish loadFinish)
	{
		mCommonBundlePath = "";
		mBundleName = "";
		mCommonResLoadProgress = 0;
		mLoadFinish = loadFinish;

		mLoadProgress = loadProgress;
	}

	public void LoadRes(string path)
	{
		mCommonBundlePath = path;
	}

	// 携程加载
	public IEnumerator CommonLoad()
	{
		mCommonLoader = new WWW (mCommonBundlePath);

		while (!mCommonLoader.isDone) {
			mCommonResLoadProgress = mCommonLoader.progress;

			if (null != mLoadProgress) {
				mLoadProgress (mBundleName, mCommonLoader.progress);
			}
			yield return mCommonLoader.progress;
			mCommonResLoadProgress = mCommonLoader.progress;
		}

		if (mCommonResLoadProgress >= 1.0f) { // 表示已经加载完成
			if (null != mLoadProgress) {
				mLoadProgress (mBundleName, mCommonLoader.progress);
			}

			if (null != mLoadFinish) {
				this.mLoadFinish (mBundleName);
			}
				
			mABLoader = new IABResLoader (mCommonLoader.assetBundle);
		} else {
			Debug.LogError ("load bundle error == " + mBundleName);
		}

		mCommonLoader = null;


	}


	IABResLoader mABLoader;
	// Debug
	public void DebugerLoader()
	{
		if (mCommonLoader != null) {
			mABLoader.DebugAllRes ();
		}
	}


	public void SetABName(string abName)
	{
		this.mBundleName = abName;
	}

	#region 需要上层传递完整路径

	public void UnLoadAssetRes(Object tmpObj)
	{
		if (null != mABLoader) {
			mABLoader.UnloadRes (tmpObj);
		}
	}
	#endregion

	#region 下层提供功能



	/// <summary>
	/// 获取单个资源
	/// </summary>
	public Object GetRes(string name)
	{
//		return mABLoader [name];

		if (null != mABLoader) {
			return mABLoader[name];
		} else {
			return null;
		}
	}

	/// <summary>
	/// 获取多个资源
	/// </summary>
	public Object[] GetMultiRes(string name)
	{
		if (null != mABLoader) {
			return mABLoader.LoadRes (name);
		} else {
			return null;
		}
	}

	/// <summary>
	/// Releases all resource used by the <see cref="IABLoader"/> object.
	/// </summary>
	/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="IABLoader"/>. The <see cref="Dispose"/>
	/// method leaves the <see cref="IABLoader"/> in an unusable state. After calling <see cref="Dispose"/>, you must
	/// release all references to the <see cref="IABLoader"/> so the garbage collector can reclaim the memory that the
	/// <see cref="IABLoader"/> was occupying.</remarks>
	public void Dispose()
	{
		if (null != mABLoader) {
			mABLoader.Dispose ();
			mABLoader = null;
		}
	}

	#endregion


}
