using UnityEngine;
using System.IO;
using System;
using System.Text;
using UnityEditor;
using System.Collections;

namespace QFramework.Editor 
{
	public class QEditorUtil  {

		/// <summary>
		/// 根据文件路径获取文件的byte数据
		/// </summary>
		public static byte[] GetFileBytes(string fileFullPath)
		{
			FileStream file = new FileStream(fileFullPath, FileMode.Open);
			byte[] retBytes = new byte[file.Length];  
			file.Read(retBytes, 0, (int)file.Length);  
			file.Close();

			return retBytes;
		}

		/// <summary>
		/// 获取文件的MD5值
		/// </summary>
		public static string MD5(byte[] bytes)
		{
			try
			{
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] hashBytes = md5.ComputeHash(bytes);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
		}

		/// <summary>
		/// 获取文件的大小
		/// </summary>
		public static string Size(byte[] bytes)
		{
			return bytes.Length.ToString ();
		}
	}
}
