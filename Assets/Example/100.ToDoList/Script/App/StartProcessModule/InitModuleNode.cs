using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using SCFramework;

namespace PTGame.PaiLogic
{
    public class InitModuleNode : ExecuteNode
    {
        public override void OnBegin()
        {
            Log.i("ExecuteNode:" + GetType().Name);
//            GameMgr.S.AddCom<UIDataModule>();
//            GameMgr.S.AddCom<InputModule>();
//            GameMgr.S.AddCom<GameplayModule>();
            isFinish = true;
        }
    }
}
