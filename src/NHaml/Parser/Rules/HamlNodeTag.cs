using System;
using NHaml.Crosscutting;
using NHaml.Parser.Exceptions;

namespace NHaml.Parser.Rules
{
    public enum WhitespaceRemoval
    {
        None = 0, Surrounding, Internal
    }

    public class HamlNodeTag : HamlNode
    {
        public HamlNodeTag(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            IsSelfClosing = false;
            int pos = 0;

            SetNamespaceAndTagName(nodeLine.Content, ref pos);
            ParseViewPropertyNodes(nodeLine.Content, ref pos);
            ParseAttributes(nodeLine.Content, ref pos);
            ParseSpecialCharacters(nodeLine.Content, ref pos);
            HandleInlineContent(nodeLine.Content, ref pos);
        }

        protected override bool IsContentGeneratingTag => true;

        private void SetNamespaceAndTagName(string content, ref int pos)
        {
            TagName = GetTagName(content, ref pos);
            
            if (pos < content.Length
                && content[pos] == ':'
                && IsSelfClosing == false)
            {
                pos++;
                Namespace = TagName;
                TagName = GetTagName(content, ref pos);
            }
        }

        private void ParseViewPropertyNodes(string content, ref int pos)
        {
            while (pos < content.Length)
            {
                if (content[pos] == '.')
                    ParseViewPropertyNode(content, ref pos);
                else
                    return;
            }
        }

        private void ParseAttributes(string content, ref int pos)
        {
            if (pos >= content.Length) return;

            var attributeEndChar = HtmlStringHelper.GetAttributeTerminatingChar(content[pos]);

            if (attributeEndChar != '\0')
            {
                string attributes = HtmlStringHelper.GetNextTagAttributeToken(content, ref pos,
                                                                               new[] {attributeEndChar});
                if (attributes[attributes.Length - 1] != attributeEndChar)
                    throw new HamlMalformedTagException(
                        "Malformed HTML Attributes collection \"" + attributes + "\".", SourceFileLineNum);
                AddChild(new HamlNodeXmlAttributeCollection(SourceFileLineNum, attributes));

                pos++;
            }
        }

        private void ParseSpecialCharacters(string content, ref int pos)
        {
            WhitespaceRemoval = WhitespaceRemoval.None;
            IsSelfClosing = false;

            while (pos < content.Length)
            {
                if (ParseWhitespaceRemoval(content, ref pos)) continue;
                if (ParseSelfClosing(content, ref pos)) continue;
                
                break;
            }
        }

        private bool ParseWhitespaceRemoval(string content, ref int pos)
        {
            if (WhitespaceRemoval != WhitespaceRemoval.None)
                return false;

            switch (content[pos])
            {
                case '>':
                    WhitespaceRemoval = WhitespaceRemoval.Surrounding;
                    pos++;
                    return true;
                case '<':
                    WhitespaceRemoval = WhitespaceRemoval.Internal;
                    pos++;
                    return true;
            }
            return false;
        }

        private bool ParseSelfClosing(string content, ref int pos)
        {
            if (IsSelfClosing || content[pos] != '/')
                return false;

            IsSelfClosing = true;
            pos++;
            return true;
        }

        private void HandleInlineContent(string content, ref int pos)
        {
            if (pos >= content.Length) return;

            var contentLine = content.Substring(pos).TrimStart();

            AddChild(new HamlNodeTextContainer(SourceFileLineNum, contentLine));
        }

        public string TagName { get; private set; } = string.Empty;

        public bool IsSelfClosing { get; private set; }

        public WhitespaceRemoval WhitespaceRemoval { get; private set; } = WhitespaceRemoval.None;

        public string Namespace { get; private set; } = string.Empty;

        private string GetTagName(string content, ref int pos)
        {
            var result = GetHtmlToken(content, ref pos);

            return string.IsNullOrEmpty(result) ? "div" : result;
        }

        [Obsolete("Id tags are not used in XAML")]
        private void ParseTagIdNode(string content, ref int pos)
        {
            pos++;
            string tagId = GetHtmlToken(content, ref pos);
            var newTag = new HamlNodeTagId(SourceFileLineNum, tagId);
            AddChild(newTag);
        }

        private void ParseViewPropertyNode(string content, ref int pos)
        {
            pos++;
            //TODO: Don't use GetHtmlToken to determine a valid name syntax for view properties.
            var propertyName = GetHtmlToken(content, ref pos);
            var newTag = new HamlNodeViewProperty(SourceFileLineNum, propertyName);
            AddChild(newTag);
        }
        
        private string GetHtmlToken(string content, ref int pos)
        {
            int startIndex = pos;
            while (pos < content.Length)
            {
                if (HtmlStringHelper.IsHtmlIdentifierChar(content[pos]))
                    pos++;
                else
                    break;
            }
            return content.Substring(startIndex, pos - startIndex);
        }

        public string NamespaceQualifiedTagName {
            get
            {
                return string.IsNullOrEmpty(Namespace)
                    ? TagName
                    : Namespace + ":" + TagName;
            }
        }
    }
}
