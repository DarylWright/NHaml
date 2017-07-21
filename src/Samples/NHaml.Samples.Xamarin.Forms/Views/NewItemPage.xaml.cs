using System;
using NHaml.Samples.Xamarin.Forms.Models;
using Xamarin.Forms;

namespace NHaml.Samples.Xamarin.Forms.Views
{
	public partial class NewItemPage : ContentPage, INewItemPage
	{
		public Item Item { get; set; }

		public NewItemPage()
		{
			InitializeComponent();

			Item = new Item
			{
				Text = "Item name",
				Description = "This is a nice description"
			};

			BindingContext = this;
		}

		async void Save_Clicked(object sender, EventArgs e)
		{
			MessagingCenter.Send(this, "AddItem", Item);
			await Navigation.PopToRootAsync();
		}
	}
}