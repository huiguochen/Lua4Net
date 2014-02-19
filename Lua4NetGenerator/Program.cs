using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Lua4NetGeneratorNew
{
    class Program
    {
        static void Main(string[] args)
        {
            string cspath = "cs";
            string luapath = "lua";
            for (int i = 0; i < args.Length - 1; ++i)
            {
                if (string.Equals(args[i], "-cs"))
                {
                    cspath = args[i + 1];
                    i += 1;
                }
                else if (string.Equals(args[i], "-lua"))
                {
                    luapath = args[i + 1];
                    i += 1;
                }
            }
            ScriptableXml xml = ScriptableXml.Create("./config.xml");
            Lua4NetGenerator g = new Lua4NetGenerator();
            foreach (ScriptableAssemble s in xml.Assembly)
            {
                List<Assembly> allAssembly = new List<Assembly>();
                Assembly a = Assembly.LoadFrom(s.Name);
                allAssembly.Add(a);

                foreach (AssemblyName an in a.GetReferencedAssemblies())
                {
                    try
                    {
                        Assembly ra = Assembly.Load(an);
                        allAssembly.Add(ra);    
                    }
                    catch(System.Exception ex)
                    {
                        Console.Write(ex.Message);
                    }                    
                }

                s.Class.ForEach(sc =>
                {
                    ClassDescription cd =  DescriptionGenerator.Generate(a, sc,allAssembly);
                    cd.aot = xml.aot;
                    cd.Namespace = s.Namespace;
                    g.GenerateServant(cd, cspath);
                });

                foreach (Type tp in a.GetTypes())
                {
                    if (tp.GetCustomAttributes(typeof(Lua4Net.ScriptableAttribute), false).Length > 0)
                    {
                        ClassDescription cd = DescriptionGenerator.Generate(a,tp,allAssembly);
                        cd.aot = xml.aot;
                        g.GenerateServant(cd, cspath);
                    }
                }
            }

            g.GenerateServantRoot(cspath);
            g.GenerateProxy(luapath, xml);
            Console.ReadLine();
        }
    }
}
