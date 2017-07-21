using Foundation;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using UIKit;

namespace NHaml.Samples.Xamarin.Forms.iOS
{
	[Register("AppDelegate")]
	public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            var container = new Container();

		    container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            Xamarin.Forms.Forms.Init();

		    RegisterTypes(container);

		    using (AsyncScopedLifestyle.BeginScope(container))
		    {
		        LoadApplication(new App(container));

		        return base.FinishedLaunching(app, options);
            }
		}

	    private static void RegisterTypes(Container container)
	    {
	        // Register platform-specific types here

            container.Register<AssetCollection, IosAssetCollection>(Lifestyle.Scoped);
	    }
	}
}
