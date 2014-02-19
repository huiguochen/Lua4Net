
using System;
using Lua4Net;
using System.Drawing;
using System.Runtime.Serialization;

namespace Lua4Net
{
      public class FontLuaServant: LuaRegister
      {
            #region constructor
            
            private static int New(Font Instance,IntPtr l)
            {
                  string familyName = LuaApi.lua_tostring(l,3);
                  Single emSize = (Single)LuaApi.lua_tonumber(l,4);
                  FontStyle style = (FontStyle)LuaApi.lua_tonumber(l,5);
                  GraphicsUnit unit = (GraphicsUnit)LuaApi.lua_tonumber(l,6);
                  Byte gdiCharSet = (Byte)LuaApi.lua_tonumber(l,7);
                  Instance = new Font(familyName,emSize,style,unit,gdiCharSet);
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
                  LuaApi.lua_pushstring(l,"FontServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,FontServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Font Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  New, // methodid = 0
                  null
            };
            
            private static int FontServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Font obj = LuaManager.Instance.GetObjectT<Font>(nObjectId);
                        
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