namespace System.Web.NHaml.Parser
{
    public enum HamlRuleEnum
    {
        Unknown = 0,
        PlainText,
        Tag,
        DocType,
        XmlComment,
        HamlComment,
        Evaluation,
        Code,
        Partial,
        Document,
        
        Filter,
        ViewProperty
    }
}
