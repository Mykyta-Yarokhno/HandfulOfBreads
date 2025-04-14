namespace HandfulOfBreads
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get;  set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Services = serviceProvider;

            MainPage = new AppShell();
        }
    }
}
