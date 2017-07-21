namespace NHaml.Samples.Xamarin.Forms.iOS
{
    public class IosAssetCollection : AssetCollection
    {
        protected new void BuildAssets()
        {
            Assets.Add(AssetIdentifier.TabFeedPng, "tab_feed.png");
            Assets.Add(AssetIdentifier.TabAboutPng, "tab_about.png");
        }
    }
}