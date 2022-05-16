using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Extensions.Configuration;

namespace MAUIInsights;

public partial class MainPage : ContentPage
{
    private readonly Settings _settings;
    private readonly IConfiguration _configuration;

    public MainPage(IConfiguration config)
    {
		InitializeComponent();

        _configuration = config;
        _settings = _configuration.GetRequiredSection("Settings").Get<Settings>();
    }

    private TelemetryClient GetTelemetryClient()
    {
        TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
        config.ConnectionString = _settings.AppInsights;
        QuickPulseTelemetryProcessor quickPulseProcessor = null;
        config.DefaultTelemetrySink.TelemetryProcessorChainBuilder
            .Use((next) =>
            {
                quickPulseProcessor = new QuickPulseTelemetryProcessor(next);
                return quickPulseProcessor;
            })
            .Build();

        var quickPulseModule = new QuickPulseTelemetryModule
        {
            AuthenticationApiKey =_settings.QuickPulse 
        };
        quickPulseModule.Initialize(config);
        quickPulseModule.RegisterTelemetryProcessor(quickPulseProcessor);
        TelemetryClient client = new(config);
        return client;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}

