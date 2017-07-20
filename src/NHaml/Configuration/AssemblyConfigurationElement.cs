using System.Configuration;

namespace NHaml.Configuration
{
    public class AssemblyConfigurationElement : KeyedConfigurationElement
    {
        public override string Key => Name;

        private const string AssemblyElement = "assembly";
        [ConfigurationProperty(AssemblyElement, IsRequired = true, IsKey = true)]
        public string Name => (string)this[AssemblyElement];
    }
}