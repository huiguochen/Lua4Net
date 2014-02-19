
using System;
using Lua4Net;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using System.Drawing;

namespace Lua4Net
{
      public class LabelLuaServant: LuaRegister
      {
            #region constructor
            
            private static int New(Label Instance,IntPtr l)
            {
                  Instance = new Label();
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
            #endregion
            
            #region property
            
            private static int get_TextAlign(Label Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.TextAlign);
                  return 1;
            }
            private static int set_TextAlign(Label Instance,IntPtr l)
            {
                  Instance.TextAlign = (ContentAlignment)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            #endregion
            
            #region event
            
            #endregion
            
            #region delegate
            
            #endregion
            
            #region register
            
            public override void Register(IntPtr l)
            {
                  LuaApi.lua_pushstring(l,"LabelServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,LabelServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Label Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  New, // methodid = 0
                  get_TextAlign, // methodid = 1
                  set_TextAlign, // methodid = 2
                  null
            };
            
            private static int LabelServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Label obj = LuaManager.Instance.GetObjectT<Label>(nObjectId);
                        
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