using System;
using QFramework;

namespace QFramework
{
	public class QUIFactory
	{
		private QUIFactory() {}

		public static QUIFactory Instance {
			get {
				return QSingletonComponent<QUIFactory>.Instance;
			}
		}
		public static void Dispose()
		{
			QSingletonComponent<QUIFactory>.Dispose ();
		}
		public IUIComponents CreateUIComponents(string strUI)
		{
			IUIComponents retComponents = null;
			switch (strUI)
			{
			}
			return retComponents;
		}
	}
}
