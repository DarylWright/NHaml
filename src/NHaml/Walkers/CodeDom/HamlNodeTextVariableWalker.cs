using System;
using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeTextVariable"/> nodes.
    /// </summary>
    public class HamlNodeTextVariableWalker : HamlNodeWalker
    {
        public HamlNodeTextVariableWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeTextVariable"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeTextVariable"/> node to be rendered to the template class builder.</param>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="node"/> is not a <see cref="HamlNodeTextVariable"/>.</exception>
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
