using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PTGame.AssetBundles
{
	public class AssetBundlesMenuItems
	{
		const string kSimulationMode = "PuTaoTool/AssetBundles/Simulation Mode";
	
		[MenuItem(kSimulationMode)]
		public static void ToggleSimulationMode ()
		{
			PTAssetBundleTool.SimulateAssetBundleInEditor = !PTAssetBundleTool.SimulateAssetBundleInEditor;
		}
	
		[MenuItem(kSimulationMode, true)]
		public static bool ToggleSimulationModeValidate ()
		{
			Menu.SetChecked(kSimulationMode, PTAssetBundleTool.SimulateAssetBundleInEditor);
			return true;
		}

	}


}