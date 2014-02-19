using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lua4Net;

namespace Lua4NetGeneratorNew
{
    class ServantGenerator
    {
        public static void Generate(ClassDescription cd, Lua4NetSerializer serializer)
        {
            string classdef = string.Format("public class {0}: LuaRegister", cd.ServantName);
            GeneratorHelper.GenerateClass(classdef, serializer, s => 
            {
                serializer.NewLine("#region constructor");
                if (cd.HasDefaultConstructor)
                    GenerateDefaultConstructor(cd, serializer);
                cd.Constructors.ForEach(c=>GenerateConstructorMethod(cd,c,serializer));
                serializer.NewLine();
                serializer.NewLine("#endregion");

                serializer.NewLine();
                serializer.NewLine("#region method");
                    cd.Methods.ForEach(md => GenerateMethod(cd, md, serializer));
                serializer.NewLine();
                serializer.NewLine("#endregion");

                serializer.NewLine();
                serializer.NewLine("#region property");
                    cd.Propertys.ForEach(pf => GenerateGetSetMethod(cd, pf, serializer));
                serializer.NewLine();
                serializer.NewLine("#endregion");

                serializer.NewLine();
                serializer.NewLine("#region event");
                    cd.Events.ForEach(e => GenerateEvent(cd, e, serializer));
                serializer.NewLine();
                serializer.NewLine("#endregion");

                serializer.NewLine();
                serializer.NewLine("#region delegate");
                    cd.Propertys.ForEach(pf => DelegateGenerator.Generate(pf.Pi, serializer));
                    cd.Events.ForEach(ed => DelegateGenerator.Generate(ed.Ei, serializer));
                serializer.NewLine();
                serializer.NewLine("#endregion");

                serializer.NewLine();
                serializer.NewLine("#region register");

                serializer.NewLine();
                GenerateRegisterFunction(cd, serializer);

                serializer.NewLine();
                GenerateStaticFields(cd,serializer);

                serializer.NewLine();
                GenerateRootMethod(cd, serializer);

                serializer.NewLine();
                serializer.NewLine("#endregion");     
            });
        }

        #region constructor

        private static void GenerateDefaultConstructor(ClassDescription cd, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            string funcdef = string.Format("private static int DefaultConstructor({0} Instance,IntPtr l)", cd.ClassName);
            GeneratorHelper.GenerateCSFunction(funcdef, serializer, s => 
            {
                serializer.NewLine(string.Format("Instance = new {0}();", cd.ClassName));
                serializer.NewLine("int id = LuaManager.Instance.PushStackObject(Instance);");
                serializer.NewLine("LuaApi.lua_pushnumber(l,id);");
                serializer.NewLine("return 1;");
            });
        }

        private static void GenerateConstructorMethod(ClassDescription cd, ConstructorDescription c, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            string ctordef = string.Format("private static int {0}({1} Instance,IntPtr l)",c.NickName,cd.ClassName);
            GeneratorHelper.GenerateCSFunction(ctordef, serializer, s => 
            {
                int nParameterIndex = 3;
                foreach (MethodFieldDescription mfd in c.InputArgs)
                {
                    switch (mfd.Type)
                    {
                        case MessageFieldType.NumberType:
                            {
                                switch (mfd.GetNumberType())
                                {
                                    case NumberType.Boolean:
                                        {
                                            serializer.NewLine(string.Format("{0} {1} = LuaApi.lua_tonumber(l,{2})!=0;", mfd.TypeName, mfd.Name, nParameterIndex));                                            
                                            break;
                                        }
                                    case NumberType.Enum:
                                    case NumberType.Numeric:
                                        {
                                            serializer.NewLine(string.Format("{0} {1} = ({0})LuaApi.lua_tonumber(l,{2});", mfd.TypeName, mfd.Name, nParameterIndex));                                
                                            break;
                                        }
                                }                                
                                break;
                            }
                        case MessageFieldType.StringType:
                            {
                                serializer.NewLine(string.Format("string {0} = LuaApi.lua_tostring(l,{1});", mfd.Name, nParameterIndex));
                                break;
                            }
                        default:
                            {
                                serializer.NewLine(string.Format("int {0}id = (int)LuaApi.lua_tonumber(l,{1});", mfd.Name, nParameterIndex));
                                serializer.NewLine(string.Format("{0} {1} = LuaManager.Instance.GetObjectT<{0}>({1}id);", mfd.TypeName, mfd.Name));
                                break;
                            }
                    }
                    ++nParameterIndex;
                }

                serializer.NewLine(string.Format("Instance = new {0}({1});", cd.ClassName, GeneratorHelper.GenerateParameter(c.Ci.GetParameters().ToList())));
                serializer.NewLine("int id = LuaManager.Instance.PushStackObject(Instance);");
                serializer.NewLine("LuaApi.lua_pushnumber(l,id);");
                serializer.NewLine("return 1;");   
            });


        }
        #endregion

