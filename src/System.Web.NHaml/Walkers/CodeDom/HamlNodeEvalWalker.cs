﻿using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    public sealed class HamlNodeEvalWalker : HamlNodeWalker
    {
        public HamlNodeEvalWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeEval;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeEvalWalker requires that HamlNode object be of type HamlNodeEval.");

            ClassBuilder.AppendCodeToString(node.Content);

            ValidateThereAreNoChildren(node);
        }
    }
}
