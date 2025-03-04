using CommunityToolkit.Maui;
using HandfulOfBreads.Services;
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

            builder.Services.AddSingleton<StartPageViewModel>();
            builder.Services.AddSingleton<StartPage>();
            builder.Services.AddSingleton<ImageLoadingService>();

            builder.Services.AddSingleton<ConvertPhotoPage>();
#endif

            return builder.Build();
        }
    }
}