        #region method
        private static void GenerateMethod(ClassDescription cd, MethodDescription md, Lua4NetSerializer serializer)
        {
            serializer.NewLine();
            string funcdef = string.Format("private static int {0}({1} Instance,IntPtr l)", md.NickName, cd.ClassName);
            GeneratorHelper.GenerateCSFunction(funcdef, serializer, s => 
            {
                string classOrInstance = md.IsStatic ? cd.ClassName : "Instance";
                serializer.NewLine("// get method arguments");
                int argIndex = 3;
                foreach (MethodFieldDescription mdf in md.InputArgs)
                {
                    switch (mdf.Type)
                    {
                        case MessageFieldType.NumberType:
                            {
                                switch (mdf.GetNumberType())
                                {
                                    case NumberType.Boolean:
                                        {
                                            serializer.NewLine(string.Format("{0} {1} = LuaApi.lua_tonumber(l,{2})!=0;", mdf.TypeName, mdf.Name, argIndex));
                                            break;
                                        }
                                    case NumberType.Enum:
                                    case NumberType.Numeric:
                                        {
                                            serializer.NewLine(string.Format("{0} {1} = ({0})LuaApi.lua_tonumber(l,{2});", mdf.TypeName, mdf.Name, argIndex));
                                            break;
                                        }
                                }      
                                break;
                            }
                        case MessageFieldType.StringType:
                            {
                                serializer.NewLine(string.Format("string {0} = LuaApi.lua_tostring(l,{1});", mdf.Name, argIndex));
                                break;
                            }
                        case MessageFieldType.DelegateType:
                        case MessageFieldType.ClientType:
                            {
                                if (mdf.RawType == typeof(LuaStackFunction))
                                {
                                    serializer.NewLine(string.Format("LuaStackFunction {0} = new LuaStackFunction({1});", mdf.Name, argIndex));
                                }
                                else if (mdf.RawType == typeof(LuaRefFunction))
                                {
                                    serializer.NewLine(string.Format("LuaRefFunction {0} = new LuaRefFunction({1},l);", mdf.Name, argIndex));
                                }
                                else
                                {
                                    serializer.NewLine(string.Format("int {0}Id = (int)LuaApi.lua_tonumber(l,{1});", mdf.Name, argIndex));
                                    serializer.NewLine(string.Format("{0} {1} = LuaManager.Instance.GetObjectT<{0}>({1}Id);", mdf.TypeName, mdf.Name));
                                }
                                break;
                            }
                    }
                    ++argIndex;
                }
                serializer.NewLine();
                serializer.NewLine("// call method");
                if (md.Output == null)
                {
                    if (md.IsStatic)
                    {
                        serializer.NewLine(string.Format("{0}.{1}(", cd.ClassName, md.Name));
                    }
                    else
                    {
                        serializer.NewLine(string.Format("Instance.{0}(", md.Name));
                    }
                }
                else
                {
                    if (md.IsStatic)
                    {
                        serializer.NewLine(string.Format("{2} methodRetVar = {0}.{1}(", cd.ClassName, md.Name, md.Output.TypeName));
                    }
                    else
                    {
                        serializer.NewLine(string.Format("{2} methodRetVar = Instance.{1}(", cd.ClassName, md.Name, md.Output.TypeName));
                    }
                }

                int nNextArgIndex = 0;
                while (nNextArgIndex < md.InputArgs.Count)
                {
                    MethodFieldDescription mfd = md.InputArgs[nNextArgIndex];
                    serializer.Apppend(mfd.Name);
                    ++nNextArgIndex;
                    if (nNextArgIndex <= (md.InputArgs.Count - 1))
                        serializer.Apppend(",");
                }
                serializer.Apppend(");");

                serializer.NewLine();
                if (md.Output != null)
                {
                    switch (md.Output.Type)
                    {
                        case MessageFieldType.NumberType:
                            {
                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,{0}methodRetVar);", string.Empty));
                                break;
                            }
                        case MessageFieldType.StringType:
                            {
                                serializer.NewLine("LuaApi.lua_pushstring(l,methodRetVar);");
                                break;
                            }
                        case MessageFieldType.DelegateType:
                        case MessageFieldType.ClientType:
                            {
                                serializer.NewLine(string.Format("int nRetObjectId = LuaManager.Instance.PushStackObject(methodRetVar);"));
                                serializer.NewLine("LuaApi.lua_pushnumber(l,nRetObjectId);");
                                break;
                            }
                    }

                    serializer.NewLine("return 1;");
                }
                else
                {
                    serializer.NewLine("return 0;");
                }
            });
        }
        #endregion

