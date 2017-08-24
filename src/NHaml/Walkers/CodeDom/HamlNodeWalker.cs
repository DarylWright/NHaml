using System;
using System.Linq;
using NHaml.Compilers;
using NHaml.Crosscutting;
using NHaml.Parser;
using NHaml.Walkers.Exceptions;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Provides a basic specification for walking through a hierarchy of <see cref="HamlNode"/>s and rendering output via a
    /// Haml template class builder.
    /// </summary>
    public abstract class HamlNodeWalker
    {
        /// <summary>
        /// The Haml template class builder that gets written
        /// </summary>
        internal ITemplateClassBuilder ClassBuilder { get; }
        internal HamlHtmlOptions Options { get; }

        /// <summary>
        /// Initializes the <see cref="HamlNodeWalker"/>.
        /// </summary>
        /// <param name="classBuilder">The Haml template class builder to render to.</param>
        /// <param name="options">HTML options required for rendering the <see cref="HamlNode"/> to be walked.</param>
        protected HamlNodeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
        {
            Invariant.ArgumentNotNull(options, "options");
            Invariant.ArgumentNotNull(classBuilder, "classBuilder");

            ClassBuilder = classBuilder;
            Options = options;
        }
        
        /// <summary>
        /// Provides default behaviour for walking through a <see cref="HamlNode"/>, which is simply walking through its children.
        /// </summary>
        /// <remarks>
        /// This method is usually overriden in subclasses and won't respect the default behaviour of the superclass.
        /// </remarks>
        /// <param name="node">The <see cref="HamlNode"/> to walk through.</param>
        /// <exception cref="HamlNodeWalkerException">Thrown when there is a problem walking the children nodes of <paramref name="node"/>.</exception>
        public virtual void Walk(HamlNode node)
        {
            foreach (var child in node.Children)
            {
                try
                {
                    var nodeWalker = HamlWalkerFactory.GetNodeWalker(child.GetType(), child.SourceFileLineNum, ClassBuilder, Options);

                    nodeWalker?.Walk(child);
                }
                catch (Exception e)
                {
                    throw new HamlNodeWalkerException(child.GetType().Name,
                        child.SourceFileLineNum, e);
                }
            }
        }

        /// <summary>
        /// Guard method that ensures that the <paramref name="node"/> contains no children.
        /// </summary>
        /// <param name="node">The <see cref="HamlNode"/> to validate.</param>
        /// <exception cref="HamlInvalidChildNodeException">Thrown when the <paramref name="node"/> has any children.</exception>
        internal void ValidateThereAreNoChildren(HamlNode node)
        {
            if (node.Children.Any())
                throw new HamlInvalidChildNodeException(node.GetType(), node.Children.First().GetType(),
                    node.SourceFileLineNum);
        }
    }
}
