using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Resources.Localization;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.Views.Popups;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace HandfulOfBreads.ViewModels
{
    public class StartPageViewModel : BaseViewModel
    {
        public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

        private readonly IPopupService _popupService;
        public ICommand AddNewCommand { get; set; }
        public ICommand LanguageSwitchCommand { get; }

        public StartPageViewModel(IPopupService popupService)
        {
            _popupService = popupService;

            AddNewCommand = new Command(async () => await ChooseNewAsync());
            LanguageSwitchCommand = new Command(async () => await OnLanguageSwitch());

            AppLogger.Info("StartPageViewModel initialization");
        }

        private async Task ChooseNewAsync()
        {
            
            var popup = App.Services.GetRequiredService<NewPatternPopup>();

            var result = await _popupService.ShowPopupAsync<string>(popup);

            if (result == "ConvertPhoto")
            {
                var images = await ImagesLoadingService.GetRecentImagesAsync();

                await _popupService.ShowPopupAsync(new ChoosePhotoPopup(images));
            }

            AppLogger.Info("ChooseNewAsync on StartPage");
        }

        private async Task OnLanguageSwitch()
        {
            var switchToCulture = AppResources.Culture.TwoLetterISOLanguageName
               .Equals("uk", StringComparison.InvariantCultureIgnoreCase) ?
               new CultureInfo("en-US") : new CultureInfo("uk-UA");

            LocalizationResourceManager.Instance.SetCulture(switchToCulture);

            AppLogger.Info("OnLanguageSwitch on StartPage");
        }
    }
}
