using UnityEngine;
using System.Collections;

[RequireComponent(typeof(QFramework.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStart : MonoBehaviour {
	void Start () {
        GetComponent<QFramework.LoopScrollRect>().RefillCells();
	}
}
