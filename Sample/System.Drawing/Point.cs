
using System;
using Lua4Net;
using System.Drawing;

namespace Lua4Net
{
      public class PointLuaServant: LuaRegister
      {
            #region constructor
            
            private static int NewBySize(Point Instance,IntPtr l)
            {
                  int szid = (int)LuaApi.lua_tonumber(l,3);
                  Size sz = LuaManager.Instance.GetObjectT<Size>(szid);
                  Instance = new Point(sz);
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int New(Point Instance,IntPtr l)
            {
                  Int32 x = (Int32)LuaApi.lua_tonumber(l,3);
                  Int32 y = (Int32)LuaApi.lua_tonumber(l,4);
                  Instance = new Point(x,y);
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
                  LuaApi.lua_pushstring(l,"PointServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,PointServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Point Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  NewBySize, // methodid = 0
                  New, // methodid = 1
                  null
            };
            
            private static int PointServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Point obj = LuaManager.Instance.GetObjectT<Point>(nObjectId);
                        
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