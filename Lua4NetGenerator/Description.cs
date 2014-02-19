using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Lua4NetGeneratorNew
{
    public class ClassDescription
    {
        public Type Class { get; set; }
        public Type BaseClass { get; set; }        
        public bool HasDefaultConstructor { get; set; }
        public bool IsNestedClass{get;set;}
        public string ClassName { get; set; }
        public string BaseClassName { get; set; }
        public bool IsValueType { get; set; }
        public bool IsEnum { get; set; }
        public string ServantCallName { get; set; }
        public string ServantName { get; set; }
        public string ProxyName { get; set; }
        public string CSFileName { get; set; }
        public string LuaFileName { get; set; }
        public List<ConstructorDescription> Constructors { get; set; }        
        public List<PropertyFieldDescription> Propertys { get; set; }
        public List<MethodDescription> Methods { get; set; }
        public List<EventDescription> Events { get; set; }
        public bool HasBaseClassDescription { get; set; }
        public string Namespace { get; set; }        
        public List<Assembly> allAssembly { get; set; }
        public bool aot { get; set; }

        public Type GetType(string typeName)
        {
            Type result = null;
            foreach (Assembly a in allAssembly)
            {
                result = a.GetType(typeName);
                if (null != result)
                    return result;
            }
            return Type.GetType(typeName);
        }

        public string GetNamespaceName(string name)
        {
            if (string.IsNullOrEmpty(Namespace))
                return name;
            else
                return Namespace + "." + name;
        }
        public string GetBaseClassProxyName()
        {
            string proxyName = BaseClass.Name;
            string ns = ScriptableXml.Instance.GetNamespace(BaseClass.Assembly.GetName().Name);
            if (string.IsNullOrEmpty(ns))
                return proxyName;
            else
                return ns + "." + proxyName;
        }
        public ConstructorDescription GetDefaultOrFirstConstructor()
        {
            ConstructorDescription result = null;
            if (null != Constructors&&Constructors.Count>0)
            {
                foreach (var cd in Constructors)
                {
                    if (cd.Default)
                    {
                        result = cd;
                        break;
                    }
                }
                if(result==null)
                    result = Constructors[0];
            }
            return result;
        }
    }

    public enum MessageFieldType
    {
        NumberType,
        StringType,
        DelegateType,
        ClientType,
    }

    public enum NumberType
    {
        Numeric,
        Boolean,
        Enum,
    }

    public class MemberDescription
    {
        public Type RawType { get; set; }
        public bool IsStatic { get; set; }
        public bool IsConstructor { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
    }

    public class MethodDescription : MemberDescription
    {
        public MethodBase Method { get; set; }
        public List<MethodFieldDescription> InputArgs { get; set; }
        public MethodFieldDescription Output { get; set; }

        public string GetOutputProxyName()
        {
            string proxyName = Output.RawType.Name;
            string ns = ScriptableXml.Instance.GetNamespace(Output.RawType.Assembly.GetName().Name);
            if (string.IsNullOrEmpty(ns))
                return proxyName;
            else
                return ns + "." + proxyName;
        }
    }

    public class PropertyFieldDescription:MemberDescription
    {
        public PropertyInfo Pi { get; set; }
        public MessageFieldType Type { get; set; }
        public MethodDescription GetMethod { get; set; }
        public MethodDescription SetMethod { get; set; }
        //[]下标访问属性
        public bool IsItemProperty { get; set; }
        public NumberType GetNumberType()
        {
            if (Pi.PropertyType.IsEnum)
                return NumberType.Enum;
            if (Pi.PropertyType == typeof(Boolean))
                return NumberType.Boolean;
            return NumberType.Numeric;
        }
    }

    public class ConstructorDescription : MethodDescription
    {
        public ConstructorInfo Ci { get; set; }
        public bool Default { get; set; }

        public ConstructorDescription()
        {
            IsConstructor = true;
        }
    }

    public class EventDescription : MemberDescription
    {
        public EventInfo Ei { get; set; }
        public MethodDescription AddMethod{get;set;}
        public MethodDescription RemoveMethod { get; set; }
    }

    public class MethodFieldDescription : MemberDescription
    {
        public MessageFieldType Type { get; set; }
        public NumberType GetNumberType()
        {
            if (RawType.IsEnum)
                return NumberType.Enum;
            if (RawType == typeof(Boolean))
                return NumberType.Boolean;
            return NumberType.Numeric;
        }
        public string GetProxyName()
        {
            return RawType.Name;
        }
    }

    #region xml

    [XmlRoot(ElementName = "config")]
    public class ScriptableXml
    {
        public static ScriptableXml Instance;

        [XmlElement(ElementName = "assembly")]
        public List<ScriptableAssemble> Assembly { get; set; }

        [XmlElement(ElementName = "aot")]
        public bool aot { get; set; }

        public static ScriptableXml Create(string path)
        {            
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ScriptableXml));
                Instance = serializer.Deserialize(fs) as ScriptableXml;
            }
            return Instance;
        }

        public string GetNamespace(string assemblyName)
        {
            assemblyName += ".dll";
            string result = string.Empty;
            foreach (ScriptableAssemble sa in Assembly)
            {
                if (sa.Name == assemblyName)
                {
                    result = sa.Namespace;
                    break;
                }
            }
            return result;
        }
    }

    public class ScriptableAssemble
    {
        [XmlElement(ElementName = "class")]
        public List<ScriptableClass> Class { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Reference { get; set; }

        [XmlAttribute]
        public string Namespace { get; set; }

        public List<string> GetReference()
        {
            if (string.IsNullOrEmpty(Reference))
                return new List<string>();
            else
                return Reference.Split(',').ToList();
        }
    }

    public class ScriptableClass
    {
        [XmlElement(ElementName = "constructor")]
        public List<ScriptableMethod> Constructor { get; set; }

        [XmlElement(ElementName = "property")]
        public List<ScriptableProperty> Property { get; set; }

        [XmlElement(ElementName = "method")]
        public List<ScriptableMethod> Method { get; set; }

        [XmlElement(ElementName = "event")]
        public List<ScriptableEvent> Event { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool HasDefaultConstructor { get; set; }
    }

    public class ScriptableMethod
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string In { get; set; }
        [XmlAttribute]
        public string NickName { get; set; }
        [XmlAttribute]
        public bool DefaultConstructor{ get; set; }

        public Type[] GetArgs(ClassDescription cd)
        {
            List<Type> tps = new List<Type>();
            if (!string.IsNullOrEmpty(In))
            {
                foreach (string s in In.Split(',').ToList())
                {
                    Type tp = cd.GetType(s);
                    if (null != tp)
                    {
                        tps.Add(tp);
                    }                    
                }
            }
            return tps.ToArray();
        }
    }

    public class ScriptableEvent
    {
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class ScriptableProperty
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string In { get; set; }

        [XmlAttribute]
        public string Get { get; set; }

        [XmlAttribute]
        public string Set { get; set; }

        public Type[] GetArgs(ClassDescription cd)
        {
            List<Type> tps = new List<Type>();
            if (!string.IsNullOrEmpty(In))
            {
                foreach (string s in In.Split(',').ToList())
                {
                    Type tp = cd.GetType(s);
                    if (null == tp)
                    {
                        tps.Add(tp);    
                    }
                }
            }
            return tps.ToArray();
        }
    }

    #endregion 
}
