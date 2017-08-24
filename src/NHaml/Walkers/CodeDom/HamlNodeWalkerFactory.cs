using System;
using NHaml.Compilers;
using NHaml.Parser.Rules;
using NHaml.Walkers.Exceptions;

namespace NHaml.Walkers.CodeDom
{
    /// <summary>
    /// Creates implementations of <see cref="HamlNodeWalker"/> class based on the <see cref="Type"/> of the <see cref="Parser.HamlNode"/> provided.
    /// </summary>
    public static class HamlWalkerFactory
    {
        /// <summary>
        /// Gets an implementation of <see cref="HamlNodeWalker"/> based on the <see cref="Type"/> of <paramref name="nodeType"/>.
        /// </summary>
        /// <param name="nodeType">The type of the node whose walker to get.</param>
        /// <param name="sourceFileLineNo">The line number in the Haml source file this node belongs to.</param>
        /// <param name="classBuilder">The Haml template builder.</param>
        /// <param name="options">Any additional options required to walk through the node.</param>
        /// <returns>An implementationof <see cref="HamlNodeWalker"/> appropriate to the type of <see cref="Parser.HamlNode"/>.</returns>
        /// <exception cref="HamlUnknownNodeTypeException">Thrown when the <see cref="Type"/> of <paramref name="nodeType"/> is unrecognized.</exception>
        public static HamlNodeWalker GetNodeWalker(Type nodeType, int sourceFileLineNo, ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
        {
            //TODO: Figure out how to walk a HamlNodeViewProperty, or determine if it needs to be walked
            if (nodeType == typeof(HamlNodeViewProperty) ||
                nodeType == typeof(HamlNodeXmlAttributeCollection))
                return null;

            if (nodeType == typeof(HamlNodeTextContainer))
                return new HamlNodeTextContainerWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeTag))
                return new HamlNodeTagWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeXmlComment))
                return new HamlNodeHtmlCommentWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeHamlComment))
                return new HamlNodeHamlCommentWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeEval))
                return new HamlNodeEvalWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeCode))
                return new HamlNodeCodeWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeTextLiteral))
                return new HamlNodeTextLiteralWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeTextVariable))
                return new HamlNodeTextVariableWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodeDocType))
                return new HamlNodeDocTypeWalker(classBuilder, options);

            if (nodeType == typeof(HamlNodePartial))
                return new HamlPartialWalker(classBuilder, options);
            
            throw new HamlUnknownNodeTypeException(nodeType, sourceFileLineNo);
        }
    }
}
