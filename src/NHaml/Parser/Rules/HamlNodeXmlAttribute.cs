using NHaml.Crosscutting;
using NHaml.Parser.Exceptions;

namespace NHaml.Parser.Rules
{
    public class HamlNodeXmlAttribute : HamlNode
    {
        public HamlNodeXmlAttribute(int sourceFileLineNo, string nameValuePair)
            : base(sourceFileLineNo, nameValuePair)
        {
            var index = 0;

            ParseName(ref index);
            ParseValue(index);
        }

        private void ParseValue(int index)
        {
            if (index >= Content.Length) return;

            var value = Content.Substring(index + 1);

            AddChild(new HamlNodeTextContainer(SourceFileLineNum, GetValue(value)));
        }

        private void ParseName(ref int index)
        {
            var result = HtmlStringHelper.GetNextTagAttributeToken(Content, ref index, new[] { '=', '\0' });

            if (string.IsNullOrEmpty(result))
                throw new HamlMalformedTagException("Malformed HTML attribute \"" + Content + "\"", SourceFileLineNum);

            Name = result.TrimEnd('=');
        }

        private string GetValue(string value)
        {
            if (IsQuoted(value))
                return RemoveQuotes(value);
            else if (IsVariable(value))
                return value;
            else
                return "#{" + value + "}";
        }

        private bool IsVariable(string value)
        {
            return value.StartsWith("#{") && value.EndsWith("}");
        }

        private bool IsQuoted(string input)
        {
          return ((input[0] == '\'' && input[input.Length - 1] == '\'')
                || (input[0] == '"' && input[input.Length - 1] == '"'));
        }

        private string RemoveQuotes(string input)
        {
            if (input.Length < 2 || IsQuoted(input) == false)
                return input;

            QuoteChar = input[0];
            return input.Substring(1, input.Length - 2);
        }

        protected override bool IsContentGeneratingTag => true;

        public string Name { get; private set; } = string.Empty;

        public char QuoteChar { get; private set; } = '\'';
    }
}
