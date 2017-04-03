using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using SCFramework;

namespace PTGame.PaiLogic
{
    public class InitEnvironmentNode : ExecuteNode
    {
        public override void OnBegin()
        {
            Log.i("ExecuteNode:" + GetType().Name);
            ResMgr.Instance.InitResMgr();
            AppConfig.S.InitAppConfig();

            if (AppConfig.S.dumpToFile)
            {
                DebugLogger.Instance.InitDebugLogger();
            }

            isFinish = true;
        }
    }
}
