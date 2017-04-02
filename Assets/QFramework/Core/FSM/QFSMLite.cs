using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace QFramework {
	/// <summary>
	/// 状态机实现
	/// 轻量级,字符串 + 字典实现
	/// </summary>
	public class QFSMLite {
		// 定义函数指针类型
		public delegate void FSMCallfunc();

		/// <summary>
		/// 状态类
		/// </summary>
		class QFSMState 
		{		
			public string name;

			public QFSMState(string name)
			{
				this.name = name;
			}

			/// <summary>
			/// 存储事件对应的条转
			/// </summary>
			public Dictionary <string,QFSMTranslation> TranslationDict = new Dictionary<string,QFSMTranslation>();
		}

		/// <summary>
		/// 跳转类
		/// </summary>
		public class QFSMTranslation
		{
			public string fromState;
			public string name;
			public string toState;
			public FSMCallfunc callfunc;	// 回调函数

			public QFSMTranslation(string fromState,string name, string toState,FSMCallfunc callfunc)
			{
				this.fromState = fromState;
				this.toState   = toState;
				this.name = name;
				this.callfunc = callfunc;
			}
		}

		// 当前状态
		string mCurState;

		public string State {
			get {
				return mCurState;
			}
		}

		// 状态字典
		Dictionary <string,QFSMState> mStateDict = new Dictionary<string,QFSMState>();

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="state">State.</param>
		public void AddState(string name)
		{
			mStateDict [name] = new QFSMState(name);
		}

		/// <summary>
		/// 添加条转
		/// </summary>
		/// <param name="translation">Translation.</param>
		public void AddTranslation(string fromState,string name,string toState,FSMCallfunc callfunc)
		{
			mStateDict [fromState].TranslationDict [name] = new QFSMTranslation (fromState, name, toState, callfunc);
		}

		/// <summary>
		/// 启动状态机
		/// </summary>
		/// <param name="state">State.</param>
		public void Start(string name)
		{
			mCurState = name;
		}


		/// <summary>
		/// 处理事件
		/// </summary>
		/// <param name="name">Name.</param>
		public void HandleEvent(string name)
		{	
			if (mCurState != null && mStateDict[mCurState].TranslationDict.ContainsKey(name)) {
				QFSMTranslation tempTranslation = mStateDict [mCurState].TranslationDict [name];
				tempTranslation.callfunc ();
				mCurState =  tempTranslation.toState;
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


	// Debu用的
	// 			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
	//watch.Start ();


}