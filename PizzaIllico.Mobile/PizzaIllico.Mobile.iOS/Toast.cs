using Foundation;
using PizzaIllico.Mobile.Controls;
using PizzaIllico.Mobile.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIOS))]
namespace PizzaIllico.Mobile.iOS
{
    public class MessageIOS : IToast
    {
        const double LongDelay = 3.5;

        NSTimer _alertDelay;
        UIAlertController _alert;

        public void LongAlert(string message)
        {
            ShowAlert(message, LongDelay);
        }

        private void ShowAlert(string message, double seconds)
        {
            _alertDelay = NSTimer.CreateScheduledTimer(seconds, obj =>
            {
                DismissMessage();
            });
            _alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            if (UIApplication.SharedApplication.KeyWindow.RootViewController != null)
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(_alert, true, null);
        }

        private void DismissMessage()
        {
            _alert?.DismissViewController(true, null);
            _alertDelay?.Dispose();
        }
    }
}