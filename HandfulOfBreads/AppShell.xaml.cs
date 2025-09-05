using HandfulOfBreads.Views;
using HandfulOfBreads.Views.Popups;

namespace HandfulOfBreads
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

            Routing.RegisterRoute(nameof(NewDesignStartPage), typeof(NewDesignStartPage));
        }
    }
}
