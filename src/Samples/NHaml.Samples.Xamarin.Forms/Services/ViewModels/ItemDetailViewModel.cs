using NHaml.Samples.Xamarin.Forms.Models;

namespace NHaml.Samples.Xamarin.Forms.ViewModels
{
	public class ItemDetailViewModel : BaseViewModel, IItemDetailViewModel
	{
	    private Item _item;
	    public Item Item
	    {
	        get => _item;
            set => SetItem(value);
	    }

	    private void SetItem(Item item)
	    {
	        SetProperty(ref _item, item);

	        Title = item.Text;
	    }

        private int _quantity = 1;
		public int Quantity
		{
			get => _quantity;
		    set => SetProperty(ref _quantity, value);
		}
	}
}