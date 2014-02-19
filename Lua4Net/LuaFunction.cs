using System;

namespace Lua4Net
{
    public class LuaRefFunction:IDisposable
    {
        private int _reference;
        private IntPtr _l;

        public LuaRefFunction(int index,IntPtr l)
        {
            LuaApi.lua_pushvalue(l, index);
            _reference = LuaApi.lua_ref(l, index);
            _l = l;
        }

        ~LuaRefFunction()
        {
            if (LuaManager.Instance != null)
            {
                LuaManager.Instance.GC(this); 
            }
        }

        public void Dispose()
        {
            if (_l != IntPtr.Zero && _reference != 0)
            {
                LuaApi.lua_unref(_l, _reference);
                _reference = 0;                
            }
        }

        public void GetFunction()
        {
            LuaApi.lua_getref(_l, _reference);
        }

        public override bool Equals(object o)
        {
            if (o is LuaRefFunction)
            {
                return _reference == ((LuaRefFunction)o)._reference;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _reference;
        }
    }

    public struct LuaStackFunction
    {
        private int _stackIndex;

        public LuaStackFunction(int index)
        {
            _stackIndex = index;
        }

        public void GetFunction(IntPtr l)
        {
            LuaApi.lua_pushvalue(l, _stackIndex);
        }
    }
}