        #region property
        private static void GenerateGetSetMethod(ClassDescription cd, PropertyFieldDescription pfd, Lua4NetSerializer serializer)
        {
            #region get
            if (pfd.GetMethod != null)
            {
                serializer.NewLine();
                string funcdef = string.Format("private static int {0}({1} Instance,IntPtr l)", pfd.GetMethod.NickName, cd.ClassName);
                GeneratorHelper.GenerateCSFunction(funcdef, serializer, s => 
                {
                    string classOrInstance = pfd.IsStatic ? cd.ClassName : "Instance";
                    MethodFieldDescription mfd = pfd.GetMethod.Output;
                    if (!pfd.IsItemProperty)
                    {
                        switch (pfd.GetMethod.Output.Type)
                        {
                            case MessageFieldType.NumberType:
                                {
                                    switch (mfd.GetNumberType())
                                    {
                                        case NumberType.Boolean:
                                            {
                                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,{1}.{0}?1:0);", pfd.Name, classOrInstance));
                                                break;
                                            }
                                        case NumberType.Enum:
                                            {
                                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,(int){1}.{0});", pfd.Name, classOrInstance));
                                                break;
                                            }
                                        case NumberType.Numeric:
                                            {
                                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,{1}.{0});", pfd.Name, classOrInstance));
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MessageFieldType.StringType:
                                {
                                    serializer.NewLine(string.Format("LuaApi.lua_pushstring(l,{1}.{0});", pfd.Name, classOrInstance));
                                    break;
                                }
                            case MessageFieldType.DelegateType:
                            case MessageFieldType.ClientType:
                                {
                                    serializer.NewLine(string.Format("int id = LuaManager.Instance.PushStackObject({1}.{0});", pfd.Name, classOrInstance));
                                    serializer.NewLine("LuaApi.lua_pushnumber(l,id);");
                                    break;
                                }
                        }
                    }
                    else
                    {
                        MethodFieldDescription input = pfd.GetMethod.InputArgs[0];
                        switch (input.Type)
                        {
                            case MessageFieldType.NumberType:
                                {
                                    serializer.NewLine("int index = (int)LuaApi.lua_tonumber(l,3);");
                                    break;
                                }
                            case MessageFieldType.StringType:
                                {
                                    serializer.NewLine("string index = LuaApi.lua_tostring(l,3);");
                                    break;
                                }
                            case MessageFieldType.DelegateType:
                            case MessageFieldType.ClientType:
                                {
                                    serializer.NewLine("not supported....");
                                    break;
                                }
                        }

                        switch (pfd.GetMethod.Output.Type)
                        {
                            case MessageFieldType.NumberType:
                                {
                                    switch (mfd.GetNumberType())
                                    {
                                        case NumberType.Boolean:
                                            {
                                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,{0}[index]?1:0);", classOrInstance));
                                                break;
                                            }
                                        case NumberType.Enum:
                                            {
                                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,(int){0}[index]);", classOrInstance));
                                                break;
                                            }
                                        case NumberType.Numeric:
                                            {
                                                serializer.NewLine(string.Format("LuaApi.lua_pushnumber(l,{0}[index]);",classOrInstance));
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MessageFieldType.StringType:
                                {
                                    serializer.NewLine(string.Format("LuaApi.lua_pushstring(l,{0}[index]);", classOrInstance));
                                    break;
                                }
                            case MessageFieldType.DelegateType:
                            case MessageFieldType.ClientType:
                                {
                                    serializer.NewLine(string.Format("int id = LuaManager.Instance.PushStackObject({0}[index]);", classOrInstance));
                                    serializer.NewLine("LuaApi.lua_pushnumber(l,id);");
                                    break;
                                }
                        }
                    }
                    serializer.NewLine("return 1;");
                });                
            }
            #endregion

            #region set
            if (pfd.SetMethod != null)
            {
                string funcdef = string.Format("private static int {0}({1} Instance,IntPtr l)", pfd.SetMethod.NickName, cd.ClassName);
                GeneratorHelper.GenerateCSFunction(funcdef, serializer, s => 
                {
                    string classOrInstance = pfd.IsStatic ? cd.ClassName : "Instance";
                    foreach (MethodFieldDescription mfd in pfd.SetMethod.InputArgs)
                    {
                        switch (mfd.Type)
                        {
                            case MessageFieldType.NumberType:
                                {
                                    switch (mfd.GetNumberType())
                                    {
                                        case NumberType.Boolean:
                                            {
                                                serializer.NewLine(string.Format("{0}.{1} = LuaApi.lua_tonumber(l,3)!=0;", classOrInstance, pfd.Name));
                                                break;
                                            }
                                        case NumberType.Enum:
                                        case NumberType.Numeric:
                                            {
                                                serializer.NewLine(string.Format("{0}.{1} = ({2})LuaApi.lua_tonumber(l,3);", classOrInstance, pfd.Name, pfd.TypeName));
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MessageFieldType.StringType:
                                {
                                    serializer.NewLine(string.Format("{0}.{1} = LuaApi.lua_tostring(l,3);", classOrInstance, pfd.Name));
                                    break;
                                }
                            case MessageFieldType.DelegateType:
                            case MessageFieldType.ClientType:
                                {
                                    serializer.NewLine("int id = (int)LuaApi.lua_tonumber(l,3);");
                                    serializer.NewLine(string.Format("{0}.{1} = LuaManager.Instance.GetObjectT<{2}>(id);", classOrInstance, pfd.Name, pfd.TypeName));
                                    break;
                                }
                        }
                    }

                    serializer.NewLine("return 0;");
                });
            }
            #endregion
        }
        #endregion

        #region event
        private static void GenerateEvent(ClassDescription cd, EventDescription ed, Lua4NetSerializer serializer)
        {
            #region add
            {
                serializer.NewLine();
                string methodName = ed.Ei.GetAddMethod().Name;
                string funcdef = string.Format("private static int {0}({1} Instance,IntPtr l)", methodName, cd.ClassName);
                GeneratorHelper.GenerateCSFunction(funcdef, serializer, s =>
                {
                    string classOrInstance = ed.IsStatic ? cd.ClassName : "Instance";
                    serializer.NewLine(string.Format("{0}Delegate d = new {0}Delegate(3,l);", ed.Ei.Name));
                    serializer.NewLine(string.Format("{0}.{1} += d.Call;",classOrInstance,ed.Ei.Name));
                    serializer.NewLine("int id = LuaManager.Instance.PushStackObject(d);");
                    serializer.NewLine("LuaApi.lua_pushnumber(l,id);");
                    serializer.NewLine("return 1;");
                });
            }
            #endregion

            #region remove
            {
                serializer.NewLine();
                string methodName = ed.Ei.GetRemoveMethod().Name;
                string funcdef = string.Format("private static int {0}({1} Instance,IntPtr l)", methodName, cd.ClassName);
                GeneratorHelper.GenerateCSFunction(funcdef, serializer, s => 
                {
                    serializer.NewLine("int id = (int)LuaApi.lua_tonumber(l,3);");
                    serializer.NewLine(string.Format("{0}Delegate d = LuaManager.Instance.GetObjectT<{0}Delegate>(id);", ed.Ei.Name));
                    serializer.NewLine("if(null != d)");
                    serializer.BeginBlock("{");
                        serializer.NewLine(string.Format("Instance.{0} -= d.Call;", ed.Ei.Name));
                    serializer.EndBlock("}");
                    serializer.NewLine("return 0;");
                });
            }
            #endregion
        }
        #endregion

        #region register
        private static void GenerateRegisterFunction(ClassDescription cd, Lua4NetSerializer serializer)
        {
            serializer.NewLine(string.Format("public override void Register(IntPtr l)"));
            serializer.BeginBlock("{");
                serializer.NewLine(string.Format("LuaApi.lua_pushstring(l,\"{0}\");", cd.ServantCallName));
                serializer.NewLine(string.Format("LuaApi.lua_pushcfunction(l,{0});", cd.ServantCallName));
                serializer.NewLine(string.Format("LuaApi.lua_settable(l,(int)LuaIndex.LUA_GLOBALSINDEX);"));
            serializer.EndBlock("}");
        }

        private static void GenerateStaticFields(ClassDescription cd,Lua4NetSerializer serializer)
        {
            serializer.NewLine(string.Format("delegate int Lua4NetFunc({0} Instance,IntPtr l);", cd.ClassName));
            serializer.NewLine("private static Lua4NetFunc[] _Methods=");
            serializer.BeginBlock("{");
            int methodid = 0;
            foreach (ConstructorDescription ctor in cd.Constructors)
            {
                serializer.NewLine(ctor.NickName+",");
                serializer.Apppend(string.Format(" // methodid = {0}", methodid++));
            }
            foreach (MethodDescription md in cd.Methods)
            {
                serializer.NewLine(md.NickName + ",");
                serializer.Apppend(string.Format(" // methodid = {0}", methodid++));
            }
            foreach (PropertyFieldDescription pfd in cd.Propertys)
            {
                if (pfd.GetMethod != null)
                {
                    serializer.NewLine(pfd.GetMethod.NickName + ",");
                    serializer.Apppend(string.Format(" // methodid = {0}", methodid++));
                }
                if (pfd.SetMethod != null)
                {
                    serializer.NewLine(pfd.SetMethod.NickName + ",");
                    serializer.Apppend(string.Format(" // methodid = {0}", methodid++));
                }
            }
            foreach (EventDescription ed in cd.Events)
            {
                if (ed.AddMethod != null)
                {
                    serializer.NewLine(ed.AddMethod.Name + ",");
                    serializer.Apppend(string.Format(" // methodid = {0}", methodid++));
                }
                if (ed.RemoveMethod != null)
                {
                    serializer.NewLine(ed.RemoveMethod.Name + ",");
                    serializer.Apppend(string.Format(" // methodid = {0}", methodid++));
                }
            }
            serializer.NewLine("null");
            serializer.EndBlock("};");
        }

        private static void GenerateRootMethod(ClassDescription cd, Lua4NetSerializer serializer)
        {
            if (cd.aot)
            {
                serializer.NewLine("[AOT.MonoPInvokeCallback(typeof(LuaCSFunction))]");
            }            
            serializer.NewLine(string.Format("private static int {0}(IntPtr l)", cd.ServantCallName));
            serializer.BeginBlock("{");
            serializer.NewLine("try");

            serializer.BeginBlock("{");

            serializer.NewLine("// get object and methodid");
            serializer.NewLine(string.Format("int nObjectId = (int)LuaApi.lua_tonumber(l,1);", cd.ClassName));
            serializer.NewLine("int  nMethodId = (int)LuaApi.lua_tonumber(l,2);");
            serializer.NewLine(string.Format("{0} obj = LuaManager.Instance.GetObjectT<{0}>(nObjectId);", cd.ClassName));

            serializer.NewLine();
            serializer.NewLine("// call member function");
            serializer.NewLine(string.Format("return _Methods[nMethodId](obj,l);", cd.ClassName));

            serializer.EndBlock("}");

            serializer.NewLine("catch(System.Exception ex)");
            serializer.BeginBlock("{");
                serializer.NewLine("LuaManager.Instance.Log(ex.Message);");
                serializer.NewLine("return 0;");
            serializer.EndBlock("}");
            serializer.EndBlock("}");
        }
        #endregion
    }
}
