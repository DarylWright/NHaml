using NHaml.Compilers;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for HamlNodePartial nodes.
    /// </summary>
    public class HamlPartialWalker : HamlNodeWalker
    {
        public HamlPartialWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }
    }
}
