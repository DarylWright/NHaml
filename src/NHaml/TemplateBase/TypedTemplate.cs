using System.IO;

namespace NHaml.TemplateBase
{
    public class TypedTemplate<T> : Template
    {
        // ReSharper disable UnusedMember.Global
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public T Model { get; set; }

        public void Render(TextWriter writer, T model)
        {
            Model = model;
            base.Render(writer);
        }

        public void Render(TextWriter writer, T model, XmlVersion xmlVersion)
        {
            Model = model;
            base.Render(writer, xmlVersion);
        }

        // ReSharper restore VirtualMemberNeverOverriden.Global
        // ReSharper restore UnusedMember.Global
        // ReSharper restore MemberCanBePrivate.Global
    }
}
