using PizzaIllico.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapsResto
    {
        public MapsResto()
        {
            InitializeComponent();
            BindingContext = new MapsViewModel();
        }
    }
}