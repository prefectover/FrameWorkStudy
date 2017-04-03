using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace QFramework
{
    public class FloatMessageItem : MonoBehaviour
    {
        [SerializeField]
        private Text m_Text;

        public void SetFloatMsg(FloatMsg msg)
        {
            if (msg == null)
            {
                return;
            }

            m_Text.text = msg.message;
        }
    }
}
