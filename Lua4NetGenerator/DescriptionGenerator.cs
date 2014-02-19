using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Lua4Net;

namespace Lua4NetGeneratorNew
{
    class DescriptionGenerator
    {
        public static ClassDescription Generate(Assembly a, ScriptableClass sc, List<Assembly> allAssembly)
        {
            ClassDescription result = new ClassDescription();
            result.allAssembly = allAssembly;
            GetBaseDescription(a, result, sc);
            GetConstructor(a, result, sc);
            GetMethod(a, result, sc);
            GetProperty(a, result, sc);
            GetEvent(a, result, sc);
            return result;
        }

        public static ClassDescription Generate(Assembly a, Type type, List<Assembly> allAssembly)
        {
            ClassDescription result = new ClassDescription();
            GetBaseDescription(a, result, type);
            GetConstructor(a, result,type);
            GetMethod(a, result, type);
            GetProperty(a, result, type);
            GetEvent(a, result, type);
            return result;
        }

        private static void GetBaseDescription(Assembly a, ClassDescription result, Type type)
        {            
            Type thisType = type;

            string className = string.Empty;
            if (thisType.IsNested)
            {
                className = thisType.Name;
                className = className.Remove(0, thisType.Namespace.Length + 1);
            }
            else
            {
                className = GeneratorHelper.GetTypeName(thisType);
            }

            result.Class = thisType;
            result.BaseClass = thisType.BaseType;
            result.HasDefaultConstructor = false;
            result.IsValueType = thisType.IsValueType;
            result.IsEnum = thisType.IsEnum;
            result.ClassName = className;            
            result.ServantCallName = result.Class.Name + "Servant4LuaCall";
            result.ServantName = result.Class.Name + "LuaServant";
            result.ProxyName = result.Class.Name;
            string assemblyName = thisType.Assembly.GetName().Name;
            result.CSFileName = Path.Combine(assemblyName, thisType.Name + ".cs");
            result.LuaFileName = Path.Combine(assemblyName, thisType.Name + ".lua");
        }

        private static void GetBaseDescription(Assembly a,ClassDescription result, ScriptableClass sc)
        {
            bool isNestedClass = false;
            string fullname = sc.Name;
            Queue<string> nestedClass = new Queue<string>();
            while (fullname.Length > 0)
            {
                int index = fullname.IndexOf('+');
                if (index == -1)
                {
                    nestedClass.Enqueue(fullname);
                    break;
                }
                else
                {
                    string parentClassName = fullname.Substring(0, index);
                    string nestedClassName = fullname.Substring(index + 1);

                    nestedClass.Enqueue(parentClassName);
                    fullname = nestedClassName;
                    isNestedClass = true;
                }
            }

            Type parentType = null;
            Type thisType = null;            
            string className = string.Empty;
           
            if (isNestedClass)
            {
                while (nestedClass.Count > 0)
                {
                    string typeName = nestedClass.Dequeue();
                    if (parentType == null)
                    {
                        parentType = a.GetType(typeName);
                    }
                    else
                    {
                        thisType = parentType.GetNestedType(typeName);
                    }
                }
                
                className = sc.Name.Replace('+', '.');
                className = className.Remove(0, thisType.Namespace.Length + 1);
            }
            else
            {               
                string typeName = nestedClass.Dequeue();
                thisType = a.GetType(typeName);

                className = GeneratorHelper.GetTypeName(thisType);
            }

            result.Class = thisType;
            result.BaseClass = thisType.BaseType;
            result.HasDefaultConstructor = sc.HasDefaultConstructor;
            result.IsValueType = thisType.IsValueType;
            result.IsEnum = thisType.IsEnum;
            result.ClassName = className;
            result.ServantCallName = result.Class.Name + "Servant4LuaCall";
            result.ServantName = result.Class.Name + "LuaServant";
            result.ProxyName = result.Class.Name;
            string assemblyName = thisType.Assembly.GetName().Name;
            result.CSFileName = Path.Combine(assemblyName,thisType.Name+".cs");
            result.LuaFileName = Path.Combine(assemblyName, thisType.Name + ".lua");
        }

        private static ScriptableAttribute GetAttribute(MemberInfo mi)
        {
            ScriptableAttribute[] attrs = mi.GetCustomAttributes(typeof(ScriptableAttribute), false) as ScriptableAttribute[];
            return attrs.Length > 0 ? attrs[0]:null;
        }

        private static void GetConstructor(Assembly a, ClassDescription result, Type type)
        {
            result.Constructors = new List<ConstructorDescription>();
            foreach (ConstructorInfo ci in type.GetConstructors())
            {
                ScriptableAttribute attr = GetAttribute(ci);
                if (null != attr)
                {
                    ConstructorDescription ctordesc = new ConstructorDescription();
                    ctordesc.Ci = ci;
                    GetMethod(ctordesc, ci, attr.Name, string.Empty);
                    result.Constructors.Add(ctordesc);
                }
            }
        }

