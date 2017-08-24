using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeEval"/> nodes.
    /// </summary>
    public sealed class HamlNodeEvalWalker : HamlNodeWalker
    {
        public HamlNodeEvalWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeEval"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeEval"/> node to be rendered to the template class builder.</param>
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
