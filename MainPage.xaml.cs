using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Extensions.Configuration;

namespace MAUIInsights;

public partial class MainPage : ContentPage
{
    private readonly Settings _settings;
    private readonly TelemetryClient _telemetry;
    private readonly IConfiguration _configuration;
    private bool _runThread;

    public MainPage(IConfiguration config)
    {
		InitializeComponent();

        _configuration = config;
        _settings = _configuration.GetRequiredSection("Settings").Get<Settings>();
        _telemetry = GetTelemetryClient();
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

    private async void Button_Clicked(object sender, EventArgs ev)
    {
        _runThread = true;

        List<Thread> threads = new List<Thread>();
        for (int i = 0; i < 1000000; i++)
        {
            threads.Add(new Thread(new ThreadStart(KillCore)));
        }

        _runThread = false;

        // Track Requests
        // This sample runs indefinitely. Replace with actual application logic.
        for (int i = 0; i < 100; i++)
        {
            // Send dependency and request telemetry.
            // These will be shown in Live Metrics stream.
            // CPU/Memory Performance counter is also shown
            // automatically without any additional steps.
            
            _telemetry.TrackDependency(
                "Bad Dependency", 
                "target", 
                "data",
                DateTimeOffset.Now, 
                TimeSpan.FromMilliseconds(50 * i), 
                true);

            _telemetry.TrackDependency(
                "Good Dependency",
                "target",
                "data",
                DateTimeOffset.Now,
                TimeSpan.FromMilliseconds(3 * i),
                true);

            _telemetry.TrackRequest("Bad Request", 
                DateTimeOffset.Now,
                TimeSpan.FromMilliseconds(50 * i), 
                "200", 
                true);

            _telemetry.TrackRequest("Bad Request",
                DateTimeOffset.Now,
                TimeSpan.FromMilliseconds(3 * i),
                "200",
                true);

            await Task.Delay(100);
        }

        try
        {
            int.Parse("invalid");
        }
        catch (Exception e)
        {
            _telemetry.TrackException(e);
        }

        try
        {
            string dog = null;
            var value = dog.ToString();
        }
        catch (Exception e)
        {
            _telemetry.TrackException(e);
        }
    }
    private void KillCore()
    {
        Random rand = new Random();
        long num = 0;
        while (_runThread)
        {
            num += rand.Next(100, 1000);
            if (num > 1000000) { num = 0; }
        }
    }
}

