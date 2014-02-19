
using System;
using Lua4Net;
using System.Drawing;

namespace Lua4Net
{
      public class SizeLuaServant: LuaRegister
      {
            #region constructor
            
            private static int NewByPont(Size Instance,IntPtr l)
            {
                  int ptid = (int)LuaApi.lua_tonumber(l,3);
                  Point pt = LuaManager.Instance.GetObjectT<Point>(ptid);
                  Instance = new Size(pt);
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int New(Size Instance,IntPtr l)
            {
                  Int32 width = (Int32)LuaApi.lua_tonumber(l,3);
                  Int32 height = (Int32)LuaApi.lua_tonumber(l,4);
                  Instance = new Size(width,height);
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
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
                  LuaApi.lua_pushstring(l,"SizeServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,SizeServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Size Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  NewByPont, // methodid = 0
                  New, // methodid = 1
                  null
            };
            
            private static int SizeServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Size obj = LuaManager.Instance.GetObjectT<Size>(nObjectId);
                        
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