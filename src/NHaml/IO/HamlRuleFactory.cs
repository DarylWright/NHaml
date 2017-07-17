using NHaml.Parser;

namespace NHaml.IO
{
    public static class HamlRuleFactory
    {
        public static HamlRuleEnum ParseHamlRule(ref string content)
        {
            if (content == "") return HamlRuleEnum.PlainText;

            if (content.StartsWith("!!!"))
            {
                content = content.Substring(3);
                return HamlRuleEnum.DocType;
            }
            if (content.StartsWith("#{"))
            {
                content = content.Substring(0);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("\\\\"))
            {
                content = content.Substring(0);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("\\#"))
            {
                content = content.Substring(0);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("%"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Tag;
            }
            if (content.StartsWith("."))
            {
                return HamlRuleEnum.ViewProperty;
            }
            if (content.StartsWith("#"))
            {
                return HamlRuleEnum.HamlComment;
            }
            if (content.StartsWith("/"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.XmlComment;
            }
            if (content.StartsWith("="))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Evaluation;
            }
            if (content.StartsWith("-"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Code;
            }
            if (content.StartsWith(@"\"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("_"))
            {
                content = content.Substring(1).Trim();
                return HamlRuleEnum.Partial;
            }
            return HamlRuleEnum.PlainText;
        }
    }
}
