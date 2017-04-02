using UnityEngine;
using System.Collections;

namespace QFramework
{
    //	Map-<key, state>:key = sourceCode << 16 + targetCode
    //		Transition
    [System.Serializable]
    public class FSMStateTransition<T>
    {
        [SerializeField]
        protected string m_SourceStateName;
        [SerializeField]
        protected string m_TargetStateName;

        public FSMStateTransition(string sourceStateName, string targetStateName)
        {
            m_SourceStateName = sourceStateName;
            m_TargetStateName = targetStateName;
        }

        public string sourceStateName
        {
            get { return m_SourceStateName; }
        }

        public string targetStateName
        {
            get { return m_TargetStateName; }
        }
    }

}
