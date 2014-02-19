
using System;
using Lua4Net;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using System.Collections.Generic;
using System.Collections;

namespace Lua4Net
{
      public class ButtonLuaServant: LuaRegister
      {
            #region constructor
            
            private static int New(Button Instance,IntPtr l)
            {
                  Instance = new Button();
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
            #endregion
            
            #region property
            
            private static int get_Text(Button Instance,IntPtr l)
            {
                  LuaApi.lua_pushstring(l,Instance.Text);
                  return 1;
            }
            private static int set_Text(Button Instance,IntPtr l)
            {
                  Instance.Text = LuaApi.lua_tostring(l,3);
                  return 0;
            }
            
            private static int get_AutoSize(Button Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.AutoSize?1:0);
                  return 1;
            }
            private static int set_AutoSize(Button Instance,IntPtr l)
            {
                  Instance.AutoSize = LuaApi.lua_tonumber(l,3)!=0;
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
                  LuaApi.lua_pushstring(l,"ButtonServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ButtonServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Button Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  New, // methodid = 0
                  get_Text, // methodid = 1
                  set_Text, // methodid = 2
                  get_AutoSize, // methodid = 3
                  set_AutoSize, // methodid = 4
                  null
            };
            
            private static int ButtonServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Button obj = LuaManager.Instance.GetObjectT<Button>(nObjectId);
                        
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