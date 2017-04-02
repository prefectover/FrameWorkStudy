using UnityEngine;
using System.Collections;
using QFramework;

/// <summary>
/// Path manager.
/// </summary>
public class PathManager : MonoBehaviour {

	void Awake() {
		Debug.Log(GetPath ("UIPrefabPath"));
	}

	public string GetPath(string pathName) {
		var pathItem = Resources.Load<PathConfig> (pathName);
		return pathItem.List[0].FullPath;
	}
}
