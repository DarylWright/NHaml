using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp;

namespace NHaml.Compilers
{
    /// <summary>
    /// An implementation of <see cref="CodeDomTemplateTypeBuilder"/> that provides additional settings.
    /// </summary>
    public class CSharp2TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CSharp2TemplateTypeBuilder()
            : base(new CSharpCodeProvider())
        {
            //TODO: Determine if the CompilerVersion "v2.0" is outdated.
            ProviderOptions.Add( "CompilerVersion", "v2.0" );
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}

