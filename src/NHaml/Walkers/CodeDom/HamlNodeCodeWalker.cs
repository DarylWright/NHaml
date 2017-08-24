using System.Linq;
using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeCode"/> nodes.
    /// </summary>
    public class HamlNodeCodeWalker : HamlNodeWalker
    {
        public HamlNodeCodeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeCode"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeCode"/> node to be rendered to the template class builder.</param>
        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeCode;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeCode requires that HamlNode object be of type HamlNodeCode.");

            ClassBuilder.AppendCodeSnippet(node.Content, node.Children.Any());
            
            base.Walk(node);

            if (node.Children.Any())
                ClassBuilder.RenderEndBlock();
        }
    }
}
