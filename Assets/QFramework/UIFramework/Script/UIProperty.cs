using UnityEngine;
using System;
using System.Collections;
using QFramework;

/// <summary>
/// UI数据,维护一个GetterSetter
/// </summary>
public class UIProperty<T> {

	T mValue;
	public T Value {
		get {
			return mValue;
		}
		set {
			mDirty = true;
			mValue = value;
		}
	}

	/// <summary>
	/// 如果更新mDirty就是true
	/// </summary>
	public bool mDirty;
}
