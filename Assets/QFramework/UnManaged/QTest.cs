using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace QFramework {

	/// <summary>
	/// 用来测试程序耗时
	/// </summary>
	public static class QTest {

		public static Dictionary<string,Stopwatch> watches = new Dictionary<string, Stopwatch> ();

		public static Stopwatch TimeBegan(string name)
		{
			var watch = new Stopwatch ();
			watches.Add (name, watch);
			watch.Start ();
			return watch;
		}

		public static long TimeStop(string name)
		{
			watches [name].Stop ();
			long retValue = watches [name].ElapsedMilliseconds;
			watches.Remove (name);
			return retValue;


		}

		public static long TimeStop(Stopwatch watch)
		{
			watch.Stop ();
			long retValue = watch.ElapsedMilliseconds;
			return retValue;
		}

	}
}
