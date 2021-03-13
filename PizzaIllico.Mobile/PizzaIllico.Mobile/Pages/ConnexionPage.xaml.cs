using PizzaIllico.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnexionPage : ContentPage
    {
        public ConnexionPage()
        {
            InitializeComponent();
            BindingContext = new ConnexionViewModel();
           
        }

    }
}