        private static void GetConstructor(Assembly a, ClassDescription result, ScriptableClass sc)
        {
            result.Constructors = new List<ConstructorDescription>();            
            foreach (ScriptableMethod ctor in sc.Constructor)
            {
                ConstructorInfo info =  result.Class.GetConstructor(ctor.GetArgs(result));
                if (null != info)
                {
                    ConstructorDescription ctordesc = new ConstructorDescription();
                    ctordesc.Ci = info;
                    ctordesc.Default = ctor.DefaultConstructor;
                    GetMethod(ctordesc, info, string.IsNullOrEmpty(ctor.NickName)?ctor.Name:ctor.NickName,string.Empty);
                    result.Constructors.Add(ctordesc);
                }
                else
                {
                    Console.WriteLine(string.Format("constructor {0} not found,In = {1}", ctor.Name, ctor.In));
                }
            }
        }

        private static void GetMethod(Assembly a, ClassDescription result, Type type)
        {
            result.Methods = new List<MethodDescription>();
            foreach (MethodInfo mi in type.GetMethods())
            {
                ScriptableAttribute attr = GetAttribute(mi);
                if (null != attr)
                {
                    MethodDescription md = new MethodDescription();
                    GetMethod(md, mi, attr.Name, string.Empty);
                    result.Methods.Add(md);
                }
            }
        }

        private static void GetMethod(Assembly a, ClassDescription result, ScriptableClass sc)
        {
            result.Methods = new List<MethodDescription>();
            foreach (ScriptableMethod sm in sc.Method)
            {
                MethodInfo mi = result.Class.GetMethod(sm.Name, sm.GetArgs(result));
                if (null != mi)
                {
                    MethodDescription md = new MethodDescription();
                    GetMethod(md,mi, sm.NickName,string.Empty);
                    result.Methods.Add(md);
                }
                else
                {
                    Console.WriteLine(string.Format("method {0} not found,In={1}", sm.Name, sm.In));
                }
            }
        }

        private static void GetMethod(MethodDescription md,MethodBase mi, string NickName,string returnName)
        {            
            md.Method = mi;
            md.InputArgs = new List<MethodFieldDescription>();
            md.Output = null;
            md.RawType = mi.GetType();
            md.TypeName = GeneratorHelper.GetTypeName(md.RawType);
            md.Name = mi.Name;
            md.NickName = string.IsNullOrEmpty(NickName) ? md.Name : NickName;
            md.IsStatic = mi.IsStatic;            

            foreach (ParameterInfo pi in md.Method.GetParameters())
            {
                MethodFieldDescription mfd = new MethodFieldDescription();
                mfd.Name = pi.Name;
                mfd.RawType = pi.ParameterType;
                mfd.TypeName = GeneratorHelper.GetTypeName(mfd.RawType);
                mfd.IsStatic = false;
                mfd.Type = GetMessageFieldType(mfd.RawType);
                md.InputArgs.Add(mfd);
            }

            MethodInfo mii = md.Method as MethodInfo;
            if (null != mii)
            {
                if (!string.Equals(mii.ReturnType.FullName, "System.Void"))
                {
                    MethodFieldDescription mfd = new MethodFieldDescription();
                    mfd.Name = returnName;
                    mfd.RawType = mii.ReturnType;
                    mfd.TypeName = GeneratorHelper.GetTypeName(mfd.RawType);
                    mfd.IsStatic = false;
                    mfd.Type = GetMessageFieldType(mfd.RawType);
                    md.Output = mfd;
                }
            }
        }

        private static void GetProperty(Assembly a, ClassDescription result, Type type)
        {
            result.Propertys = new List<PropertyFieldDescription>();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ScriptableAttribute attr = GetAttribute(pi);
                if (null != attr)
                {
                    PropertyFieldDescription pfd = new PropertyFieldDescription();
                    pfd.Pi = pi;
                    pfd.IsStatic = false;
                    pfd.RawType = pi.PropertyType;
                    pfd.TypeName = GeneratorHelper.GetTypeName(pfd.RawType);
                    pfd.Name = pi.Name;
                    pfd.Type = GetMessageFieldType(pi.PropertyType);
                    pfd.IsItemProperty = string.Equals(attr.Name, "Item");
                    result.Propertys.Add(pfd);
                    if (pi.GetGetMethod() != null && pi.GetGetMethod().IsStatic)
                    {
                        pfd.IsStatic = true;
                    }
                    if (pi.GetSetMethod() != null && pi.GetSetMethod().IsStatic)
                    {
                        pfd.IsStatic = true;
                    }

                    if (pi.GetGetMethod() != null)
                    {
                        pfd.GetMethod = new MethodDescription();
                        string getName = string.IsNullOrEmpty(attr.Name) ? "get_" + pi.Name : "get_" + attr.Name;
                        GetMethod(pfd.GetMethod, pi.GetGetMethod(), getName, pi.Name);
                    }

                    if (pi.GetSetMethod() != null)
                    {
                        pfd.SetMethod = new MethodDescription();
                        string setName = string.IsNullOrEmpty(attr.Name) ? "set_" + pi.Name : "set_" + attr.Name;
                        GetMethod(pfd.SetMethod, pi.GetSetMethod(), setName, pi.Name);
                    } 
                }                 
            }
        }

