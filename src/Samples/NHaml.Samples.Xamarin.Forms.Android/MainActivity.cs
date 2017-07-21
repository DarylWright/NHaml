using Android.App;
using Android.Content.PM;
using Android.OS;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace NHaml.Samples.Xamarin.Forms.Droid
{
    [Activity(Label = "NHaml.Samples.Xamarin.Forms.Android", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterTypes(container);

            using (AsyncScopedLifestyle.BeginScope(container))
            {
                LoadApplication(new App(container));
            }
        }

        private static void RegisterTypes(Container container)
        {
            // Register platform-specific types here
            container.Register<AssetCollection, AndroidAssetCollection>(Lifestyle.Scoped);
        }
    }
}