using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestMsgDispatcher {

	[Test]
	public void 测试MsgTestRunner() {
		//Arrange
		var gameObject = new GameObject();

		//Act
		//Try to rename the GameObject
		var newGameObjectName = "My game object";
		gameObject.name = newGameObjectName;
		
		Debug.LogError ("run the test");
		//Assert
		//The object has a new name
		Assert.AreEqual(newGameObjectName, gameObject.name);
	}
}
