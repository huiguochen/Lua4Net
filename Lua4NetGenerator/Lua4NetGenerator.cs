using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Lua4NetGeneratorNew
{
    class Lua4NetGenerator
    {
        private Queue<ClassDescription> csds = new Queue<ClassDescription>();
        private Dictionary<Type, ClassDescription> generatedCSD = new Dictionary<Type, ClassDescription>();
        private Dictionary<Type, ClassDescription> allCSD = new Dictionary<Type, ClassDescription>();
        private List<string> allServants = new List<string>();

        public void GenerateServantRoot(string path)
        {
            Lua4NetSerializer serializer = new Lua4NetSerializer(0);
            serializer.NewLine("using System;");
            serializer.NewLine("namespace Lua4Net");
            serializer.BeginBlock("{");

            serializer.NewLine();
            serializer.NewLine("public class Lua4NetRoot : LuaRegister");
            serializer.BeginBlock("{");

            serializer.NewLine("private LuaRegister[] allRegister = ");
            serializer.BeginBlock("{");
            for (int i = 0; i < allServants.Count; ++i)
            {
                serializer.NewLine(string.Format("new {0}()", allServants[i]));
                if (i != (allServants.Count - 1))
                    serializer.Apppend(",");
            }
            serializer.EndBlock("};");

            serializer.NewLine();
            serializer.NewLine("public override void Register(IntPtr l)");
            serializer.BeginBlock("{");
            serializer.NewLine("foreach(LuaRegister r in allRegister)");
            serializer.BeginBlock("{");
            serializer.NewLine("r.Register(l);");
            serializer.EndBlock("}");
            serializer.EndBlock("}");

            serializer.EndBlock("}");

            serializer.EndBlock("}");

            string fullpath = Path.Combine(path, "Lua4NetRoot.cs");
            string content = serializer.ToString();
            SaveAsFile(fullpath, content);
        }
        public void GenerateServant(ClassDescription cd, string path)
        {
            if (!allCSD.ContainsKey(cd.Class))
            {
                if (cd.Class.IsEnum)
                {
                    allCSD[cd.Class] = cd;
                    csds.Enqueue(cd);
                }
                else
                {
                    Generate(path, cd, GenerateServant);
                }
            }           
        }

        private string GenerateServant(ClassDescription cd, Lua4NetSerializer serializer)
        {
           ServantGenerator.Generate(cd, serializer);

           csds.Enqueue(cd);
           allCSD.Add(cd.Class, cd);
           allServants.Add(cd.ServantName);

           return cd.CSFileName;
        }

        private void Generate(string path, ClassDescription cd, Func<ClassDescription, Lua4NetSerializer, string> GetContent)
        {
            Lua4NetSerializer serializer = new Lua4NetSerializer(0);

            List<string> namespaces = new List<string>() { "System", "Lua4Net" };
            GenerateNamespace(cd, ns =>
            {
                foreach (string n in ns)
                {
                    if (!namespaces.Contains(n))
                        namespaces.Add(n);
                }
            });

            namespaces.ForEach(n =>
            {
                serializer.NewLine(string.Format("using {0};", n));
            });

            serializer.NewLine();
            serializer.NewLine(string.Format("namespace {0}", "Lua4Net"));
            serializer.BeginBlock("{");
            string file = GetContent(cd, serializer);
            serializer.EndBlock("}");

            string fullpath = Path.Combine(path, file);
            string content = serializer.ToString();
            Console.WriteLine(content);
            SaveAsFile(fullpath, content);
        }

        private void GenerateNamespace(ClassDescription cd, Action<List<string>> OnAddNamespace)
        {
            OnAddNamespace(NamespaceGenerator.Generate(cd.Class, true));

            cd.Constructors.ForEach(c =>
            {
                OnAddNamespace(NamespaceGenerator.Generate(c.Ci));
            });

            cd.Propertys.ForEach(p =>
            {
                OnAddNamespace(NamespaceGenerator.Generate(p.Pi.PropertyType, false));

                MethodInfo mi = p.Pi.GetGetMethod();
                if (null != mi)
                    OnAddNamespace(NamespaceGenerator.Generate(mi));
                mi = p.Pi.GetSetMethod();
                if (null != mi)
                    OnAddNamespace(NamespaceGenerator.Generate(mi));
            });

            cd.Methods.ForEach(m =>
            {
                OnAddNamespace(NamespaceGenerator.Generate(m.Method));
            });

            cd.Events.ForEach(e =>
            {
                MethodInfo mi = e.Ei.GetAddMethod();
                if (null != mi)
                    OnAddNamespace(NamespaceGenerator.Generate(mi));
                mi = e.Ei.GetRemoveMethod();
                if (null != mi)
                    OnAddNamespace(NamespaceGenerator.Generate(mi));
            });
        }
        private void SaveAsFile(string file, string content)
        {
            string fileName = Path.GetFileName(file);
            string dir = Path.GetDirectoryName(file);

            if (!string.IsNullOrEmpty(dir)&&!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileStream fs = File.Open(file, FileMode.Create);
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
        }

        public void GenerateProxy(string path,ScriptableXml sxml)
        {
            List<string> allfile = new List<string>();
            GenerateProxyImpl(csd =>
            {
                if (csd.IsEnum)
                {
                    GenerateEnum(csd, path);
                    allfile.Add(csd.LuaFileName);
                }
                else
                {
                    Lua4NetSerializer serializer = new Lua4NetSerializer(0);
                    ProxyGenerator.Generate(csd, serializer);

                    string content = serializer.ToString();
                    Console.WriteLine(content);

                    string filename = csd.LuaFileName;
                    string fullpath = Path.Combine(path, filename);
                    allfile.Add(filename);

                    SaveAsFile(fullpath, content);
                }
            });


            Lua4NetSerializer rs = new Lua4NetSerializer(0);
            rs.NewLine();

            List<string> allNamespaces = new List<string>();
            sxml.Assembly.ForEach(s =>
            {
                if (!string.IsNullOrEmpty(s.Namespace))
                {
                    GetNamespace(s.Namespace).ForEach(ns=>
                    {
                        if (!allNamespaces.Contains(ns))
                        {
                            allNamespaces.Add(ns);
                        }
                    });
                }                    
            });

            if (allNamespaces.Count > 0)
            {
                allNamespaces.ForEach(n =>
                {
                    rs.NewLine(n + " = { }");
                });
            }

            allfile.ForEach(s =>
            {
                char ch1 = '\\';
                char ch2 = '/';
                s = s.Replace(ch1, ch2);
                rs.NewLine(string.Format("dofile('{0}')", s));
            });
            rs.NewLine("print('load Lua4NetRoot done!')");
            string rp = Path.Combine(path, "Lua4NetRoot.lua");
            SaveAsFile(rp, rs.ToString());
        }

        private void GenerateProxyImpl(Action<ClassDescription> OnGenerateProxy)
        {
            while (csds.Count > 0)
            {
                ClassDescription csd = csds.Dequeue();

                Type baseClass = csd.BaseClass;
                bool canGenerate = baseClass!=null?false:true;
                while (baseClass != null)
                {
                    // 查看是否有直接基类需要导出
                    if (allCSD.ContainsKey(baseClass))
                    {
                        
                        // 查看基类是否已经导出
                        if (generatedCSD.ContainsKey(baseClass))
                        {
                            canGenerate = true;
                            break;
                        }
                        else
                        {
                            canGenerate = false;
                            break;
                        }
                    }
                    else
                    {
                        // 回溯上一层基类
                        baseClass = baseClass.BaseType;
                    }                    
                }

                if (canGenerate)
                {
                    csd.HasBaseClassDescription = baseClass!=null;
                    csd.BaseClass = baseClass;
                    OnGenerateProxy(csd);
                    generatedCSD.Add(csd.Class, csd);
                }
                else
                {
                    // 放到队尾，等基类导出后在做处理
                    csds.Enqueue(csd);
                }
            }
        }

        public void GenerateEnum(ClassDescription cd, string path)
        {
            Lua4NetSerializer serializer = new Lua4NetSerializer(0);
            ProxyGenerator.GenerateEnum(cd, serializer);
            string fullpath = Path.Combine(path, cd.LuaFileName);
            SaveAsFile(fullpath, serializer.ToString());
        }

        private static List<string> GetNamespace(string s)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(s))
                return result;

            int nStarIndex = 0;
            int nLastStartIndex = nStarIndex;
            while (nStarIndex != -1)
            {
                nLastStartIndex = nStarIndex;
                nStarIndex = s.IndexOf('.', nStarIndex);
                if (nStarIndex != -1)
                {
                    result.Add(s.Substring(0, nStarIndex));
                    nStarIndex += 1;
                }
            }
            result.Add(s);
            return result;
        }
    }
}
