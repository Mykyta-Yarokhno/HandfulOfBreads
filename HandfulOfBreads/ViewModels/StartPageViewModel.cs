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
        }

        private async Task ChooseNewAsync()
        {
            var popup = new NewPatternPopup();
            await _popupService.ShowPopupAsync(popup);
        }

        private async Task OnLanguageSwitch()
        {
            var switchToCulture = AppResources.Culture.TwoLetterISOLanguageName
               .Equals("uk", StringComparison.InvariantCultureIgnoreCase) ?
               new CultureInfo("en-US") : new CultureInfo("uk-UA");

            LocalizationResourceManager.Instance.SetCulture(switchToCulture);
        }
    }
}
