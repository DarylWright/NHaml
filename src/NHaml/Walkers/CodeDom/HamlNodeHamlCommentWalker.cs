using NHaml.Compilers;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for HamlNodeHamlComment nodes.
    /// </summary>
    public class HamlNodeHamlCommentWalker : HamlNodeWalker
    {
        public HamlNodeHamlCommentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        /// <summary>
        /// Walks through a HamlNodeHamlComment node.
        /// </summary>
        /// <param name="node">The HamlNodeHamlComment node to be rendered to the template class builder.</param>
        public override void Walk(Parser.HamlNode node)
        { }
    }
}
