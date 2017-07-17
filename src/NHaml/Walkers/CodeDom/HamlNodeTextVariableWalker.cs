using System;
using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    public class HamlNodeTextVariableWalker : HamlNodeWalker
    {
        public HamlNodeTextVariableWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextVariable;
            if (nodeText == null)
                throw new InvalidCastException("HamlNodeTextVariableWalker requires that HamlNode object be of type HamlNodeTextVariable.");

            string variableName = nodeText.VariableName;

            if (nodeText.IsVariableViewDataKey())
                ClassBuilder.AppendVariable(variableName);
            else
                ClassBuilder.AppendCodeToString(variableName);
        }
    }
}
