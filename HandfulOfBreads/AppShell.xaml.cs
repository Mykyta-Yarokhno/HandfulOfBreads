using HandfulOfBreads.Views;
using HandfulOfBreads.Views.Popups;

namespace HandfulOfBreads
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(StartPage), typeof(StartPage));
            Routing.RegisterRoute(nameof(NewPatternPopup), typeof(NewPatternPopup));
            Routing.RegisterRoute(nameof(NewDesignStartPage), typeof(NewDesignStartPage));
            Routing.RegisterRoute(nameof(ConvertPhotoPage), typeof(ConvertPhotoPage));
        }
    }
}
