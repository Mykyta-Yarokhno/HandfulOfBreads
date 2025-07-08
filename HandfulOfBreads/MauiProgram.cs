using CommunityToolkit.Maui;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.ViewModels;
using HandfulOfBreads.Views;
using HandfulOfBreads.Views.Popups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace HandfulOfBreads
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
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

            builder.Services.AddSingleton<GridLoadingService>();

            builder.Services.AddSingleton<ImagesLoadingService>();

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<StartPage>();

            builder.Services.AddTransient<NewDesignStartViewModel>();
            builder.Services.AddTransient<NewDesignStartPage>();

            builder.Services.AddTransient<ImageToGridViewModel>();
            builder.Services.AddTransient<ImageToGridPage>();

            builder.Services.AddTransient<NewPatternPopup>();
            builder.Services.AddTransient<ColorPickerPopup>();
            builder.Services.AddTransient<ChoosePalettePopup>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
                options.UseSqlite($"Filename={dbPath}");
            });

            builder.Services.AddTransient<CsvImportService>();

            var app = builder.Build();
            App.Services = app.Services;

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated(); 

                var importer = scope.ServiceProvider.GetRequiredService<CsvImportService>();
                importer.ImportAllPalettesAsync().Wait();

                AppLogger.Info("Database active");
            }

            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("AppLogger");
            AppLogger.Initialize(logger);


            return app;
        }
    }
}
