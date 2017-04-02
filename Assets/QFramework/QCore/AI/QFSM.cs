using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using QFramework;

namespace QFramework {

	/// <summary>
	/// 状态机基类
	/// </summary>
	public class QFSMState
	{
		public QFSMState(ushort name) {
			this.name = name;
		}

		public ushort name;						// 字符串
		public virtual void OnEnter() {}			// 进入状态(逻辑)
		public virtual void OnExit() {}			// 离开状态(逻辑)
		/// <summary>
		/// 存储事件对应的条转
		/// </summary>
		public Dictionary <ushort,QFSMTranslation> TranslationDict = new Dictionary<ushort,QFSMTranslation>();
	}
		
	/// <summary>
	/// 跳转类
	/// </summary>
	public class QFSMTranslation
	{
		public QFSMState fromState;
		public ushort eventName;
		public QFSMState toState;

		public QFSMTranslation(QFSMState fromState,ushort eventName, QFSMState toState)
		{
			this.fromState = fromState;
			this.toState   = toState;
			this.eventName = eventName;
		}
	}

	public class QFSM
	{
		QFSMState mCurState;

		public QFSMState State {
			get {
				return mCurState;
			}
		}

		// 状态字典
		Dictionary<ushort, QFSMState> mStateDict = new Dictionary<ushort, QFSMState>();

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="state">State.</param>
		public void AddState(QFSMState state)
		{
			mStateDict.Add (state.name, state);
		}


		/// <summary>
		/// 添加条转
		/// </summary>
		/// <param name="translation">Translation.</param>
		public void AddTranslation(QFSMTranslation translation)
		{
			mStateDict [translation.fromState.name].TranslationDict.Add(translation.eventName, translation);
		}


		/// <summary>
		/// 添加条转
		/// </summary>
		/// <param name="translation">Translation.</param>
		public void AddTranslation(QFSMState fromState,ushort eventName,QFSMState toState)
		{
			mStateDict [fromState.name].TranslationDict.Add(eventName, new QFSMTranslation (fromState, eventName, toState));
		}

		/// <summary>
		/// 启动状态机
		/// </summary>
		/// <param name="state">State.</param>
		public void Start(QFSMState startState)
		{
			mCurState = startState;
			mCurState.OnEnter ();
		}

		/// <summary>
		/// 处理事件
		/// </summary>
		/// <param name="name">Name.</param>
		public void HandleEvent(ushort eventName)
		{	
			if (mCurState != null && mStateDict[mCurState.name].TranslationDict.ContainsKey(eventName)) {
				QFSMTranslation tempTranslation = mStateDict [mCurState.name].TranslationDict [eventName];
				tempTranslation.fromState.OnExit ();
				mCurState =  tempTranslation.toState;
				tempTranslation.toState.OnEnter ();
			}
		}
			
		/// <summary>
		/// 清空
		/// </summary>
		public void Clear()
		{
			mStateDict.Clear ();
		}
	}
}