using HandfulOfBreads.Data;
using HandfulOfBreads.Services;
using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}