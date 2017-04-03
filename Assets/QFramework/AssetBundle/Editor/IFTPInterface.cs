using UnityEngine;
using System.Collections;
using QFramework;

namespace QFrameworkAB
{
	public interface IFTPInterface
	{

		bool Connect ();

		bool Exist (string dir, string fileName);

		bool Rename (string oldName, string newFileName);

		bool MakeDir (string pathName);

		bool Upload (string localPath, string remotePath);

		void Disconnect ();
	}

}