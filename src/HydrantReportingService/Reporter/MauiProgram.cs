using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Reporter
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


            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("Reporter.appsettings.json");

            var config = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();


            builder.Configuration.AddConfiguration(config);

            GlobalSettings.Settings = config.GetRequiredSection("Settings").Get<Settings>();

            return builder.Build();
        }
    }
}