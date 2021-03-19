using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaIllico.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PizzaIllico.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavPage : TabbedPage
    {
        public MainNavPage()
        {
            InitializeComponent();
            BindingContext = new MainNavViewModel();

        }

        protected override async void OnAppearing()
        {
            if (BindingContext is MainNavViewModel bc)
            {            
                bc._mainVue = tabbedMain;
                bc._profileTab = profilePage;
                await bc.OnResume();
            }
            
        }
    }
}