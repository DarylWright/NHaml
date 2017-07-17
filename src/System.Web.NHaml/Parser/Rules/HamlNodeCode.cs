﻿using NHaml.IO;

namespace NHaml.Parser.Rules
{
    public class HamlNodeCode : HamlNode
    {
        public HamlNodeCode(HamlLine line)
            : base(line) { }

        protected override bool IsContentGeneratingTag
        {
            get { return false; }
        }
    }
}
