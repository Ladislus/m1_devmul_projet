using PizzaIllico.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapsResto
    {
        public MapsResto()
        {
            // Initialisation de la page qui contient la map avec les restos en pin
            InitializeComponent(); 
            // Liaison avec le Context qui est le MapsViewModel
            BindingContext = new MapsViewModel();
        }
    }
}