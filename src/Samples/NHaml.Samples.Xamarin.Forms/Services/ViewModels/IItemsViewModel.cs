using NHaml.Samples.Xamarin.Forms.Helpers;
using NHaml.Samples.Xamarin.Forms.Models;
using Xamarin.Forms;

namespace NHaml.Samples.Xamarin.Forms.ViewModels
{
    public interface IItemsViewModel
    {
        ObservableRangeCollection<Item> Items { get; set; }
        Command LoadItemsCommand { get; set; }
    }
}