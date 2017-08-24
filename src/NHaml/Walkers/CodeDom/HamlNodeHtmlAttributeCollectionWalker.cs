using System.Collections.Generic;
using System.Linq;
using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Parser.Exceptions;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlNodeXmlAttributeCollection"/> nodes.
    /// </summary>
    public class HamlNodeHtmlAttributeCollectionWalker : HamlNodeWalker
    {
        public HamlNodeHtmlAttributeCollectionWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlNodeXmlAttributeCollection"/> node.
        /// </summary>
        /// <param name="node">The <see cref="HamlNodeXmlAttributeCollection"/> node to be rendered to the template class builder.</param>
        /// <exception cref="System.InvalidCastException">Thrown when <paramref name="node"/> is not a <see cref="HamlNodeXmlAttributeCollection"/>.</exception>
        public override void Walk(HamlNode node)
        {
            var attributeCollectionNode = node as HamlNodeXmlAttributeCollection;
            if (attributeCollectionNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlAttributeCollectionWalker requires that HamlNode object be of type HamlNodeXmlAttributeCollection.");

            foreach (var hamlNode in attributeCollectionNode.Children)
            {
                var childNode = (HamlNodeXmlAttribute) hamlNode;
                //if (childNode.Content.StartsWith("class=")
                //    || childNode.Content.StartsWith("id=")) continue;
                MakeAttribute(childNode);
            }
        }

        /// <summary>
        /// Writes the <see cref="HamlNodeXmlAttribute"/> to the class builder.
        /// </summary>
        /// <param name="childNode">The <see cref="HamlNodeXmlAttribute"/> to write to the class builder.</param>
        /// <exception cref="HamlMalformedTagException">Thrown when <paramref name="childNode"/> is not a <see cref="HamlNodeXmlAttribute"/>.</exception>
        private void MakeAttribute(HamlNode childNode)
        {
            var attributeNode = childNode as HamlNodeXmlAttribute;
            if (attributeNode == null)
                throw new HamlMalformedTagException($"Unexpected {childNode.GetType().FullName} tag in AttributeCollection node",
                    childNode.SourceFileLineNum);

            var valueFragments = attributeNode.Children.Any(ch => ch is HamlNodeTextContainer)
                                     ? attributeNode.Children.First().Children
                                     : attributeNode.Children;

            ClassBuilder.AppendAttributeNameValuePair(attributeNode.Name, valueFragments.ToList(), attributeNode.QuoteChar);
        }
    }
}
