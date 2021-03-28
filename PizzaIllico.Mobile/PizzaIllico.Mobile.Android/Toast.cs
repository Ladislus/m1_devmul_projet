using Android.App;
using Android.Widget;
using PizzaIllico.Mobile.Android;
using PizzaIllico.Mobile.Controls;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace PizzaIllico.Mobile.Android
{
    public class MessageAndroid : IToast
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long)?.Show();
        }
    }
}