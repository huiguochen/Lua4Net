
using System;
using Lua4Net;
using System.Windows.Forms;

namespace Lua4Net
{
      public class ApplicationLuaServant: LuaRegister
      {
            #region constructor
            
            #endregion
            
            #region method
            
            private static int Run(Application Instance,IntPtr l)
            {
                  // get method arguments
                  int mainFormId = (int)LuaApi.lua_tonumber(l,3);
                  Form mainForm = LuaManager.Instance.GetObjectT<Form>(mainFormId);
                  
                  // call method
                  Application.Run(mainForm);
                  
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
                  LuaApi.lua_pushstring(l,"ApplicationServant4LuaCall");
                  LuaApi.lua_pushcfunction(l,ApplicationServant4LuaCall);
                  LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);
            }
            
            delegate int Lua4NetFunc(Application Instance,IntPtr l);
            private static Lua4NetFunc[] _Methods=
            {
                  Run, // methodid = 0
                  null
            };
            
            private static int ApplicationServant4LuaCall(IntPtr l)
            {
                  try
                  {
                        // get object and methodid
                        int nObjectId = (int)LuaApi.lua_tonumber(l,1);
                        int  nMethodId = (int)LuaApi.lua_tonumber(l,2);
                        Application obj = LuaManager.Instance.GetObjectT<Application>(nObjectId);
                        
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