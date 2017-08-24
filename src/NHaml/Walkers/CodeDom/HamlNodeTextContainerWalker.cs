using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeTextContainer"/> nodes.
    /// </summary>
    public class HamlNodeTextContainerWalker : HamlNodeWalker
    {
        public HamlNodeTextContainerWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeTextContainer"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeCode"/> node to be rendered to the template class builder.</param>
        /// <exception cref="System.InvalidCastException">Thrown when <paramref name="node"/> is not a <see cref="HamlNodeTextContainer"/>.</exception>
        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextContainer;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            RenderIndent(nodeText);

            base.Walk(node);
        }

        /// <summary>
        /// Appends the node's indent if it is not whitespace nor if its leading whitespace is trimmed.
        /// </summary>
        /// <param name="nodeText">The node to render the indent on.</param>
        private void RenderIndent(HamlNode nodeText)
        {
            if (nodeText.IsLeadingWhitespaceTrimmed) return;
            if (nodeText.IsWhitespaceNode() && nodeText.IsTrailingWhitespaceTrimmed) return;
            
            ClassBuilder.Append(nodeText.Indent);
        }
    }
}
