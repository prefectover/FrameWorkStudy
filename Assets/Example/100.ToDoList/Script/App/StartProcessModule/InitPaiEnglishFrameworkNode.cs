using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Putao.PaiEnglish;

namespace Putao.PaiEnglish {
	public class InitPaiEnglishFrameworkNode : ExecuteNode {

		public override void OnExecute ()
		{
//			ConfigManager.Instance.Load ();
			isFinish = true;
		}
	}

}