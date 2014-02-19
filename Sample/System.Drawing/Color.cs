
using System;
using Lua4Net;
using System.Drawing;

namespace Lua4Net
{
      public class ColorLuaServant: LuaRegister
      {
            #region constructor
            
            private static int DefaultConstructor(Color Instance,IntPtr l)
            {
                  Instance = new Color();
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
            #endregion
            
            #region property
            
            private static int get_Red(Color Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Color.Red);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int get_Green(Color Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Color.Green);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int get_Blue(Color Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Color.Blue);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int get_Yellow(Color Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Color.Yellow);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int get_White(Color Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Color.White);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int get_Black(Color Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Color.Black);
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
                  LuaApi.lua_pushstring(l,"ColorServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ColorServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Color Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  get_Red, // methodid = 0
                  get_Green, // methodid = 1
                  get_Blue, // methodid = 2
                  get_Yellow, // methodid = 3
                  get_White, // methodid = 4
                  get_Black, // methodid = 5
                  null
            };
            
            private static int ColorServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Color obj = LuaManager.Instance.GetObjectT<Color>(nObjectId);
                        
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