using System.Collections.Generic;
using System.Linq;
using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Exceptions;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    public class HamlNodeHtmlAttributeCollectionWalker : HamlNodeWalker
    {
        public HamlNodeHtmlAttributeCollectionWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var attributeCollectionNode = node as HamlNodeHtmlAttributeCollection;
            if (attributeCollectionNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlAttributeCollectionWalker requires that HamlNode object be of type HamlNodeHtmlAttributeCollection.");

            foreach (HamlNodeHtmlAttribute childNode in attributeCollectionNode.Children)
            {
                if (childNode.Content.StartsWith("class=")
                    || childNode.Content.StartsWith("id=")) continue;
                MakeAttribute(childNode);
            }
        }

        private void MakeAttribute(HamlNode childNode)
        {
            var attributeNode = childNode as HamlNodeHtmlAttribute;
            if (attributeNode == null)
                throw new HamlMalformedTagException("Unexpected " + childNode.GetType().FullName + " tag in AttributeCollection node",
                    childNode.SourceFileLineNum);

            var valueFragments = attributeNode.Children.Any(ch => ch is HamlNodeTextContainer)
                                     ? attributeNode.Children.First().Children
                                     : attributeNode.Children;
            ClassBuilder.AppendAttributeNameValuePair(attributeNode.Name, valueFragments.ToList(), attributeNode.QuoteChar);
        }
    }
}
