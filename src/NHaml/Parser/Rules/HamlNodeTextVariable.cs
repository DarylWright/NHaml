using System;
using System.Linq;

namespace NHaml.Parser.Rules
{
    public class HamlNodeTextVariable : HamlNode
    {
        public HamlNodeTextVariable(int sourceLineNum, string content)
            : base(sourceLineNum, content)
        { }

        protected override bool IsContentGeneratingTag => true;

        public string VariableName => Content.Substring(2, Content.Length - 3);

        public bool IsVariableViewDataKey()
        {
            return VariableName.All(char.IsLetterOrDigit);
        }
    }
}
