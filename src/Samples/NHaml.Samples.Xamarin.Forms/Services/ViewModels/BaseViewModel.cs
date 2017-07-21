using NHaml.Samples.Xamarin.Forms.Models;
using NHaml.Samples.Xamarin.Forms.Services;
using NHaml.Samples.Xamarin.Forms.Helpers;
using Xamarin.Forms;

namespace NHaml.Samples.Xamarin.Forms.ViewModels
{
	public class BaseViewModel : ObservableObject
	{
	    private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
		    set => SetProperty(ref _isBusy, value);
		}

		/// <summary>
		/// Private backing field to hold the title
		/// </summary>
		private string _title = string.Empty;
		/// <summary>
		/// Public property to set and get the title of the item
		/// </summary>
		public string Title
		{
			get => _title;
		    set => SetProperty(ref _title, value);
		}
	}
}

