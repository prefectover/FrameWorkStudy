using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 各种文件的读写复制操作,主要是对System.IO的一些封装
/// </summary>
namespace QFramework {
	
	public class QIO {

		/// <summary>
		/// 创建新的文件夹,如果存在则不创建
		/// </summary>
		public static void CreateDirIfNotExists(string dirFullPath)
		{
			if (!Directory.Exists (dirFullPath)) {
				Directory.CreateDirectory (dirFullPath);
			}
		}
	}
}
