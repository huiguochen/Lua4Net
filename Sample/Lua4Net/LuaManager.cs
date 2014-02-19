
using System;
using Lua4Net;

namespace Lua4Net
{
      public class LuaManagerLuaServant: LuaRegister
      {
            #region constructor
            
            #endregion
            
            #region method
            
            private static int GetObject(LuaManager Instance,IntPtr l)
            {
                  // get method arguments
                  Int32 id = (Int32)LuaApi.lua_tonumber(l,3);
                  
                  // call method
                  Object methodRetVar = Instance.GetObject(id);
                  
                  int nRetObjectId = LuaManager.Instance.PushStackObject(methodRetVar);
                  LuaApi.lua_pushnumber(l,nRetObjectId);
                  return 1;
            }
            
            private static int AddGlobalObject(LuaManager Instance,IntPtr l)
            {
                  // get method arguments
                  int objId = (int)LuaApi.lua_tonumber(l,3);
                  Object obj = LuaManager.Instance.GetObjectT<Object>(objId);
                  
                  // call method
                  Int32 methodRetVar = Instance.AddHeapObject(obj);
                  
                  LuaApi.lua_pushnumber(l,methodRetVar);
                  return 1;
            }
            
            private static int SetGlobalObject(LuaManager Instance,IntPtr l)
            {
                  // get method arguments
                  Int32 id = (Int32)LuaApi.lua_tonumber(l,3);
                  int objId = (int)LuaApi.lua_tonumber(l,4);
                  Object obj = LuaManager.Instance.GetObjectT<Object>(objId);
                  
                  // call method
                  Instance.SetHeapObject(id,obj);
                  
                  return 0;
            }
            
            private static int Test(LuaManager Instance,IntPtr l)
            {
                  // get method arguments
                  LuaStackFunction lsf = new LuaStackFunction(3);
                  LuaRefFunction lrf = new LuaRefFunction(4,l);
                  
                  // call method
                  Instance.Test(lsf,lrf);
                  
                  return 0;
            }
            
            #endregion
            
            #region property
            
            private static int get_Instance(LuaManager Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(LuaManager.Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region event
            
            #endregion
            
            #region delegate
            
            #endregion
            
            #region register
            
            public override void Register(IntPtr l)
            {
                  LuaApi.lua_pushstring(l,"LuaManagerServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,LuaManagerServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(LuaManager Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  GetObject, // methodid = 0
                  AddGlobalObject, // methodid = 1
                  SetGlobalObject, // methodid = 2
                  Test, // methodid = 3
                  get_Instance, // methodid = 4
                  null
            };
            
            private static int LuaManagerServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        LuaManager obj = LuaManager.Instance.GetObjectT<LuaManager>(nObjectId);
                        
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