using System;

namespace NHaml.Parser.Rules
{
    [Obsolete("There are no div tags in XAML.")]
    public class HamlNodeTagId : HamlNode
    {
        public HamlNodeTagId(int sourceFileLineNo, string tagId)
            : base(sourceFileLineNo, tagId)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
