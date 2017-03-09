using UnityEngine;
using System.Collections;
using QFramework;

public class GameManager : QSingleton<GameManager> {

	private GameManager() {}
	public IEnumerator Launch() {

		yield return null;
	}
}
