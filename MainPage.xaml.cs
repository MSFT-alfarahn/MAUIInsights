using Microsoft.Extensions.Configuration;

namespace MAUIInsights;

public partial class MainPage : ContentPage
{
	int count = 0;
    IConfiguration configuration;
    public MainPage(IConfiguration config)
    {
		InitializeComponent();

        configuration = config;
    }

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

        var settings = configuration.GetRequiredSection("Settings").Get<Settings>();
    }
}

