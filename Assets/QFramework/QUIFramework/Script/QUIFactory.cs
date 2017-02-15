using System;
using QFramework;

namespace QFramework.UI
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
				case "UIFirstPage":
					retComponents = new UIFirstPageComponents();
					break;
				case "UIMainPage":
					retComponents = new UIMainPageComponents();
					break;
				case "UIUnityMsgChildPanel":
					retComponents = new UIUnityMsgChildPanelComponents();
					break;
				case "UIUnityMsgRootPage":
					retComponents = new UIUnityMsgRootPageComponents();
					break;
			}
			return retComponents;
		}
	}
}
