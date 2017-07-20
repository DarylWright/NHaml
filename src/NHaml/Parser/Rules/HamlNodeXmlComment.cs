namespace NHaml.Parser.Rules
{
    public class HamlNodeXmlComment : HamlNode
    {
        public HamlNodeXmlComment(IO.HamlLine nodeLine)
            : base(nodeLine)
        { }

        protected override bool IsContentGeneratingTag => true;
    }
}
