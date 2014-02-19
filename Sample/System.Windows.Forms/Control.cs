
using System;
using Lua4Net;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lua4Net
{
      public class ControlLuaServant: LuaRegister
      {
            #region constructor
            
            #endregion
            
            #region method
            
            private static int SuspendLayout(Control Instance,IntPtr l)
            {
                  // get method arguments
                  
                  // call method
                  Instance.SuspendLayout();
                  
                  return 0;
            }
            
            #endregion
            
            #region property
            
            private static int get_Location(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.Location);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_Location(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.Location = LuaManager.Instance.GetObjectT<Point>(id);
                  return 0;
            }
            
            private static int get_Parent(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.Parent);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_Parent(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.Parent = LuaManager.Instance.GetObjectT<Control>(id);
                  return 0;
            }
            
            private static int get_Controls(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.Controls);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int get_Name(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushstring(l,Instance.Name);
                  return 1;
            }
            private static int set_Name(Control Instance,IntPtr l)
            {
                  Instance.Name = LuaApi.lua_tostring(l,3);
                  return 0;
            }
            
            private static int get_Size(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.Size);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_Size(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.Size = LuaManager.Instance.GetObjectT<Size>(id);
                  return 0;
            }
            
            private static int get_TabIndex(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.TabIndex);
                  return 1;
            }
            private static int set_TabIndex(Control Instance,IntPtr l)
            {
                  Instance.TabIndex = (Int32)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_ForeColor(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.ForeColor);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_ForeColor(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.ForeColor = LuaManager.Instance.GetObjectT<Color>(id);
                  return 0;
            }
            
            private static int get_Font(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.Font);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_Font(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.Font = LuaManager.Instance.GetObjectT<Font>(id);
                  return 0;
            }
            
            private static int get_Enabled(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.Enabled?1:0);
                  return 1;
            }
            private static int set_Enabled(Control Instance,IntPtr l)
            {
                  Instance.Enabled = LuaApi.lua_tonumber(l,3)!=0;
                  return 0;
            }
            
            private static int get_Focused(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,Instance.Focused?1:0);
                  return 1;
            }
            
            private static int get_BackColor(Control Instance,IntPtr l)
            {
                  int id = LuaManager.Instance.PushStackObject(Instance.BackColor);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            private static int set_BackColor(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  Instance.BackColor = LuaManager.Instance.GetObjectT<Color>(id);
                  return 0;
            }
            
            private static int get_Dock(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.Dock);
                  return 1;
            }
            private static int set_Dock(Control Instance,IntPtr l)
            {
                  Instance.Dock = (DockStyle)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_Anchor(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.Anchor);
                  return 1;
            }
            private static int set_Anchor(Control Instance,IntPtr l)
            {
                  Instance.Anchor = (AnchorStyles)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            private static int get_Text(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushstring(l,Instance.Text);
                  return 1;
            }
            private static int set_Text(Control Instance,IntPtr l)
            {
                  Instance.Text = LuaApi.lua_tostring(l,3);
                  return 0;
            }
            
            private static int get_RightToLeft(Control Instance,IntPtr l)
            {
                  LuaApi.lua_pushnumber(l,(int)Instance.RightToLeft);
                  return 1;
            }
            private static int set_RightToLeft(Control Instance,IntPtr l)
            {
                  Instance.RightToLeft = (RightToLeft)LuaApi.lua_tonumber(l,3);
                  return 0;
            }
            
            #endregion
            
            #region event
            
            private static int add_Click(Control Instance,IntPtr l)
            {
                  ClickDelegate d = new ClickDelegate(3,l);
                  Instance.Click += d.Call;
                  int id = LuaManager.Instance.PushStackObject(d);
                  LuaApi.lua_pushnumber(l,id);
                  return 1;
            }
            
            private static int remove_Click(Control Instance,IntPtr l)
            {
                  int id = (int)LuaApi.lua_tonumber(l,3);
                  ClickDelegate d = LuaManager.Instance.GetObjectT<ClickDelegate>(id);
                  if(null != d)
                  {
                        Instance.Click -= d.Call;
                  }
                  return 0;
            }
            
            #endregion
            
            #region delegate
            
            class ClickDelegate:LuaRefFunction
            {
                  public ClickDelegate(int ref_,IntPtr l)
                      :base(ref_,l)
                  {
                  }
                  
                  public void Call(Object arg0,EventArgs arg1)
                  {
                        LuaManager.Instance.Call(this,arg0,arg1);
                  }
            }
            
            #endregion
            
            #region register
            
            public override void Register(IntPtr l)
            {
                  LuaApi.lua_pushstring(l,"ControlServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ControlServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Control Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  SuspendLayout, // methodid = 0
                  get_Location, // methodid = 1
                  set_Location, // methodid = 2
                  get_Parent, // methodid = 3
                  set_Parent, // methodid = 4
                  get_Controls, // methodid = 5
                  get_Name, // methodid = 6
                  set_Name, // methodid = 7
                  get_Size, // methodid = 8
                  set_Size, // methodid = 9
                  get_TabIndex, // methodid = 10
                  set_TabIndex, // methodid = 11
                  get_ForeColor, // methodid = 12
                  set_ForeColor, // methodid = 13
                  get_Font, // methodid = 14
                  set_Font, // methodid = 15
                  get_Enabled, // methodid = 16
                  set_Enabled, // methodid = 17
                  get_Focused, // methodid = 18
                  get_BackColor, // methodid = 19
                  set_BackColor, // methodid = 20
                  get_Dock, // methodid = 21
                  set_Dock, // methodid = 22
                  get_Anchor, // methodid = 23
                  set_Anchor, // methodid = 24
                  get_Text, // methodid = 25
                  set_Text, // methodid = 26
                  get_RightToLeft, // methodid = 27
                  set_RightToLeft, // methodid = 28
                  add_Click, // methodid = 29
                  remove_Click, // methodid = 30
                  null
            };
            
            private static int ControlServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Control obj = LuaManager.Instance.GetObjectT<Control>(nObjectId);
                        
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