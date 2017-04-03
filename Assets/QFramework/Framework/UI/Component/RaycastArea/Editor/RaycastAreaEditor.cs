using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace QFramework
{
    [CustomEditor(typeof(RaycastArea), false)]
    [CanEditMultipleObjects]
    public class RaycastAreaEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {

        }
    }
}
