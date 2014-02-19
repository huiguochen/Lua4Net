
using System;
using Lua4Net;
using System.Collections.Generic;
using System.Collections;

namespace Lua4Net
{
      public class ObjectLuaServant: LuaRegister
      {
            #region constructor
            
            private static int DefaultConstructor(Object Instance,IntPtr l)
            {
                  Instance = new Object();
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
            private static int GetHashCode(Object Instance,IntPtr l)
            {
                  // get method arguments
                  
                  // call method
                  Int32 methodRetVar = Instance.GetHashCode();
                  
                  LuaApi.lua_pushnumber(l,methodRetVar);
                  return 1;
            }
            
            private static int ToString(Object Instance,IntPtr l)
            {
                  // get method arguments
                  
                  // call method
                  String methodRetVar = Instance.ToString();
                  
                  LuaApi.lua_pushstring(l,methodRetVar);
                  return 1;
            }
            
            #endregion
            
            #region property
            
            #endregion
            
            #region event
            
            #endregion
            
            #region delegate
            
            #endregion
            
            #region register
            
            public override void Register(IntPtr l)
            {
                  LuaApi.lua_pushstring(l,"ObjectServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ObjectServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Object Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  GetHashCode, // methodid = 0
                  ToString, // methodid = 1
                  null
            };
            
            private static int ObjectServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Object obj = LuaManager.Instance.GetObjectT<Object>(nObjectId);
                        
                        // call member function
                        return _Methods[nMethodId](obj,l);
                  }
                  catch(System.Exception ex)
                  {
                        LuaManager.Instance.Log(ex.Message);
                        return 0;
                  }
            }
            
            #endregion
      }
}