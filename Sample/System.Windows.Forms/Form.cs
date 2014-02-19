
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
      public class FormLuaServant: LuaRegister
      {
            #region constructor
            
            private static int Form(Form Instance,IntPtr l)
            {
                  Instance = new Form();
                  int id = LuaManager.Instance.PushStackObject(Instance);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            #endregion
            
            #region method
            
            private static int ResumeLayout(Form Instance,IntPtr l)
            {
                  // get method arguments
                  Boolean performLayout = LuaApi.lua_tonumber(l,3)!=0;
                  
                  // call method
                  Instance.ResumeLayout(performLayout);
                  
                  return 0;
            }
            
            #endregion
            
            #region property
            
            private static int get_ClientSize(Form Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.ClientSize);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_ClientSize(Form Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.ClientSize = LuaManager.Instance.GetObjectT<Size>(id);
                  return 0;
            }
            
            private static int get_Text(Form Instance,IntPtr l)
            {
                  LuaApi.lua_pushstring(l,Instance.Text);
                  return 1;
            }
            private static int set_Text(Form Instance,IntPtr l)
            {
                  Instance.Text = LuaApi.lua_tostring(l,3);
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
                  LuaApi.lua_pushstring(l,"FormServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,FormServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Form Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  Form, // methodid = 0
                  ResumeLayout, // methodid = 1
                  get_ClientSize, // methodid = 2
                  set_ClientSize, // methodid = 3
                  get_Text, // methodid = 4
                  set_Text, // methodid = 5
                  null
            };
            
            private static int FormServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Form obj = LuaManager.Instance.GetObjectT<Form>(nObjectId);
                        
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