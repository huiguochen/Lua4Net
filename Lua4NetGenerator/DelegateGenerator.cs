using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Lua4NetGeneratorNew
{
    class DelegateGenerator
    {
        private static bool IsDelegate(Type tp)
        {
            return tp.BaseType.Name.Contains("MulticastDelegate") || tp.BaseType.Name.Contains("Delegate");
        }

        public static void Generate(PropertyInfo pi, Lua4NetSerializer serializer)
        {
            if (!IsDelegate(pi.PropertyType))
                return;

            serializer.NewLine();
            serializer.NewLine(string.Format("class {0}Delegate:LuaRefFunction", pi.Name));
            serializer.BeginBlock("{");
            // 构造函数
            serializer.NewLine(string.Format("public {0}Delegate(int ref_,IntPtr l)", pi.Name));
            serializer.NewLine("    :base(ref_,l)");
            serializer.BeginBlock("{");
            serializer.EndBlock("}");

            // 调用函数
            serializer.NewLine();
            GeneratorHelper.GenerateMethod("public void Call", pi.PropertyType, serializer, (args) =>
            {
                GeneratorHelper.GenerateMethodCall("LuaManager.Instance.Call", new string[] { "this" }, args, serializer);
            });
            serializer.EndBlock("}");
        }

        public static void Generate(EventInfo ei, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            serializer.NewLine(string.Format("class {0}Delegate:LuaRefFunction", ei.Name));
            serializer.BeginBlock("{");
            // 构造函数
            serializer.NewLine(string.Format("public {0}Delegate(int ref_,IntPtr l)", ei.Name));
            serializer.NewLine("    :base(ref_,l)");
            serializer.BeginBlock("{");
            serializer.EndBlock("}");

            // 调用函数
            serializer.NewLine();
            GeneratorHelper.GenerateMethod("public void Call", ei.EventHandlerType, serializer, (args) =>
            {
                GeneratorHelper.GenerateMethodCall("LuaManager.Instance.Call", new string[] { "this" }, args, serializer);
            });
            serializer.EndBlock("}");
        }
    }
}
