using NHaml.IO;

namespace NHaml.Parser.Rules
{
    public class HamlNodeViewProperty : HamlNode
    {
        public HamlNodeViewProperty(HamlLine nodeLine)
            : base(nodeLine)
        {
            var tag = new HamlNodeTag(nodeLine);

            tag.
            AddChild();
        }

        protected override bool IsContentGeneratingTag => false;
    }
}