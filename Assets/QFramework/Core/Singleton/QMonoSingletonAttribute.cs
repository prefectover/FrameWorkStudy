using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QMonoSingletonAttribute : System.Attribute
    {
        private string m_AbsolutePath;

        public QMonoSingletonAttribute(string relativePath)
        {
            m_AbsolutePath = relativePath;
        }

        public string AbsolutePath
        {
            get { return m_AbsolutePath; }
        }
    }
}
