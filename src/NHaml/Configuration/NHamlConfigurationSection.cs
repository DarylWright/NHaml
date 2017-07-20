using System;
using System.Configuration;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NHaml.Configuration
{
	public class NHamlConfigurationSection : ConfigurationSection
	{
		public static NHamlConfigurationSection GetConfiguration()
		{
            return ConfigurationManager.GetSection("NHaml") as NHamlConfigurationSection
                ?? new NHamlConfigurationSection();
        }

        public static NHamlConfigurationSection GetConfiguration(string configFile)
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException("Unable to find configuration file " + configFile, configFile);

            var map = new ConfigurationFileMap(configFile);
            var config = ConfigurationManager.OpenMappedMachineConfiguration(map);
            var result = (NHamlConfigurationSection)config.GetSection("NHaml");
            return result ?? new NHamlConfigurationSection();
        }

        const string ReferencedAssembliesElement = "assemblies";
        [ConfigurationProperty(ReferencedAssembliesElement)]
        public ConfigurationCollection<AssemblyConfigurationElement> ReferencedAssemblies
            => (ConfigurationCollection<AssemblyConfigurationElement>)base[ReferencedAssembliesElement];

	    public IEnumerable<string> ReferencedAssembliesList
        {
            get
            {
                return (ReferencedAssemblies != null)
                    ? ReferencedAssemblies.Select(x => GetAssemblyLocation(x.Name))
                    : new List<string>();
            }
        }

        private string GetAssemblyLocation(string assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName).Location;
            }
            catch (Exception exception)
            {
                var message =
                    $"Could not load Assembly '{assemblyName}'.Did you forget to fully qualify it? For example 'System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'";
                throw new Exception(message, exception);
            }
        }

        const string ImportsElement = "imports";
        [ConfigurationProperty(ImportsElement)]
        private ConfigurationCollection<ImportConfigurationElement> Imports
            => (ConfigurationCollection<ImportConfigurationElement>)base[ImportsElement];

	    public IEnumerable<string> ImportsList
        {
            get
            {
                return Imports?.Select(x => x.Name) ?? new List<string>();
            }
        }

        //const string AutoRecompileAttribute = "AutoRecompile";
        //[ConfigurationProperty(AutoRecompileAttribute)]
        //public virtual bool? AutoRecompile
        //{
        //    get { return this[AutoRecompileAttribute] as bool?; }
        //}

        //const string OutputDebugFilesAttribute = "OutputDebugFiles";
        //[ConfigurationProperty(OutputDebugFilesAttribute)]
        //public virtual bool? OutputDebugFiles
        //{
        //    get { return this[OutputDebugFilesAttribute] as bool?; }
        //}

        //const string NamespacesElement = "Namespaces";
        //[ConfigurationProperty(NamespacesElement)]
        //public virtual NamespacesConfigurationCollection Namespaces
        //{
        //    get { return (NamespacesConfigurationCollection)base[NamespacesElement]; }
        //}
    }
}