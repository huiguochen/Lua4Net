
using System;
namespace Lua4Net
{
      
      public class Lua4NetRoot : LuaRegister
      {
            private LuaRegister[] allRegister = 
            {
                  new ObjectLuaServant(),
                  new LuaManagerLuaServant(),
                  new ColorLuaServant(),
                  new PointLuaServant(),
                  new SizeLuaServant(),
                  new FontLuaServant(),
                  new ControlCollectionLuaServant(),
                  new FormLuaServant(),
                  new ApplicationLuaServant(),
                  new LabelLuaServant(),
                  new ControlLuaServant(),
                  new ButtonBaseLuaServant(),
                  new ButtonLuaServant(),
                  new CheckBoxLuaServant()
            };
            
            public override void Register(IntPtr l)
            {
                  foreach(LuaRegister r in allRegister)
                  {
                        r.Register(l);
                  }
            }
      }
}