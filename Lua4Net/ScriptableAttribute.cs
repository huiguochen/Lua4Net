using System;

namespace Lua4Net
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor |
                    AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Enum
                    )]
    public class ScriptableAttribute:Attribute
    {
        public string Name { get; set; }
    }
}
