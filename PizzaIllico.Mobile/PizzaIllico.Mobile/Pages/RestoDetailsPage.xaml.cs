using PizzaIllico.Mobile.ViewModels;
using Storm.Mvvm.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestoDetailsPage : BaseContentPage
    {
        public RestoDetailsPage()
        {
            BindingContext = new RestoDetailsViewModel();
            InitializeComponent();
        }
    }
}