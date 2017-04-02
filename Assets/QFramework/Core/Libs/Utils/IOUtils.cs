using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace QFramework.Libs {

	/// <summary>
	/// 各种文件的读写复制操作,主要是对System.IO的一些封装
	/// </summary>
	public class IOUtils {

		/// <summary>
		/// 创建新的文件夹,如果存在则不创建
		/// </summary>
		public static void CreateDirIfNotExists(string dirFullPath)
		{
			if (!Directory.Exists (dirFullPath)) {
				Debug.Log ("dir " + dirFullPath + " is not exist,create new one");
				Directory.CreateDirectory (dirFullPath);
			}
		}


		#region SCFramework
		public static List<string> GetDirSubFilePathList(string dirABSPath, bool isRecursive = true, string suffix = "")
		{
			List<string> pathList = new List<string>();
			DirectoryInfo di = new DirectoryInfo(dirABSPath);

			if (!di.Exists)
			{
				return pathList;
			}

			FileInfo[] files = di.GetFiles();
			foreach (FileInfo fi in files)
			{
				if (!string.IsNullOrEmpty(suffix))
				{
					if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
					{
						continue;
					}
				}
				pathList.Add(fi.FullName);
			}

			if (isRecursive)
			{
				DirectoryInfo[] dirs = di.GetDirectories();
				foreach (DirectoryInfo d in dirs)
				{
					pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
				}
			}

			return pathList;
		}

		public static List<string> GetDirSubDirNameList(string dirABSPath)
		{
			List<string> nameList = new List<string>();
			DirectoryInfo di = new DirectoryInfo(dirABSPath);

			DirectoryInfo[] dirs = di.GetDirectories();
			foreach (DirectoryInfo d in dirs)
			{
				nameList.Add(d.Name);
			}

			return nameList;
		}

		public static string GetFileName(string absOrAssetsPath)
		{
			string name = absOrAssetsPath.Replace("\\", "/");
			int lastIndex = name.LastIndexOf("/");

			if (lastIndex >= 0)
			{
				return name.Substring(lastIndex + 1);
			}
			else
			{
				return name;
			}
		}

		public static string GetFileNameWithoutExtend(string absOrAssetsPath)
		{
			string fileName = GetFileName(absOrAssetsPath);
			int lastIndex = fileName.LastIndexOf(".");

			if (lastIndex >= 0)
			{
				return fileName.Substring(0, lastIndex);
			}
			else
			{
				return fileName;
			}
		}

		public static string GetFileExtendName(string absOrAssetsPath)
		{
			int lastIndex = absOrAssetsPath.LastIndexOf(".");

			if (lastIndex >= 0)
			{
				return absOrAssetsPath.Substring(lastIndex);
			}

			return string.Empty;
		}

		public static string GetDirPath(string absOrAssetsPath)
		{
			string name = absOrAssetsPath.Replace("\\", "/");
			int lastIndex = name.LastIndexOf("/");
			return name.Substring(0, lastIndex + 1);
		}
		#endregion
	}
}
