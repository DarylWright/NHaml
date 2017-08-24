using NHaml.IO;

namespace NHaml.Parser.Rules
{
    public class HamlNodeViewProperty : HamlNode
    {
        public HamlNodeViewProperty(HamlLine nodeLine)
            : base(nodeLine)
        {
            var tag = new HamlNodeTag(nodeLine);

            //TODO: Ensure the tag name represents the format in the Xaml file (e.g. ParentTagName.ThisTagName)
            AddChild(tag);
        }

        protected override bool IsContentGeneratingTag => false;
    }
}