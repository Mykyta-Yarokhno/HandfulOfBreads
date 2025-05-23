
using HandfulOfBreads.Data;
using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get;  set; }
        public static MainPageViewModel MainViewModel { get; private set; }
        public App(IServiceProvider serviceProvider, AppDbContext appDbContext)
        {
            InitializeComponent();

            ColorRepository.Initialize(appDbContext);

            Services = serviceProvider;

            MainPage = new AppShell();

            MainViewModel = new MainPageViewModel();

            InitializePalettesAsync();
        }

        private async void InitializePalettesAsync()
        {
            var allPalettes = await ColorRepository.GetAllPalettesAsync();

            ColorPaletteViewCache.InitializeAllPalettes(allPalettes, OnColorTapped);
        }

        private void OnColorTapped(ColorItemViewModel color)
        {
            MainViewModel.SelectColorCommand.Execute(color);
        }
    }
}
