using System;
using System.Collections;
using System.Collections.Generic;
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
            ParseAttributeReference(nodeLine.Content, ref pos);
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

        private void ParseAttributeReference(string content, ref int pos)
        {
            if (pos >= content.Length) return;

            if (content[pos] != '[') return;

            var startPos = pos;

            var attrRefEndChar = ']';

            for (; pos < content.Length; pos++)
            {
                if (content[pos] == attrRefEndChar)
                {
                    var attributeRef = content.Substring(startPos, pos - startPos + 1);

                    AddChild(new HamlNodeXmlAttributeReferenceCollection(SourceFileLineNum, attributeRef));

                    return;
                }

                if (!HtmlStringHelper.IsXmlIdentifierChar(content[pos]) && content[pos] != attrRefEndChar)
                    throw new HamlMalformedTagException($"Malformed attribute reference \"{content}\".", SourceFileLineNum);
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
        
        private string GetHtmlToken(string content, ref int pos)
        {
            int startIndex = pos;
            while (pos < content.Length)
            {
                if (HtmlStringHelper.IsXmlIdentifierChar(content[pos]))
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

    public class HamlNodeXmlAttributeReferenceCollection : HamlNode
    {
        public HamlNodeXmlAttributeReferenceCollection(int sourceFileLineNum, string attributeRefs)
            : base(sourceFileLineNum, attributeRefs)
        {
            if (Content[0] != '[')
                throw new HamlMalformedTagException("AttributeReference tag must start with an opening bracket.", SourceFileLineNum);
        }

        public void ResolveAttributes(IDictionary<string, object> attributes)
        {
            
        }

        protected override bool IsContentGeneratingTag => false;

        public bool IsResolved { get; private set; } = false;
    }
}
