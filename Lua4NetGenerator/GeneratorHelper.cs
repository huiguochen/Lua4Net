using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Lua4NetGeneratorNew
{
    class GeneratorHelper
    {
        public static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;
            string name = type.Name;
            int index = name.IndexOf("`");
            name = name.Substring(0, index);
            name += "<";
            int nNextArgIndex = 0;
            List<Type> args = type.GetGenericArguments().ToList();
            while (nNextArgIndex < args.Count)
            {
                Type arg = args[nNextArgIndex];
                name += GetTypeName(arg);
                ++nNextArgIndex;
                if (nNextArgIndex <= (args.Count - 1))
                {
                    name += ",";
                }
            }
            name += ">";
            return name;
        }

        public static void GenerateCSFunction(string definition,Lua4NetSerializer serializer,Action<Lua4NetSerializer> OnGetBody)
        {
            serializer.NewLine(definition);
            serializer.BeginBlock("{");
                OnGetBody(serializer);
            serializer.EndBlock("}");
        }

        public static void GenerateClass(string definition, Lua4NetSerializer serializer, Action<Lua4NetSerializer> OnGetBody)
        {
            serializer.NewLine(definition);
            serializer.BeginBlock("{");
                OnGetBody(serializer);
            serializer.EndBlock("}");
        }

        public static void GetMethodParameter(MethodBase method,Action<MessageFieldType,string,string> OnGetParam)
        {
            List<ParameterInfo> pis = method.GetParameters().ToList();
            foreach (ParameterInfo pi in pis)
            {

            }
        }

        public static string GenerateParameter(List<ParameterInfo> pis)
        {
            StringBuilder sb = new StringBuilder();
            int nNextParamIndex = 0;
            while (nNextParamIndex < pis.Count)
            {
                sb = sb.Append(pis[nNextParamIndex].Name);
                ++nNextParamIndex;
                if (nNextParamIndex <= (pis.Count - 1))
                    sb.Append(",");
            }
            return sb.ToString();
        }

        public static void GenerateMethod(string methodName, Type type, Lua4NetSerializer serializer, Action<List<Type>> OnGetBody)
        {
            serializer.NewLine(methodName + "(");
            List<Type> args = new List<Type>();
            if (type.IsGenericType)
                args = type.GetGenericArguments().ToList();
            else
            {
                (type.GetMembers()[0] as MethodBase).GetParameters().ToList().ForEach(pi =>
                {
                    args.Add(pi.ParameterType);
                }); ;
            }
            int nNextArgIndex = 0;
            while (nNextArgIndex < args.Count)
            {
                Type arg = args[nNextArgIndex];
                serializer.Apppend(GeneratorHelper.GetTypeName(arg) + " arg" + nNextArgIndex.ToString());
                ++nNextArgIndex;
                if (nNextArgIndex <= (args.Count - 1))
                    serializer.Apppend(",");
            }
            serializer.Apppend(")");
            serializer.BeginBlock("{");
            OnGetBody(args);
            serializer.EndBlock("}");
        }

        public static void GenerateMethodCall(string methodName, string[] definedArgs, List<Type> args, Lua4NetSerializer serializer)
        {
            serializer.NewLine(string.Format("{0}(", methodName));

            if (null != definedArgs && definedArgs.Length > 0)
            {
                int definedArgIndex = 0;
                while (definedArgIndex < definedArgs.Length)
                {
                    serializer.Apppend(definedArgs[definedArgIndex]);
                    ++definedArgIndex;
                    while (definedArgIndex <= (definedArgs.Length - 1))
                        serializer.Apppend(",");
                }
            }

            if (null != args && args.Count > 0)
            {
                if (null != definedArgs && definedArgs.Length > 0)
                    serializer.Apppend(",");

                int nNextArgIndex = 0;
                while (nNextArgIndex < args.Count)
                {
                    serializer.Apppend("arg" + nNextArgIndex.ToString());
                    ++nNextArgIndex;
                    if (nNextArgIndex <= (args.Count - 1))
                        serializer.Apppend(",");
                }
            }
            serializer.Apppend(");");
        }
    }
}
