using NHaml.Samples.Xamarin.Forms.Models;

namespace NHaml.Samples.Xamarin.Forms.ViewModels
{
    public interface IItemDetailViewModel
    {
        Item Item { get; set; }
        int Quantity { get; set; }
    }
}