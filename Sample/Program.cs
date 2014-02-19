using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lua4Net;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            LuaManager lm = new LuaManager();
            lm.Initialize(new Lua4NetRoot(), "Lua4NetRoot.lua", "application.lua", null);
            Console.ReadLine();
        }
    }
}
