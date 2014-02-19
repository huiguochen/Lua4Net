
using System;
using Lua4Net;
using System.Windows.Forms;
using System.Collections;
using System.Windows.Forms.Layout;

namespace Lua4Net
{
      public class ControlCollectionLuaServant: LuaRegister
      {
            #region constructor
            
            #endregion
            
            #region method
            
            private static int Add(Control.ControlCollection Instance,IntPtr l)
            {
                  // get method arguments
                  int valueId = (int)LuaApi.lua_tonumber(l,3);
                  Control value = LuaManager.Instance.GetObjectT<Control>(valueId);
                  
                  // call method
                  Instance.Add(value);
                  
                  return 0;
            }
            
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
                  LuaApi.lua_pushstring(l,"ControlCollectionServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ControlCollectionServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Control.ControlCollection Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  Add, // methodid = 0
                  null
            };
            
            private static int ControlCollectionServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Control.ControlCollection obj = LuaManager.Instance.GetObjectT<Control.ControlCollection>(nObjectId);
                        
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