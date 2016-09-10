using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/// <summary>
///	组合方式实现单例子
/// </summary>
namespace QFramework {

	/// <summary>
	/// class是引用类型
	/// </summary>
	public class QSingletonComponent<T> where T : class
	{
		protected static T mInstance = null;

		public static T Instance
		{

			get {
				if (mInstance == null) {
					// 先获取所有非public的构造方法
					ConstructorInfo[] ctors = typeof(T).GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic);
					// 从ctors中获取无参的构造方法
					ConstructorInfo ctor = Array.Find (ctors, c => c.GetParameters ().Length == 0);
					if (ctor == null)
						throw new Exception ("Non-public ctor() not found!");
					// 调用构造方法
					mInstance = ctor.Invoke (null) as T;
				}

				return mInstance;
			}
		}

		public static void Dispose()
		{
			mInstance = null;
		}
	}
}