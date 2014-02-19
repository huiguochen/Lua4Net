using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lua4NetGeneratorNew
{
    class ProxyGenerator
    {
        public static void GenerateEnum(ClassDescription cd, Lua4NetSerializer serializer)
        {
            Type tp = cd.Class;
            List<string> names = new List<string>();
            List<int> values = new List<int>();

            foreach (string s in Enum.GetNames(tp))
            {
                names.Add(s);
            }

            foreach (int i in Enum.GetValues(tp))
            {
                values.Add(i);
            }

            serializer.NewLine(string.Format("{0} = ", cd.GetNamespaceName(tp.Name)));
            serializer.NewLine("        { ");
            for (int i = 0; i < names.Count; ++i)
            {
                serializer.NewLine(string.Format("              {0} = {1}", names[i], values[i]));
                if (i != (names.Count - 1))
                    serializer.Apppend(",");
            }
            serializer.NewLine("        }");
        }

        public static void Generate(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(csd.GetNamespaceName(csd.ProxyName) + " = { " +  string.Format("__Name = {0}",csd.ProxyName) +" }");

            List<MethodDescription> allMethods = new List<MethodDescription>();
            List<MethodDescription> allFunctions = new List<MethodDescription>();
            allMethods.AddRange(csd.Constructors);
            allFunctions.AddRange(csd.Constructors);
            allMethods.AddRange(csd.Methods);
            allFunctions.AddRange(csd.Methods);
            csd.Propertys.ForEach(p=>
            {
                if (p.GetMethod != null)
                {
                    allMethods.Add(p.GetMethod);
                    if(p.IsItemProperty)
                        allFunctions.Add(p.GetMethod);
                }

                if (p.SetMethod != null)
                {
                    allMethods.Add(p.SetMethod);
                    if (p.IsItemProperty)
                        allFunctions.Add(p.SetMethod);
                }
                 
            });           
            csd.Events.ForEach(e => 
            {
                if (e.AddMethod != null)
                {
                    allMethods.Add(e.AddMethod);
                    allFunctions.Add(e.AddMethod);
                }

                if (e.RemoveMethod != null)
                {
                    allMethods.Add(e.RemoveMethod);
                    allFunctions.Add(e.RemoveMethod);
                }                 
            });

            GenerateGet(csd, serializer);
            GenerateConvertFrom(csd, serializer);
            int id = 0;
            allMethods.ForEach(smd => GenerateMethod(csd, smd, id++, serializer));
            GenerateGetPropertys(csd, serializer);            
            GenerateSetPropertys(csd, serializer);
            GenerateIndex(csd, serializer);
            GenerateNewIndex(csd, serializer);
            GenerateMetaTable(csd, allFunctions, serializer,csd.GetDefaultOrFirstConstructor());
        }

        private static void GenerateGet(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(string.Format("function {0}.Get(id)", csd.GetNamespaceName(csd.ProxyName)));
            serializer.BeginBlock(string.Empty);
                serializer.NewLine("local t = { Id = id }");
                serializer.NewLine(string.Format("setmetatable(t,{0}.MetaTable)", csd.GetNamespaceName(csd.ProxyName)));
                serializer.NewLine("return t");
                serializer.NewLine();
            serializer.EndBlock("end");
        }

        private static void GenerateConvertFrom(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(string.Format("function {0}.ConvertFrom(t)", csd.GetNamespaceName(csd.ProxyName)));
            serializer.BeginBlock(string.Empty);
                serializer.NewLine(string.Format("return {0}.Get(t.Id)", csd.GetNamespaceName(csd.ProxyName)));
            serializer.EndBlock("end");
        }

        private static void GenerateMethod(ClassDescription csd, MethodDescription md, int methodId, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(string.Format("function {0}{1}{2}(", csd.GetNamespaceName(csd.ProxyName), md.IsStatic ? "." : ":", md.NickName));
            int nNextArg = 0;
            while (nNextArg < md.InputArgs.Count)
            {
                serializer.Apppend(md.InputArgs[nNextArg].Name);
                nNextArg++;
                if (nNextArg <= (md.InputArgs.Count - 1))
                    serializer.Apppend(",");
            }
            serializer.Apppend(")");
            serializer.BeginBlock(string.Empty);

            if (md.Output != null)
            {
                if (md.Output.Type == MessageFieldType.ClientType)
                {
                    serializer.NewLine(string.Format("local id = {0}({1},{2}", csd.ServantCallName,
                           md.IsConstructor || md.IsStatic ? "0" : "self.Id",
                           methodId));
                }
                else
                {
                    serializer.NewLine(string.Format("return {0}({1},{2}", csd.ServantCallName,
                                               md.IsConstructor || md.IsStatic ? "0" : "self.Id",
                                               methodId));
                }
            }
            else
            {
                if (md.IsConstructor)
                {
                    serializer.NewLine(string.Format("local id = {0}({1},{2}", csd.ServantCallName,
                                       md.IsConstructor || md.IsStatic ? "0" : "self.Id",
                                       methodId));
                }
                else
                {
                    serializer.NewLine(string.Format("{0}({1},{2}", csd.ServantCallName,
                               md.IsConstructor || md.IsStatic ? "0" : "self.Id",
                               methodId));
                }
            }

            if (md.InputArgs.Count > 0)
                serializer.Apppend(",");
            nNextArg = 0;
            while (nNextArg < md.InputArgs.Count)
            {
                MethodFieldDescription mfd = md.InputArgs[nNextArg];
                if (mfd.Type == MessageFieldType.ClientType)
                {
                    serializer.Apppend(md.InputArgs[nNextArg].Name+".Id");
                }
                else
                {
                    serializer.Apppend(md.InputArgs[nNextArg].Name);
                }                
                nNextArg++;
                if (nNextArg <= (md.InputArgs.Count - 1))
                    serializer.Apppend(",");
            }
            serializer.Apppend(")");

            if (md.Output != null)
            {
                if (md.Output.Type == MessageFieldType.ClientType)
                {
                    serializer.NewLine(string.Format("return {0}.Get(id)", md.GetOutputProxyName()));
                }
            }
            else if (md.IsConstructor)
            {
                serializer.NewLine(string.Format("return {0}.Get(id)", csd.GetNamespaceName(csd.ProxyName)));
            }
            serializer.EndBlock("end");
        }

        private static void GenerateGetPropertys(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(csd.GetNamespaceName(csd.ProxyName)+".__GetProperty = ");
            serializer.BeginBlock("{");
            int nNextProp = 0;
            while (nNextProp < csd.Propertys.Count)
            {
                PropertyFieldDescription pfd = csd.Propertys[nNextProp];
                if (pfd.GetMethod != null)
                {
                    serializer.NewLine(string.Format("{0} = {1}.{2}", pfd.Name, csd.GetNamespaceName(csd.ProxyName), pfd.GetMethod.Name));
                }                
                ++nNextProp;
                if (nNextProp <= (csd.Propertys.Count - 1) && pfd.GetMethod != null)
                {
                    serializer.Apppend(",");
                }
            }
            serializer.EndBlock("}");
        }

        private static void GenerateSetPropertys(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(csd.GetNamespaceName(csd.ProxyName) + ".__SetProperty = ");
            serializer.BeginBlock("{");
            int nNextProp = 0;
            while (nNextProp < csd.Propertys.Count)
            {
                PropertyFieldDescription pfd = csd.Propertys[nNextProp];
                if (pfd.SetMethod != null)
                {
                    serializer.NewLine(string.Format("{0} = {1}.{2}", pfd.Name, csd.GetNamespaceName(csd.ProxyName), pfd.SetMethod.Name));
                }
                ++nNextProp;
                if (nNextProp <= (csd.Propertys.Count - 1) && pfd.SetMethod != null)
                {
                    serializer.Apppend(",");
                }
            }
            serializer.EndBlock("}");
        }

        private static void GenerateIndex(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(string.Format("function {0}.__GetByIndex(t,k)", csd.GetNamespaceName(csd.ProxyName)));
            serializer.BeginBlock("");
                serializer.NewLine(string.Format("local pf = {0}.__GetProperty[k]", csd.GetNamespaceName(csd.ProxyName)));
                serializer.NewLine("if pf ~= nil then");
                serializer.BeginBlock("");
                    serializer.Apppend("    return pf(t)");
                serializer.EndBlock("end");

                serializer.NewLine(string.Format("local f = {0}.__Method[k]", csd.GetNamespaceName(csd.ProxyName)));
                serializer.NewLine("if f ~= nil then");
                serializer.BeginBlock("");
                serializer.Apppend("    return f");
                serializer.EndBlock("end");
                if (csd.HasBaseClassDescription)
                {
                    serializer.NewLine(string.Format("return {0}.__GetByIndex(t,k)", csd.GetBaseClassProxyName()));
                }
                else
                {
                    serializer.NewLine("return nil");
                }
            serializer.EndBlock("end");
        }

        private static void GenerateNewIndex(ClassDescription csd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(string.Format("function {0}.__SetByIndex(t,k,v)", csd.GetNamespaceName(csd.ProxyName)));
            serializer.BeginBlock("");
            serializer.NewLine(string.Format("local pf = {0}.__SetProperty[k]", csd.GetNamespaceName(csd.ProxyName)));
            serializer.NewLine("if pf ~= nil then");
            serializer.BeginBlock("");
            serializer.Apppend("        pf(t,v)");
            serializer.NewLine("return");
            serializer.EndBlock("end");

            if (csd.HasBaseClassDescription)
            {
                serializer.NewLine(string.Format("{0}.__SetByIndex(t,k,v)", csd.GetBaseClassProxyName()));
                serializer.NewLine("return");
            }
            else
            {
                serializer.NewLine("print(k .. ' is not a property')");
            }
            serializer.EndBlock("end");
        }

        private static void GenerateMetaTable(ClassDescription csd,List<MethodDescription> methods, Lua4NetSerializer serializer,MethodDescription ctor)
        {
            serializer.NewLine();
            serializer.NewLine(csd.GetNamespaceName(csd.ProxyName) + ".__Method = { ");
            int nNextMeghod = 0;
            while (nNextMeghod < methods.Count)
            {
                serializer.NewLine(string.Format("          {0} = {1}.{0}", methods[nNextMeghod].NickName, csd.GetNamespaceName(csd.ProxyName)));
                nNextMeghod++;
                if (nNextMeghod <= (methods.Count - 1))
                    serializer.Apppend(",");
            }
            serializer.NewLine("}");

            serializer.NewLine();
            serializer.NewLine(csd.GetNamespaceName(csd.ProxyName));
            serializer.Apppend(".MetaTable = { ");
            serializer.Apppend(string.Format("__index = {0}", csd.GetNamespaceName(csd.ProxyName) + ".__GetByIndex"));
            serializer.Apppend(string.Format(",__newindex = {0}", csd.GetNamespaceName(csd.ProxyName) + ".__SetByIndex"));
            if (ctor != null)
            {
                serializer.Apppend(string.Format(",__call = {0}", csd.GetNamespaceName(csd.ProxyName) + "." + ctor.NickName));
            }            
            serializer.Apppend(" }");
            serializer.NewLine(string.Format("setmetatable({0},{0}.MetaTable)", csd.GetNamespaceName(csd.ProxyName)));
        }
    }
}
