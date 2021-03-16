using PizzaIllico.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestoDetailsPage : ContentPage
    {
        public RestoDetailsPage()
        {
            BindingContext = new RestoDetailsViewModel();
            InitializeComponent();
        }
    }
}