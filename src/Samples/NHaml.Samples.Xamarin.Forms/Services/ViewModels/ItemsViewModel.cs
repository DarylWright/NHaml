using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NHaml.Samples.Xamarin.Forms.Helpers;
using NHaml.Samples.Xamarin.Forms.Models;
using NHaml.Samples.Xamarin.Forms.Services;
using NHaml.Samples.Xamarin.Forms.Views;
using Xamarin.Forms;

namespace NHaml.Samples.Xamarin.Forms.ViewModels
{
	public class ItemsViewModel : BaseViewModel, IItemsViewModel
	{
	    private readonly IDataStore<Item> _dataStore;
	    public ObservableRangeCollection<Item> Items { get; set; }
		public Command LoadItemsCommand { get; set; }

		public ItemsViewModel(IDataStore<Item> dataStore)
		{
		    _dataStore = dataStore;
		    Title = "Browse";
			Items = new ObservableRangeCollection<Item>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

			MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
			{
				var _item = item;
				Items.Add(_item);
				await _dataStore.AddItemAsync(_item);
			});
		}

	    private async Task ExecuteLoadItemsCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Items.Clear();
				var items = await _dataStore.GetItemsAsync(true);
				Items.ReplaceRange(items);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				MessagingCenter.Send(new MessagingCenterAlert
				{
					Title = "Error",
					Message = "Unable to load items.",
					Cancel = "OK"
				}, "message");
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}