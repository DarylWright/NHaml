using System;
using System.Collections.Generic;
using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Haml node walker for <see cref="HamlDocument"/> nodes.
    /// </summary>
    public class HamlDocumentWalker : HamlNodeWalker, IDocumentWalker
    {
        public HamlDocumentWalker(ITemplateClassBuilder classBuilder)
            : base (classBuilder, new HamlHtmlOptions())
        { }

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }

        /// <summary>
        /// Walks through a <see cref="HamlDocument"/> node.
        /// </summary>
        /// <param name="document">The <see cref="HamlDocument"/> node to be rendered to the template class builder.</param>
        /// <param name="className">The name of the template class to build.</param>
        /// <param name="baseType">The base type of the class template.</param>
        /// <param name="imports">The list of namespaces to import.</param>
        public string Walk(HamlDocument document, string className, Type baseType, IEnumerable<string> imports)
        {
            ClassBuilder.Clear();
            base.Walk(document);
            return ClassBuilder.Build(className, baseType, imports);
        }    
    }
}
