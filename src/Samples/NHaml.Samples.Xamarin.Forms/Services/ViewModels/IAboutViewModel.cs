using System.Windows.Input;

namespace NHaml.Samples.Xamarin.Forms.ViewModels
{
    public interface IAboutViewModel
    {
        ICommand OpenWebCommand { get; }
    }
}