        private static void GetProperty(Assembly a, ClassDescription result, ScriptableClass sc)
        {
            result.Propertys = new List<PropertyFieldDescription>();
            foreach (ScriptableProperty sp in sc.Property)
            {
                //
                PropertyInfo pi = result.Class.GetProperty(sp.Name,sp.GetArgs(result));
                if (null != pi)
                {
                    PropertyFieldDescription pfd = new PropertyFieldDescription();
                    pfd.Pi = pi;
                    pfd.IsStatic = false;
                    pfd.RawType = pi.PropertyType;
                    pfd.TypeName = GeneratorHelper.GetTypeName(pfd.RawType);
                    pfd.Name = pi.Name;
                    pfd.Type = GetMessageFieldType(pi.PropertyType);
                    pfd.IsItemProperty = string.Equals(sp.Name, "Item");
                    result.Propertys.Add(pfd);
                    if (pi.GetGetMethod() != null && pi.GetGetMethod().IsStatic)
                    {
                        pfd.IsStatic = true;
                    }
                    if (pi.GetSetMethod() != null && pi.GetSetMethod().IsStatic)
                    {
                        pfd.IsStatic = true;
                    }

                    if (pi.GetGetMethod() != null)
                    {
                        pfd.GetMethod = new MethodDescription();
                        GetMethod(pfd.GetMethod,pi.GetGetMethod(), sp.Get,pi.Name);
                    }

                    if (pi.GetSetMethod() != null)
                    {
                        pfd.SetMethod = new MethodDescription();
                        GetMethod(pfd.SetMethod, pi.GetSetMethod(), sp.Set, pi.Name);
                    }                     
                }
                else
                {
                    Console.WriteLine(string.Format("property {0} not found", sp.Name));
                }
            }
        }

        private static void GetEvent(Assembly a, ClassDescription result, Type type)
        {
            result.Events = new List<EventDescription>();
            foreach (EventInfo ei in type.GetEvents())
            {
                ScriptableAttribute attr = GetAttribute(ei);
                if (null != attr)
                {
                    EventDescription ed = new EventDescription();
                    ed.Ei = ei;
                    ed.RawType = ei.GetType();
                    ed.TypeName = GeneratorHelper.GetTypeName(ei.GetType());
                    ed.IsStatic = true;

                    if (ei.GetAddMethod() != null)
                    {
                        ed.IsStatic = false;
                        ed.AddMethod = new MethodDescription();
                        GetMethod(ed.AddMethod, ei.GetAddMethod(), string.Empty, ei.Name);
                    }
                    if (ei.GetRemoveMethod() != null)
                    {
                        ed.IsStatic = false;
                        ed.RemoveMethod = new MethodDescription();
                        GetMethod(ed.RemoveMethod, ei.GetRemoveMethod(), string.Empty, ei.Name);
                    }
                    result.Events.Add(ed);
                }
            }
        }

        private static void GetEvent(Assembly a, ClassDescription result, ScriptableClass sc)
        {
            result.Events = new List<EventDescription>();            
            foreach (ScriptableEvent se in sc.Event)
            {
                EventInfo ei = result.Class.GetEvent(se.Name);
                if (null != ei)
                {
                    EventDescription ed = new EventDescription();
                    ed.Ei = ei;
                    ed.RawType = ei.GetType();
                    ed.TypeName = GeneratorHelper.GetTypeName(ei.GetType());
                    ed.IsStatic = true;

                    if (ei.GetAddMethod() != null)
                    {
                        ed.IsStatic = false;
                        ed.AddMethod = new MethodDescription();
                        GetMethod(ed.AddMethod,ei.GetAddMethod(), string.Empty,ei.Name);
                    }
                    if (ei.GetRemoveMethod() != null)
                    {
                        ed.IsStatic = false;
                        ed.RemoveMethod = new MethodDescription();
                        GetMethod(ed.RemoveMethod, ei.GetRemoveMethod(), string.Empty, ei.Name);
                    }
                    result.Events.Add(ed);
                }
                else
                {
                    Console.WriteLine(string.Format("event {0} not found", se.Name));
                }
            }
        }

        private static Type[] primitiveTypes = { typeof(sbyte), typeof(byte), typeof(Int16),typeof(UInt16),
                                                 typeof(Int32),typeof(UInt32),typeof(Int64),typeof(UInt64),
                                                 typeof(float),typeof(double),typeof(bool) ,typeof(DateTime)
                                               };
        private static MessageFieldType GetMessageFieldType(Type tp)
        {
            if (primitiveTypes.Contains(tp) || tp.IsEnum)
                return MessageFieldType.NumberType;
            if(tp == typeof(string))
                return MessageFieldType.StringType;
            if (tp.BaseType != null)
            {
                if (tp.BaseType.Name.Contains("MulticastDelegate") || tp.BaseType.Name.Contains("Delegate"))
                    return MessageFieldType.DelegateType;
            }
            return MessageFieldType.ClientType;
        }
    }
}
