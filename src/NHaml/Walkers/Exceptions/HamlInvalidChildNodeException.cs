using System;

namespace NHaml.Walkers.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a <see cref="Parser.HamlNode"/> has a child node when it shouldn't.
    /// </summary>
    [Serializable]
    public class HamlInvalidChildNodeException : Exception
    {
        /// <summary>
        /// Constructor for the <see cref="HamlInvalidChildNodeException"/> class.
        /// </summary>
        /// <param name="nodeType">The type of the <see cref="Parser.HamlNode"/> that shouldn't have a child.</param>
        /// <param name="childType">The type of the invalid child.</param>
        /// <param name="lineNo">The line number of the Haml file this exception occurred on.</param>
        public HamlInvalidChildNodeException(Type nodeType, Type childType, int lineNo)
            : base($"Node '{nodeType.FullName}' has invalid child node {childType.FullName} on line {lineNo}", null)
        { }
    }
}
