using NHaml.Samples.Xamarin.Forms.ViewModels;
using Xamarin.Forms;

namespace NHaml.Samples.Xamarin.Forms.Views
{
	public partial class AboutPage : ContentPage, IAboutPage
	{
	    public AboutPage(IAboutViewModel viewModel)
	    {
	        InitializeComponent();

            BindingContext = viewModel;
	    }
	}
}
