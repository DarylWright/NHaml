using System;

namespace NHaml.Parser.Rules
{
    [Obsolete("There are no div tags in XAML.")]
    public class HamlNodeTagClass : HamlNode
    {
        public HamlNodeTagClass(int sourceFileLineNo, string className)
            : base(sourceFileLineNo, className)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
