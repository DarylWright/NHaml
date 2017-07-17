﻿using NHaml.Compilers;

namespace NHaml.Walkers.CodeDom
{
    public class HamlNodeHamlCommentWalker : HamlNodeWalker
    {
        public HamlNodeHamlCommentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(Parser.HamlNode node)
        { }
    }
}