using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeDocType"/> nodes.
    /// </summary>
    public sealed class HamlNodeDocTypeWalker : HamlNodeWalker
    {
        public HamlNodeDocTypeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeDocType"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeDocType"/> node to be rendered to the template class builder.</param>
        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeDocType;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeDocTypeWalker requires that HamlNode object be of type HamlNodeDocType.");

            ClassBuilder.AppendDocType(node.Content.Trim());

            ValidateThereAreNoChildren(node);
        }
    }
}
