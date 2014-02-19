
using System;
using Lua4Net;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using System.Drawing;

namespace Lua4Net
{
      public class CheckBoxLuaServant: LuaRegister
      {
            #region constructor
            
            private static int New(CheckBox Instance,IntPtr l)
            {
                  Instance = new CheckBox();
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
            #endregion
            
            #region property
            
            private static int get_Checked(CheckBox Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.Checked?1:0);
                  return 1;
            }
            private static int set_Checked(CheckBox Instance,IntPtr l)
            {
                  Instance.Checked = LuaApi.lua_tonumber(l,3)!=0;
                  return 0;
            }
            
            private static int get_ThreeState(CheckBox Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.ThreeState?1:0);
                  return 1;
            }
            private static int set_ThreeState(CheckBox Instance,IntPtr l)
            {
                  Instance.ThreeState = LuaApi.lua_tonumber(l,3)!=0;
                  return 0;
            }
            
            private static int get_CheckAlign(CheckBox Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.CheckAlign);
                  return 1;
            }
            private static int set_CheckAlign(CheckBox Instance,IntPtr l)
            {
                  Instance.CheckAlign = (ContentAlignment)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_Appearance(CheckBox Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.Appearance);
                  return 1;
            }
            private static int set_Appearance(CheckBox Instance,IntPtr l)
            {
                  Instance.Appearance = (Appearance)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_CheckState(CheckBox Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.CheckState);
                  return 1;
            }
            private static int set_CheckState(CheckBox Instance,IntPtr l)
            {
                  Instance.CheckState = (CheckState)LuaApi.lua_tonumber(l,3);
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
                  LuaApi.lua_pushstring(l,"CheckBoxServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,CheckBoxServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(CheckBox Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  New, // methodid = 0
                  get_Checked, // methodid = 1
                  set_Checked, // methodid = 2
                  get_ThreeState, // methodid = 3
                  set_ThreeState, // methodid = 4
                  get_CheckAlign, // methodid = 5
                  set_CheckAlign, // methodid = 6
                  get_Appearance, // methodid = 7
                  set_Appearance, // methodid = 8
                  get_CheckState, // methodid = 9
                  set_CheckState, // methodid = 10
                  null
            };
            
            private static int CheckBoxServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        CheckBox obj = LuaManager.Instance.GetObjectT<CheckBox>(nObjectId);
                        
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