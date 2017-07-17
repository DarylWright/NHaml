using System.Linq;
using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    public class HamlNodeCodeWalker : HamlNodeWalker
    {
        public HamlNodeCodeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

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
