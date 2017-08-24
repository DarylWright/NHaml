using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeTextLiteral"/> nodes.
    /// </summary>
    public sealed class HamlNodeTextLiteralWalker : HamlNodeWalker
    {
        public HamlNodeTextLiteralWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeTextLiteral"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeTextLiteral"/> node to be rendered to the template class builder.</param>
        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextLiteral;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextLiteralWalker requires that HamlNode object be of type HamlNodeTextLiteral.");

            string outputText = node.Content;
            outputText = HandleLeadingWhitespace(node, outputText);
            outputText = HandleTrailingWhitespace(node, outputText);

            if (outputText.Length > 0)
                ClassBuilder.Append(outputText);
        }

        private static string HandleTrailingWhitespace(HamlNode node, string outputText)
        {
            if (node.Parent.IsTrailingWhitespaceTrimmed)
                outputText = outputText.TrimEnd(' ', '\n', '\r', '\t');
            return outputText;
        }

        private static string HandleLeadingWhitespace(HamlNode node, string outputText)
        {
            if (node.Parent.IsLeadingWhitespaceTrimmed)
                outputText = outputText.TrimStart(' ', '\n', '\r', '\t');
            return outputText;
        }
    }
}
