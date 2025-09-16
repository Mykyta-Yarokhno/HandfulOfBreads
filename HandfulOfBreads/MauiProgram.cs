// MauiProgram.cs
using CommunityToolkit.Maui;
using HandfulOfBreads.Data;
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

            // Registration of services
            builder.Services.AddSingleton<IPopupService, Services.PopupService>();
            builder.Services.AddSingleton<GridLoadingService>();
            builder.Services.AddSingleton<ImagesLoadingService>();
            builder.Services.AddSingleton<ColorPaletteSvgCache>();
            builder.Services.AddSingleton<GridSavingService>();

            // DbContext and its dependencies must be scoped
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
                options.UseSqlite($"Filename={dbPath}");
            });

            // Repositories that depend on DbContext must be scoped
            builder.Services.AddScoped<ColorRepository>();
            builder.Services.AddScoped<CsvImportService>();

            // Transient ViewModels and Pages
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<NewDesignStartViewModel>();
            builder.Services.AddTransient<NewDesignStartPage>();
            builder.Services.AddTransient<ImageToGridViewModel>();
            builder.Services.AddTransient<ImageToGridPage>();
            builder.Services.AddTransient<NewPatternPopup>();
            builder.Services.AddTransient<ChoosePalettePopup>();

            var app = builder.Build();

            // Set the static service provider
            App.Services = app.Services;

            // Perform database and cache initialization in a safe scope
            InitializeDatabaseAndCache(app.Services);

            return app;
        }

        private static void InitializeDatabaseAndCache(IServiceProvider services)
        {
            // Create a dedicated scope for database operations
            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var db = serviceProvider.GetRequiredService<AppDbContext>();
                var importer = serviceProvider.GetRequiredService<CsvImportService>();
                var colorRepository = serviceProvider.GetRequiredService<ColorRepository>();
                var paletteCache = serviceProvider.GetRequiredService<ColorPaletteSvgCache>();

                // Perform operations
                db.Database.EnsureCreated();
                importer.ImportAllPalettesAsync().GetAwaiter().GetResult();
                var allPalettes = colorRepository.GetAllPalettesAsync().GetAwaiter().GetResult();
                paletteCache.InitializeAllPalettes(allPalettes);
            }
        }
    }
}