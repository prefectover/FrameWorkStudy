#define ToLuaSupport
using UnityEngine;
using System.Collections;


#if ToLuaSupport

#else
namespace LuaInterface {
	public class LuaFileUtils {
		protected static LuaFileUtils instance;
		public static LuaFileUtils Instance;
		public bool beZip;
		public virtual byte[] ReadFile(string fileName) {
			return null;    
		}
		public void AddSearchBundle(string bundleToLower,AssetBundle bundle) {}

	}

	public class LuaState {
		public bool LogGC;
		public void Start() {}
		public object[] DoFile (string fileName) { return null;}
		public LuaFunction GetFunction(string funcName) { return null; }
		public void Dispose() {
		}

		public void AddSearchPath (string path){
		}
		public void LuaSetTop(int top) {}

		public void LuaGetField(object index,string str) {
		}

		public void OpenLibs(object o) {
		}
		public void LuaSetField(object o,object o2) {
		}
		public void  LuaGC(object o) {}

	

	}

	public class LuaDLL {
		public const int luaopen_cjson = 0;
		public const int luaopen_pb = 0;
		public const int luaopen_cjson_safe = 0;
		public const int luaopen_sproto_core = 0;
		public const int luaopen_protobuf_c = 0;
		public const int luaopen_lpeg = 0;
		public const int luaopen_bit = 0;
		public const int luaopen_socket_core = 0;
	}

	public class LuaGCOptions {
		public const int LUA_GCCOLLECT = 0;
	}
	public class LuaIndexes {
		public const int LUA_REGISTRYINDEX = 1;
	}

	public class LuaFunction {
		public void BeginPCall() {}
		public void Push(object o) {}
		public void PCall() {}
		public object[] Call(object o) {
			return null;
		}

		public void EndPCall() {}
		public LuaTable CheckLuaTable() {
			return null;
		}
		public void Dispose() {}
		public string CheckString () {
			return null;
		}
		public void Call() {}
	}

	public class LuaTable {
		public void Dispose() {} 

		public object this[string name] {
			get {
				return "";
			} 
			set {

			}
		}

		public LuaFunction GetLuaFunction(string funcName) { return null;}
	}

	public class LuaBinder {
		public static void Bind(LuaState state) {}
	}

	public class LuaCoroutine {
		public static void Register(LuaState state, MonoBehaviour behaviour) {}
	}

	public class Debugger {
		public static void LogError(string errorMsg)
		{
			Debug.LogError (errorMsg);
		}
	}

	public class LuaByteBuffer {
		public object[] buffer;
	}

	public class LuaLooper : MonoBehaviour {
		public LuaState luaState;
		public void Destroy() {} 
	}
}
#endif

