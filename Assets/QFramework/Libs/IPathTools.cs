using UnityEngine;
using System.Collections;
using System.IO;

public class IPathTools 
{

	public static string GetPlatformForderName(RuntimePlatform platform)
	{
		switch (platform) {
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:

			return "iOS";

		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			return "Windows";
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.OSXEditor:

			return "OSX";

		default:
			return null;
		}
	}

	public static string GetAppFilePath()
	{
		string retPath = "";
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
			retPath = Application.streamingAssetsPath;
		} else {
			retPath = Application.persistentDataPath;
		}

		return retPath;
	}

	public static string GetABPath()
	{
		string platformForder = GetPlatformForderName (Application.platform);

		string allPath = Path.Combine (GetAppFilePath(), platformForder);

		return allPath;
	}


}
