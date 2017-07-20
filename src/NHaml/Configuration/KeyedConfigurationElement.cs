using System.Configuration;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Configuration
{
    //TODO: Determine if these permissions are required in a mobile context
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    //[AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public abstract class KeyedConfigurationElement : ConfigurationElement
    {
        public abstract string Key { get; }
    }
}