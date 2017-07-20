namespace NHaml.Parser.Rules
{
    public class HamlNodeViewProperty : HamlNode
    {
        public HamlNodeViewProperty(int sourceFileLineNo, string className)
            : base(sourceFileLineNo, className)
        { }

        protected override bool IsContentGeneratingTag => true;
    }
}