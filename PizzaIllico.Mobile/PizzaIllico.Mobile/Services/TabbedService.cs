using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface ITabbedService
    {
        public TabbedPage get();
        public void set(TabbedPage tabbedPage);

    }
    public class TabbedService : ITabbedService
    {
        private TabbedPage tabbedPage;
        public TabbedPage get()
        {
            return this.tabbedPage;
        }

        public void set(TabbedPage tabbedPage)
        {
            this.tabbedPage = tabbedPage;
        }
    }
}