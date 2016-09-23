using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
#if ToLua
using LuaInterface;
#endif
using System;
using QFramework;
using QFramework.AB;

namespace QFramework {
    public static class QLuaHelper {

		#if ToLua

        /// <summary>
        /// getType
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static System.Type GetType(string classname) {
            Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
            System.Type t = null;
            t = assb.GetType(classname); ;
            if (t == null) {
                t = assb.GetType(classname);
            }
            return t;
        }

        /// <summary>
        /// 资源管理器
        /// </summary>
		public static QResMgr GetResManager() {
			return QFramework.Instance.GetMgr<QResMgr>();
        }

        /// <summary>
        /// 音乐管理器
        /// </summary>
		public static QSoundMgr GetSoundManager() {
			return QFramework.Instance.GetMgr<QSoundMgr>();
        }


        /// <summary>
        /// pbc/pblua函数回调
        /// </summary>
        /// <param name="func"></param>
		/// 
        public static void OnCallLuaFunc(LuaByteBuffer data, LuaFunction func) {
            if (func != null) func.Call(data);
            Debug.LogWarning("OnCallLuaFunc length:>>" + data.buffer.Length);
        }

        /// <summary>
        /// cjson函数回调
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        public static void OnJsonCallFunc(string data, LuaFunction func) {
            Debug.LogWarning("OnJsonCallback data:>>" + data + " lenght:>>" + data.Length);
            if (func != null) func.Call(data);
        }
		#endif

    }
}