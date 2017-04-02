using UnityEngine;
using UnityEditor;
using System.Collections;
using QFramework;

namespace QFrameworkAB
{
	public class AssetBundlesMenuItems
	{
		const string kSimulationMode = "PuTaoTool/AssetBundles/Simulation Mode";
	
		[MenuItem(kSimulationMode)]
		public static void ToggleSimulationMode ()
		{
			QAssetBundleTool.SimulateAssetBundleInEditor = !QAssetBundleTool.SimulateAssetBundleInEditor;
		}
	
		[MenuItem(kSimulationMode, true)]
		public static bool ToggleSimulationModeValidate ()
		{
			Menu.SetChecked(kSimulationMode, QAssetBundleTool.SimulateAssetBundleInEditor);
			return true;
		}

	}


}