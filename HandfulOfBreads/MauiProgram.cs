using CommunityToolkit.Maui;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.ViewModels;
using HandfulOfBreads.Views;
using HandfulOfBreads.Views.Popups;
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

            builder.Services.AddTransient<NewDesignStartViewModel>();
            builder.Services.AddTransient<NewDesignStartPage>();

            builder.Services.AddTransient<ImageToGridViewModel>();
            builder.Services.AddTransient<ImageToGridPage>();

            builder.Services.AddTransient<NewPatternPopup>();

            builder.Services.AddSingleton<GridLoadingService>();
            builder.Services.AddSingleton<ImagesLoadingService>();

            var app = builder.Build();
            App.Services = app.Services;

            return app;
        }
    }
}
