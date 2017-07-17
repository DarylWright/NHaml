using System;
using NHaml.IO;

namespace NHaml.Parser.Rules
{
    public class HamlNodeFilter : HamlNode
    {
        public HamlNodeFilter(HamlLine line) : base(line)
        {
            throw new NotImplementedException();
        }

        protected override bool IsContentGeneratingTag => true;
    }
}