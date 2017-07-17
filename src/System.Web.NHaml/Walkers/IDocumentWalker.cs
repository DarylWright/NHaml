using System;
using System.Collections.Generic;
using NHaml.Parser;

namespace NHaml.Walkers
{
    public interface IDocumentWalker
    {
        string Walk(HamlDocument hamlDocument, string className, Type baseType, IEnumerable<string> imports);
    }
}
