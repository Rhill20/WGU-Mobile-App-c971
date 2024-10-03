using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace College_Tracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // DB
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "College_Tracker.db3");

            // Register services
            builder.Services.AddSingleton(dbPath);
            builder.Services.AddSingleton<App>();

            // Use Maui and LocalNotification plugin
            builder
                .UseMauiApp<App>()
                .UseLocalNotification() // Only need this once
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add logging in debug mode
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}