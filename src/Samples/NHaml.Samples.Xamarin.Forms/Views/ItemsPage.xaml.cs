using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHaml.Samples.Xamarin.Forms.Helpers;
using NHaml.Samples.Xamarin.Forms.Models;
using NHaml.Samples.Xamarin.Forms.ViewModels;
using Xamarin.Forms;

namespace NHaml.Samples.Xamarin.Forms.Views
{
	public partial class ItemsPage : ContentPage, IItemsPage
    {
        private readonly IItemDetailPage _itemDetailPage;
        private readonly INewItemPage _newItemPage;
        private readonly IItemsViewModel _viewModel;

	    public ItemsPage(IItemsViewModel viewModel,
                         IItemDetailPage itemDetailPage,
                         INewItemPage newItemPage)
	    {
            InitializeComponent();
            

            _itemDetailPage = itemDetailPage;
	        _newItemPage = newItemPage;
	        BindingContext = _viewModel = viewModel;
	    }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var item = args.SelectedItem as Item;
			if (item == null)
				return;

		    _itemDetailPage.Item = item;
            
		    await Navigation.PushAsync(_itemDetailPage as Page);

            // Manually deselect item
            ItemsListView.SelectedItem = default(object);
		}

	    public async void AddItem_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(_newItemPage as Page);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (_viewModel.Items.Count == 0)
				_viewModel.LoadItemsCommand.Execute(default(object));
		}
	}
}
