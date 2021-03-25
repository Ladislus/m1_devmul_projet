using PizzaIllico.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartPage
    {
        public CartPage()
        {
            InitializeComponent();
            BindingContext = new CartViewModel();
        }
    }
}