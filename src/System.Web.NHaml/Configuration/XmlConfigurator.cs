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
    public static class XmlConfigurator
    {
        public static TemplateEngine GetTemplateEngine()
        {
            string configFile = HttpContext.Current == null
                                    ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath
                                    : WebConfigurationManager.OpenWebConfiguration("~").FilePath;

            return GetTemplateEngine(configFile);
        }

        public static TemplateEngine GetTemplateEngine(ITemplateContentProvider templateContentProvider,
            IEnumerable<string> defaultImports, IEnumerable<string> defaultReferences)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration();
            return GetTemplateEngine(templateContentProvider, nhamlConfiguration, defaultImports, defaultReferences);
        }
        
        public static TemplateEngine GetTemplateEngine(string configFile)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration(configFile);
            return GetTemplateEngine(new FileTemplateContentProvider(), nhamlConfiguration, new List<string>(), new List<string>());
        }

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
