using System;
using UnityEngine;
using SCFramework;

using System.Collections;
using System.Collections.Generic;
using QFramework;
namespace QFramework
{
	[QMonoSingletonAttribute("[World]/World")]
    public class WorldMgr : TMonoSingleton<WorldMgr>
    {
        private WorldRoot m_WorldRoot;

        public WorldRoot worldRoot
        {
            get { return m_WorldRoot; }
        }

        public override void OnSingletonInit()
        {
            if (m_WorldRoot == null)
            {
                WorldRoot root = GameObject.FindObjectOfType<WorldRoot>();
                if (root == null)
                {
                    Log.e("Failed to Find WorldRoot!");
                    return;
                }

                m_WorldRoot = root;
            }
        }
    }
}
