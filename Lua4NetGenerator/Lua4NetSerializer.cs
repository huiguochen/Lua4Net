using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lua4NetGeneratorNew
{
    class Lua4NetSerializer
    {
        private int _indent;
        private StringBuilder _builder = new StringBuilder();
        private int _indentLength = 6;

        public Lua4NetSerializer(int indent)
        {
            _indent = indent;
        }

        public void Apppend(string s)
        {
            _builder.Append(s);
        }

        public void NewLine()
        {
            _builder.AppendLine();
            _builder = _builder.Append(' ', _indent);
            _builder = _builder.Append(string.Empty);
        }

        public void NewLine(string s)
        {
            _builder.AppendLine();
            _builder = _builder.Append(' ', _indent);
            _builder = _builder.Append(s);
        }

        public void BeginBlock(string name)
        {
            _builder.AppendLine();
            _builder = _builder.Append(' ', _indent);
            _builder = _builder.Append(name);
            _indent += _indentLength;
        }

        public void EndBlock(string name)
        {
            _indent -= _indentLength;
            _builder.AppendLine();
            _builder = _builder.Append(' ', _indent);
            _builder = _builder.Append(name);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
