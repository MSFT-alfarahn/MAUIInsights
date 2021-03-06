global using Microsoft.ApplicationInsights;
global using Microsoft.ApplicationInsights.Extensibility;
global using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
global using Microsoft.Extensions.Configuration;

using System.Reflection;

namespace MAUIInsights;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        var a = typeof(App).GetTypeInfo().Assembly;
        var stream = a.GetManifestResourceStream($"{a.GetName().Name}.appsettings.json");

        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();


        builder.Configuration.AddConfiguration(config);
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
	}
}


public class Settings
{
    public string AppInsights { get; set; }
    public string QuickPulse { get; set; }
}
