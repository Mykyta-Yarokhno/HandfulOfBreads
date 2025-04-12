using CommunityToolkit.Maui;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.ViewModels;
using HandfulOfBreads.Views;
using Microsoft.Extensions.Logging;

namespace HandfulOfBreads
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IPopupService, Services.PopupService>();
            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<StartPage>();

            return builder.Build();
        }
    }
}
