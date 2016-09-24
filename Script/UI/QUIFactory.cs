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
//				case "UIBottomFrame":
//					retComponents = new UIBottomFrameComponents();
//					break;
//				case "UICommonBg":
//					retComponents = new UICommonBgComponents();
//					break;
//				case "UIGameLayer":
//					retComponents = new UIGameLayerComponents();
//					break;
//				case "UIGoldLabel":
//					retComponents = new UIGoldLabelComponents();
//					break;
//				case "UIHelpLayer":
//					retComponents = new UIHelpLayerComponents();
//					break;
//				case "UIHomeBg":
//					retComponents = new UIHomeBgComponents();
//					break;
//				case "UIHomeLayer":
//					retComponents = new UIHomeLayerComponents();
//					break;
//				case "UIPauseLayer":
//					retComponents = new UIPauseLayerComponents();
//					break;
//				case "UISettingLayer":
//					retComponents = new UISettingLayerComponents();
//					break;
//				case "UIShopLayer":
//					retComponents = new UIShopLayerComponents();
//					break;
//				case "UITopFrame":
//					retComponents = new UITopFrameComponents();
//					break;
			}
			return retComponents;
		}
	}
}
