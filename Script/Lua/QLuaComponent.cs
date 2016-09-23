using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LuaInterface;

using UnityEngine.UI;

using System.IO;
using System;

namespace QFramework
{
    public class QLuaComponent : MonoBehaviour {
      
		protected static class  FuncName
		{
			public static readonly string Awake = "Awake";
			public static readonly string OnEnable = "OnEnable";
			public static readonly string Start = "Start";
			public static readonly string Update = "Update";
			public static readonly string OnDisable = "OnDisable";
			public static readonly string OnDestroy = "OnDestroy";
		};
//#if UNITY_EDITOR
		public static bool isFirstLaunch = true;
//#endif
		/// <summary>  
		/// 提供给外部手动执行LUA脚本的接口  
		/// </summary>  
		public bool Initilize(string path)  
		{  
//#if UNITY_EDITOR
//
//			if(isFirstLaunch)
//			{
//				isFirstLaunch = false;
//				string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
//				string frontName = sceneName.Split('_')[0];
//				if(frontName.Equals("ARBookFramework"))
//				{
//					LuaMain.LoadBundlesThenToScene(sceneName,false,"core","PTLua","ARBookFramework");
//					return false;
//				}else if(frontName.Equals("PTLua")){
//					
//				}else{
//					LuaMain.LoadBundlesThenToScene(sceneName,false,"core","PTLua","ARBookFramework",frontName);
//					return false;
//				}
//			}
//#endif


			LuaPath = path;
			Init ();
			return true;
		}  

       
        //lua路径，不用填缀名，可以是bundle
        [Tooltip("script path")]
        public string LuaPath;

		//lua路径，不用填缀名，可以是bundle
//		[Tooltip("is Lua Class")]
//		public bool isLuaClass = true;

		public LuaTable LuaModule{
			get{ 
				return mSelfLuaTable;
			}
		}


		public string LuaClass{
			get{ 
				return LuaClassName;
			}
		}


		private LuaTable mSelfLuaTable = null;
		private string LuaClassName = null;

        //初始化函数，可以被重写，已添加其他
        protected virtual bool Init()
        {
			

			mSelfLuaTable = LuaMain.getInstance().addLuaFile(LuaPath, gameObject);

			LuaClassName = CallLuaFunctionRString ("getClassName");
			mSelfLuaTable["gameObject"] = gameObject;
			mSelfLuaTable["transform"] = transform;
//			mSelfLuaTable.Push ();
			//add button func
			if (gameObject.GetComponent<Button>()!=null) {
				gameObject.GetComponent<Button>().onClick.AddListener(
					delegate() {
						onClick();
					}
				);
			}


            return true;
        }

		private string CallLuaFunctionRString(string name, params object[] args)
		{
			string resault = null;
			if (mSelfLuaTable != null) {
				LuaFunction func = mSelfLuaTable.GetLuaFunction (name);
				if (null == func) {
					return resault;
				}
				func.BeginPCall ();
				func.Push(mSelfLuaTable);
				foreach (var o in args) {
					func.Push (o);
				}
				func.PCall ();
				resault = func.CheckString ();
				func.EndPCall ();
			}
			return resault;
		}

			
		public void CallLuaFunction(string name, params object[] args)
		{
			if (mSelfLuaTable != null) {
				LuaFunction func = mSelfLuaTable.GetLuaFunction (name);
				if (null == func) {
					return;
				}
				func.BeginPCall ();
				func.Push(mSelfLuaTable);
				foreach (var o in args) {
					func.Push (o);
				}
				func.PCall ();
				func.EndPCall ();
			}
		}
			

        void OnEnable()
        {
			CallLuaFunction (LuaMain.FuncName.OnEnable);
        }

        void Start()
        {
			if(Initilize(LuaPath))
				CallLuaFunction (LuaMain.FuncName.Awake);
			
			CallLuaFunction (LuaMain.FuncName.Start);
        }

        void Update()
        {
			CallLuaFunction (LuaMain.FuncName.Update);
        }

        void OnDisable()
        {
			CallLuaFunction (LuaMain.FuncName.OnDisable);
        }

        void OnDestroy()
        {
			CallLuaFunction (LuaMain.FuncName.OnDestroy);

			if (null != mSelfLuaTable)
			{
				mSelfLuaTable.Dispose();
				mSelfLuaTable = null;
			}
        } 

		void onClick()
		{
			CallLuaFunction (LuaMain.FuncName.onClick);
		}
			
    }
}

