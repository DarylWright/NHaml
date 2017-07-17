using System;
using NHaml.TemplateResolution;

namespace NHaml
{
    public interface ITemplateFactoryFactory
    {
        TemplateFactory CompileTemplateFactory(string className, ViewSourceCollection viewSourceCollection, Type baseType);
        ITemplateContentProvider TemplateContentProvider { set; }
    }
}
