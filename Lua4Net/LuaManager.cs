using System;
using System.Collections.Generic;

namespace Lua4Net
{
    [Scriptable]
    public class LuaManager
    {
        public LuaManager()
        {
            Instance = this;

            l = LuaApi.luaL_newstate();
            LuaApi.luaL_openlibs(l);

            stack = new StackManager();
            heap = new HeapManager();
            funcs = new List<LuaRefFunction>();
        }

        public bool Initialize(LuaRegister r,string rootfile, string userfile,Action<string> OnErrorLog)
        {
            bool result = false;
            register = r;
            this.OnErrorLog = OnErrorLog != null ? OnErrorLog : (s) => { Console.WriteLine(s); };

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            register.Register(l);
            int ret = LuaApi.luaL_dofile(l, rootfile);
            if (ret == 0)
            {
                if (!string.IsNullOrEmpty(userfile))
                {
                    ret = LuaApi.luaL_dofile(l, userfile);
                    if (0 == ret)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        LastErrorMsg = LuaApi.lua_tostring(l, -1);
                        Log(LastErrorMsg);
                    }
                }
                else
                {
                    result = true;
                }
            }     
            else
            {
                result = false;
                LastErrorMsg = LuaApi.lua_tostring(l, -1);
                Log(LastErrorMsg);                
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        [Scriptable]
        public object GetObject(int id)
        {
            if (id > 0)
            {
                return stack.GetObject(id);
            }
            else if (id < 0)
            {
                return heap.Get(id);
            }
            else
            {
                return null;
            }            
        }

        public T GetObjectT<T>(int id)
        {
            object ret = GetObject(id);
            if (null != ret)
                return (T)ret;
            else
                return default(T);
        }

        public int PushStackObject(object obj)
        {
            return stack.PushObject(obj);
        }

        [Scriptable(Name = "AddGlobalObject")]
        public int AddHeapObject(object obj)
        {
            return heap.Add(obj);
        }

        [Scriptable(Name = "SetGlobalObject")]
        public void SetHeapObject(int id, object obj)
        {
            heap.Set(id, obj);
        }

        [Scriptable]
        public void Test(LuaStackFunction lsf, LuaRefFunction lrf)
        {
        }

        public void Log(string content)
        {
            OnErrorLog(content);
        }

        public void GC()
        {
            lock (Instance)
            {
                int count = funcs.Count;
                for (int i = 0; i < count; ++i)
                {
                    funcs[i].Dispose();
                }
                funcs.Clear();
            }
        }

        public void GC(LuaRefFunction func)
        {
            lock (Instance)
            {
                funcs.Add(func);
            }
        }

        public bool Call(string name, params object[] args)
        {
            bool result = true;
            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            LuaApi.lua_pushstring(l, name);
            LuaApi.lua_gettable(l, (int)LuaIndex.LUA_GLOBALSINDEX);

            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = args.Length;
                for (int i = 0; i < count; ++i)
                {
                    PushArg(args[i]);
                }

                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }   
            }
            else
            {
                result = false;
                LastErrorMsg = string.Format("{0} function not found", name);
                Log(LastErrorMsg);
            }            

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(string name, Func<IntPtr,int> OnGetArgs)
        {
            bool result = true;
            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            LuaApi.lua_pushstring(l, name);
            LuaApi.lua_gettable(l, (int)LuaIndex.LUA_GLOBALSINDEX);

            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = OnGetArgs(l);

                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = string.Format("{0} function not found", name);
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(string name, Action<IntPtr> OnReturn,int nReturnCount,params object[] args)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            LuaApi.lua_pushstring(l, name);
            LuaApi.lua_gettable(l, (int)LuaIndex.LUA_GLOBALSINDEX);

            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = args.Length;
                for (int i = 0; i < count; ++i)
                {
                    PushArg(args[i]);
                }

                int error = LuaApi.lua_pcall(l, count, nReturnCount, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
                else
                {
                    OnReturn(l);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = string.Format("{0} function not found", name);
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(string name, Action<IntPtr> OnReturn, int nReturnCount, Func<IntPtr, int> OnGetArgs)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            LuaApi.lua_pushstring(l, name);
            LuaApi.lua_gettable(l, (int)LuaIndex.LUA_GLOBALSINDEX);

            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = OnGetArgs(l);
                int error = LuaApi.lua_pcall(l, count, nReturnCount, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
                else
                {
                    OnReturn(l);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = string.Format("{0} function not found", name);
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaRefFunction func, params object[] args)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction();
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = args.Length;
                for (int i = 0; i < count; ++i)
                {
                    PushArg(args[i]);
                }

                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "reffunction not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaRefFunction func, Func<IntPtr, int> OnGetArgs)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction();
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = OnGetArgs(l);
                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "reffunction not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaRefFunction func, Action<IntPtr> OnReturn, int nReturnCount, params object[] args)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction();
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = args.Length;
                for (int i = 0; i < count; ++i)
                {
                    PushArg(args[i]);
                }

                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
                else
                {
                    OnReturn(l);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "ref function not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaRefFunction func, Action<IntPtr> OnReturn, int nReturnCount, Func<IntPtr, int> OnGetArgs)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction();
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = OnGetArgs(l);
                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
                else
                {
                    OnReturn(l);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "ref function not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaStackFunction func, params object[] args)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction(l);
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = args.Length;
                for (int i = 0; i < count; ++i)
                {
                    PushArg(args[i]);
                }

                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "stack function not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaStackFunction func, Func<IntPtr, int> OnGetArgs)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction(l);
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = OnGetArgs(l);
                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "stack function not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaStackFunction func, Action<IntPtr> OnReturn, int nReturnCount, params object[] args)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction(l);
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = args.Length;
                for (int i = 0; i < count; ++i)
                {
                    PushArg(args[i]);
                }

                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
                else
                {
                    OnReturn(l);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "stack function not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        public bool Call(LuaStackFunction func, Action<IntPtr> OnReturn, int nReturnCount, Func<IntPtr, int> OnGetArgs)
        {
            bool result = true;

            int top = LuaApi.lua_gettop(l);
            stack.BeginCall();

            func.GetFunction(l);
            LuaType type = LuaApi.lua_type(l, -1);
            if (type == LuaType.LUA_TFUNCTION)
            {
                int count = OnGetArgs(l);
                int error = LuaApi.lua_pcall(l, count, 0, 0);
                if (error != 0)
                {
                    result = false;
                    LastErrorMsg = LuaApi.lua_tostring(l, -1);
                    Log(LastErrorMsg);
                }
                else
                {
                    OnReturn(l);
                }
            }
            else
            {
                result = false;
                LastErrorMsg = "stack function not found";
                Log(LastErrorMsg);
            }

            stack.EndCall();
            LuaApi.lua_settop(l, top);

            return result;
        }

        private void PushArg(object o)
        {
            if (o == null)
            {
                LuaApi.lua_pushnumber(l,0);
            }
            else
            {
                TypeCode code = Type.GetTypeCode(o.GetType());
                switch (code)
                {
                    case TypeCode.Empty:
                    case TypeCode.Object:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        {
                            int index = stack.PushObject(o);
                            LuaApi.lua_pushnumber(l, index);
                            break;
                        }
                    case TypeCode.String:
                        {
                            string str = (string)o;
                            LuaApi.lua_pushstring(l, str);
                            break;
                        }
                    default:
                        {
                            double d = Convert.ToDouble(o);
                            LuaApi.lua_pushnumber(l, d);
                            break;
                        }
                }
            }
        }

        class HeapManager
        {
            private int _heapTop;
            private object[] _objects;

            public HeapManager()
            {
                _heapTop = 1;
                _objects = new object[100];
            }

            public int Add(object obj)
            {
                if (_heapTop >= 100)
                    return 0;
                _objects[_heapTop++] = obj;
                return 1 - _heapTop;
            }

            public object Get(int id)
            {
                id = 0 - id;
                if (id <= 0 || id >= _heapTop)
                    return null;
                else
                    return _objects[id];
            }

            public void Set(int id, object obj)
            {
                id = 0 - id;
                if (id <= 0 || id >= _heapTop)
                    return;
                else
                    _objects[id] = obj;
            }
        }

        class StackManager
        {
            class MiniStack<T>
            {
                public int Count;
                public int MaxCount;
                public T[] Stack;

                public MiniStack()
                {
                    Count = 0;
                    MaxCount = 100;
                    Stack = new T[MaxCount];
                }

                public int Push(T arg)
                {
                    if (Count >= MaxCount)
                    {
                        MaxCount += 50;
                        System.Array.Resize<T>(ref Stack, MaxCount);                        
                    }

                    Stack[Count++] = arg;
                    return Count;
                }

                public T Pop()
                {
                    int Top = --Count;
                    T ret = Stack[Top];
                    Stack[Top] = default(T); ;
                    return ret;
                }
            }

            private MiniStack<object> ObjectStack = new MiniStack<object>();
            private MiniStack<int> FrameStack = new MiniStack<int>();

            public void BeginCall()
            {
                int nObjectCount = ObjectStack.Count;
                FrameStack.Push(nObjectCount);
            }

            public void EndCall()
            {
                int nObjectCount = FrameStack.Pop();
                while (ObjectStack.Count > nObjectCount)
                {
                    ObjectStack.Pop();
                }
            }

            public int PushObject(object obj)
            {
                return ObjectStack.Push(obj);
            }

            public object GetObject(int id)
            {
                if (id <= ObjectStack.Count)
                    return ObjectStack.Stack[id - 1];
                else
                    return null;
            }
        }

        [Scriptable]
        static public   LuaManager              Instance { get; internal set; }
               private  IntPtr                  l;
               private  StackManager            stack;
               private  HeapManager             heap;
               private  LuaRegister             register;
               private  Action<string>          OnErrorLog;
               private  List<LuaRefFunction>    funcs;
               public   string                  LastErrorMsg{get;internal set;}
                
    }
}
