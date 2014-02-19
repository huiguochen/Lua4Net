using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Lua4NetGeneratorNew
{
    class NamespaceGenerator
    {
        public static List<string> Generate(MethodBase method)
        {
            List<string> result = new List<string>();
            foreach (ParameterInfo pi in method.GetParameters())
            {
                string name = pi.ParameterType.Namespace;
                if (!result.Contains(name))
                    result.Add(name);
            }

            MethodInfo mi = method as MethodInfo;
            if (null != mi)
            {
                result.AddRange(Generate(mi.ReturnType, false));
            }

            return result;
        }

        public static List<string> Generate(Type type, bool checkbase)
        {
            List<string> result = new List<string>() { type.Namespace };
            foreach (Type tp in type.GetInterfaces())
            {
                string name = tp.Namespace;
                if (!result.Contains(name))
                    result.Add(name);
            }
            if (checkbase && type.BaseType!=null && !result.Contains(type.BaseType.Namespace))
                result.Add(type.BaseType.Namespace);

            return result;
        }

        public static string Generate(string path,ScriptableXml sxml)
        {
            List<string> allNamespaces = new List<string>();
            sxml.Assembly.ForEach(s => 
            {
                if (!string.IsNullOrEmpty(s.Namespace))
                    allNamespaces.Add(s.Namespace);
            });
            Lua4NetSerializer serializer = new Lua4NetSerializer(0);
            allNamespaces.ForEach(n => 
            {
                serializer.NewLine(string.Format("{0} = { }",n));
            });
            return Path.Combine(path, "namespace.lua");
        }
    }
}
