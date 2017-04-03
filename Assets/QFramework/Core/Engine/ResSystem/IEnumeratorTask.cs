using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace QFramework
{
    public interface IEnumeratorTask
    {
        IEnumerator StartIEnumeratorTask(Action finishCallback);
    }

    public interface IEnumeratorTaskMgr
    {
        void PostIEnumeratorTask(IEnumeratorTask task);
    }
}
