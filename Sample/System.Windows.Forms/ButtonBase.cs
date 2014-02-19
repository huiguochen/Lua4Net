
using System;
using Lua4Net;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;

namespace Lua4Net
{
      public class ButtonBaseLuaServant: LuaRegister
      {
            #region constructor
            
            #endregion
            
            #region method
            
            #endregion
            
            #region property
            
            private static int get_TextAlign(ButtonBase Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.TextAlign);
                  return 1;
            }
            private static int set_TextAlign(ButtonBase Instance,IntPtr l)
            {
                  Instance.TextAlign = (ContentAlignment)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_AutoEllipsis(ButtonBase Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.AutoEllipsis?1:0);
                  return 1;
            }
            private static int set_AutoEllipsis(ButtonBase Instance,IntPtr l)
            {
                  Instance.AutoEllipsis = LuaApi.lua_tonumber(l,3)!=0;
                  return 0;
            }
            
            private static int get_FlatStyle(ButtonBase Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.FlatStyle);
                  return 1;
            }
            private static int set_FlatStyle(ButtonBase Instance,IntPtr l)
            {
                  Instance.FlatStyle = (FlatStyle)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_ImageIndex(ButtonBase Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.ImageIndex);
                  return 1;
            }
            private static int set_ImageIndex(ButtonBase Instance,IntPtr l)
            {
                  Instance.ImageIndex = (Int32)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_ImageKey(ButtonBase Instance,IntPtr l)
            {
                  LuaApi.lua_pushstring(l,Instance.ImageKey);
                  return 1;
            }
            private static int set_ImageKey(ButtonBase Instance,IntPtr l)
            {
                  Instance.ImageKey = LuaApi.lua_tostring(l,3);
                  return 0;
            }
            
            private static int get_ImeMode(ButtonBase Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.ImeMode);
                  return 1;
            }
            private static int set_ImeMode(ButtonBase Instance,IntPtr l)
            {
                  Instance.ImeMode = (ImeMode)LuaApi.lua_tonumber(l,3);
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
                  LuaApi.lua_pushstring(l,"ButtonBaseServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ButtonBaseServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(ButtonBase Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  get_TextAlign, // methodid = 0
                  set_TextAlign, // methodid = 1
                  get_AutoEllipsis, // methodid = 2
                  set_AutoEllipsis, // methodid = 3
                  get_FlatStyle, // methodid = 4
                  set_FlatStyle, // methodid = 5
                  get_ImageIndex, // methodid = 6
                  set_ImageIndex, // methodid = 7
                  get_ImageKey, // methodid = 8
                  set_ImageKey, // methodid = 9
                  get_ImeMode, // methodid = 10
                  set_ImeMode, // methodid = 11
                  null
            };
            
            private static int ButtonBaseServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        ButtonBase obj = LuaManager.Instance.GetObjectT<ButtonBase>(nObjectId);
                        
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