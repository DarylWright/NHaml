using System.Web.NHaml.IO;

namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeViewProperty : HamlNode
    {
        public HamlNodeViewProperty(HamlLine line) : base(line)
        {
            throw new NotImplementedException();
        }

        protected override bool IsContentGeneratingTag => true;
    }
}