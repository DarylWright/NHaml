using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using NHaml.Compilers;
using NHaml.IO;
using NHaml.Parser;
using NHaml.TemplateResolution;
using NHaml.Walkers.CodeDom;

namespace NHaml.Configuration
{
    /// <summary>
    /// The XmlConfigurator class is where the Haml engine is built and is also the composition root of the library.
    /// </summary>
    /// <remarks>
    /// It is recommended to encapsulate the call to this class in an adapter from the calling application.
    /// </remarks>
    public static class XmlConfigurator
    {
        /// <summary>
        /// Gets the Haml template engine using a configured configuration file location.
        /// </summary>
        /// <returns>The Haml template engine.</returns>
        public static TemplateEngine GetTemplateEngine()
        {
            //TODO: Get configuration file within mobile environment. Do not use ConfigurationManager nor WebConfigurationManager.
            var configFile = HttpContext.Current == null
                                    ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath
                                    : WebConfigurationManager.OpenWebConfiguration("~").FilePath;

            return GetTemplateEngine(configFile);
        }

        /// <summary>
        /// Gets the Haml template engine by explicitly providing the ITemplageContentProvider, a list of namespaces,
        /// and a list of assembly references.
        /// </summary>
        /// <remarks>
        /// TODO: Explain how the namespaces and assembly references work and what they are for.
        /// </remarks>
        /// <param name="templateContentProvider"></param>
        /// <param name="defaultImports"></param>
        /// <param name="defaultReferences"></param>
        /// <returns>The Haml template engine.</returns>
        public static TemplateEngine GetTemplateEngine(ITemplateContentProvider templateContentProvider,
            IEnumerable<string> defaultImports, IEnumerable<string> defaultReferences)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration();
            return GetTemplateEngine(templateContentProvider, nhamlConfiguration, defaultImports, defaultReferences);
        }
        
        /// <summary>
        /// Gets the Haml template engine with a single config file.
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns>The Haml template engine.</returns>
        public static TemplateEngine GetTemplateEngine(string configFile)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration(configFile);
            return GetTemplateEngine(new FileTemplateContentProvider(), nhamlConfiguration, new List<string>(), new List<string>());
        }

        /// <summary>
        /// Builds and returns the Haml template engine.
        /// </summary>
        /// <param name="templateContentProvider"></param>
        /// <param name="nhamlConfiguration"></param>
        /// <param name="imports"></param>
        /// <param name="referencedAssemblies"></param>
        /// <returns>The Haml template engine.</returns>
        private static TemplateEngine GetTemplateEngine(ITemplateContentProvider templateContentProvider, NHamlConfigurationSection nhamlConfiguration, IEnumerable<string> imports, IEnumerable<string> referencedAssemblies)
        {
            var templateCache = new SimpleTemplateCache();

            var templateFactoryFactory = new TemplateFactoryFactory(
                templateContentProvider,
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CodeDomClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                nhamlConfiguration.ImportsList.Concat(imports),
                nhamlConfiguration.ReferencedAssembliesList.Concat(referencedAssemblies));

            return new TemplateEngine(templateCache, templateFactoryFactory);
        }
    }
}
