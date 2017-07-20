using System.Linq;
using NHaml.Crosscutting;
using NHaml.Parser.Exceptions;

namespace NHaml.Parser.Rules
{
    public class HamlNodeXmlAttributeCollection : HamlNode
    {
        public HamlNodeXmlAttributeCollection(int sourceFileLineNo, string attributeCollection)
            : base(sourceFileLineNo, attributeCollection)
            
        {
            if (Content[0] != '(' && Content[0] != '{')
                throw new HamlMalformedTagException("AttributeCollection tag must start with an opening bracket or curly bracket.", SourceFileLineNum);

            ParseChildren(attributeCollection);
        }

        protected override bool IsContentGeneratingTag => true;

        private void ParseChildren(string attributeCollection)
        {
            var index = 1;

            var closingBracket = attributeCollection[0] == '{' ? '}' : ')';

            while (index < attributeCollection.Length)
            {
                var nameValuePair = GetNextAttributeToken(attributeCollection, closingBracket, ref index);

                if (!string.IsNullOrEmpty(nameValuePair))
                    AddChild(new HamlNodeXmlAttribute(SourceFileLineNum, nameValuePair));

                index++;
            }
        }

        private static string GetNextAttributeToken(string attributeCollection, char closingBracketChar, ref int index)
        {
            var terminatingChars = new[] { ',', ' ', '\t', closingBracketChar };

            var nameValuePair = HtmlStringHelper.GetNextTagAttributeToken(attributeCollection, ref index, terminatingChars);

            if (terminatingChars.Contains(nameValuePair[nameValuePair.Length - 1]))
                nameValuePair = nameValuePair.Substring(0, nameValuePair.Length - 1);

            return nameValuePair;
        }
    }
}
