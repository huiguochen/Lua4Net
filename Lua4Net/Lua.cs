using System;
using System.Runtime.InteropServices;

namespace Lua4Net
{
    public enum LuaType
    {
        LUA_TNONE = -1,
        LUA_TNIL = 0,
        LUA_TBOOLEAN = 1,
        LUA_TLIGHTUSERDATA = 2,
        LUA_TNUMBER = 3,
        LUA_TSTRING = 4,
        LUA_TTABLE = 5,
        LUA_TFUNCTION = 6,
        LUA_TUSERDATA = 7,
    }

    public enum LuaIndex
    {
        LUA_GLOBALSINDEX = -10002,
        LUA_ENVIRONINDEX = -10001,
        LUA_REGISTRYINDEX = -10000,
    }

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl,CharSet=CharSet.Ansi)]  
    public delegate int LuaCSFunction(IntPtr l);

    public class LuaApi
    {
        #region lua.h
        // state manipulation
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_newstate();

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_close();

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaCSFunction lua_atpanic(IntPtr l, LuaCSFunction f);

        // basic stack manipulation
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(IntPtr l);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(IntPtr l, int idx);

        // access functions (stack -> C)
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isnumber(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isstring(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_iscfunction(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isuserdata(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_type(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern string lua_typename(IntPtr l, int tp);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_tonumber(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_toboolean(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tolstring(IntPtr l, int idx, IntPtr size_t);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaCSFunction lua_tocfunction(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_touserdata(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_topointer(IntPtr l, int idx);

        // push functions (C -> stack)
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnil(IntPtr l);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(IntPtr l, double n);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_pushstring(IntPtr l, string s);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(IntPtr l, LuaCSFunction fn, int n);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushboolean(IntPtr l, int b);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushlightuserdata(IntPtr l, IntPtr p);

        // get functions (Lua -> stack)
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_gettable(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_getfield(IntPtr l, int idx, string k);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawget(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawgeti(IntPtr l, int idx, int n);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_createtable(IntPtr l, int narr, int nrec);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_newuserdata(IntPtr l, int sz);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getmetatable(IntPtr l, int objindex);

        // set functions (stack -> Lua)
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settable(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void lua_setfield(IntPtr l, int idx, string k);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawset(IntPtr l, int idx);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(IntPtr l, int idx, int n);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_setmetatable(IntPtr l, int objindex);

        // load' and `call' functions (load and run Lua code)
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_pcall(IntPtr l, int nargs, int nresults, int errfunc);

        // miscellaneous functions
        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_error(IntPtr l);

        // some useful macros
        public static void lua_pop(IntPtr l, int n)
        {
            lua_settop(l, -(n) - 1);
        }

        public static void lua_newtable(IntPtr l)
        {
            lua_createtable(l, 0, 0);
        }

        public static void lua_pushcfunction(IntPtr l, LuaCSFunction f)
        {
            lua_pushcclosure(l, f, 0);
        }

        public static string lua_tostring(IntPtr l, int i)
        {
            IntPtr addr = lua_tolstring(l, i, IntPtr.Zero);
            return Marshal.PtrToStringAnsi(addr);
        }
        #endregion

        #region luaxlib.h

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl,CharSet=CharSet.Ansi)]
        public static extern int luaL_newmetatable(IntPtr l,string tname);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_ref(IntPtr l, int t);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_unref(IntPtr l, int t,int _ref);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl,CharSet=CharSet.Ansi)]
        public static extern int luaL_loadfile(IntPtr l, string filename);

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int luaL_loadstring(IntPtr l, string s);

        public static int luaL_dofile(IntPtr l, string fn)
        {
            int result = luaL_loadfile(l, fn);
            if(result!=0)
                return result;
            return lua_pcall(l,0,-1,0);
        }

        public static void luaL_dostring(IntPtr l, string s)
        {
            luaL_loadstring(l, s);
            lua_pcall(l, 0, -1, 0);
        }

        public static void luaL_getmetatable(IntPtr l, string n)
        {
            lua_getfield(l, (int)LuaIndex.LUA_REGISTRYINDEX, n);
        }

        public static int lua_ref(IntPtr l, int lock_)
        {
            if (lock_ != 0)
            {
                return luaL_ref(l, (int)LuaIndex.LUA_REGISTRYINDEX);
            }
            else
            {
                lua_pushstring(l, "unlocked references are obsolete");
                lua_error(l);
                return 0;
            }
        }

        public static void lua_unref(IntPtr l, int ref_)
        {
            luaL_unref(l, (int)LuaIndex.LUA_REGISTRYINDEX, ref_);
        }

        public static void lua_getref(IntPtr l, int ref_)
        {
            lua_rawgeti(l, (int)LuaIndex.LUA_REGISTRYINDEX, ref_);
        }

        #endregion

        #region lualib.h

        [DllImport("lua5.1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_openlibs(IntPtr l);

        #endregion
    }
}
