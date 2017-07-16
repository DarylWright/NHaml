using System.Web.NHaml.IO;

namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeFilter : HamlNode
    {
        public HamlNodeFilter(HamlLine line) : base(line)
        {
            throw new NotImplementedException();
        }

        protected override bool IsContentGeneratingTag => true;
    }
}