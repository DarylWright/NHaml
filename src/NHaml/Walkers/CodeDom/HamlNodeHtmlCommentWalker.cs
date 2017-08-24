using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeXmlComment"/> nodes.
    /// </summary>
    public sealed class HamlNodeHtmlCommentWalker : HamlNodeWalker
    {
        public HamlNodeHtmlCommentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeXmlComment"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeXmlComment"/> node to be rendered to the template class builder.</param>
        public override void Walk(HamlNode node)
        {
            var commentNode = node as HamlNodeXmlComment;
            if (commentNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlCommentWalker requires that HamlNode object be of type HamlNodeXmlComment.");

            ClassBuilder.Append(node.Indent);
            ClassBuilder.Append("<!--" + commentNode.Content);
       
            base.Walk(node);

            if (node.IsMultiLine)
            {
                ClassBuilder.AppendNewLine();
                ClassBuilder.Append(node.Indent + "-->");
            }
            else
            {
                ClassBuilder.Append(" -->");
            }

        }
    }
}
