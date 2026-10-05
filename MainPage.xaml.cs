
using System.Globalization;

namespace WorldTimeClock;

public partial class MainPage : ContentPage
{
    private readonly PeriodicTimer timer;

    private readonly Dictionary<string, string> selectedTimeZones = new();

    private readonly Dictionary<string, string> timeZoneIds = new()
    {
        { "India", "India Standard Time" },
        { "New York", "Eastern Standard Time" },
        { "London", "GMT Standard Time" },
        { "Tokyo", "Tokyo Standard Time" },
        { "Sydney", "AUS Eastern Standard Time" }
    };

    public MainPage()
    {
        InitializeComponent();

        timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        UpdateClocks();

        StartClock();
    }

    private async void StartClock()
    {
        while (await timer.WaitForNextTickAsync())
        {
            MainThread.BeginInvokeOnMainThread(UpdateClocks);
        }
    }

    private void UpdateClocks()
    {
        IndiaTimeLabel.Text = GetTime("India Standard Time");

        NewYorkTimeLabel.Text = GetTime("Eastern Standard Time");

        LondonTimeLabel.Text = GetTime("GMT Standard Time");

        TokyoTimeLabel.Text = GetTime("Tokyo Standard Time");

        SydneyTimeLabel.Text = GetTime("AUS Eastern Standard Time");

        UpdateSelectedCity();
    }

    private void UpdateSelectedCity()
    {
        if (selectedTimeZones.Count == 0)
        {
            SelectedCityLabel.Text = "Selected Time Zones";
            SelectedCityTimeLabel.Text = "Select a city and click Add";
            return;
        }

        string selectedCities = "";

        foreach (var zone in selectedTimeZones)
        {
            string currentTime = GetTime(zone.Value);

            selectedCities += $"{zone.Key}: {currentTime}\n";
        }

        SelectedCityLabel.Text = "Selected Time Zones";
        SelectedCityTimeLabel.Text = selectedCities.TrimEnd();
    }

    private void AddTimeZone_Clicked(object sender, EventArgs e)
    {
        if (TimeZonePicker.SelectedIndex == -1)
            return;

        string selectedCity =
            TimeZonePicker.SelectedItem?.ToString() ?? "";

        if (timeZoneIds.ContainsKey(selectedCity))
        {
            if (!selectedTimeZones.ContainsKey(selectedCity))
            {
                selectedTimeZones.Add(
                    selectedCity,
                    timeZoneIds[selectedCity]);
            }
        }

        UpdateSelectedCity();
    }

    private string GetTime(string timeZoneId)
    {
        TimeZoneInfo timeZone =
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        DateTime time = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            timeZone);

        return time.ToString(
            "hh:mm:ss tt",
            CultureInfo.InvariantCulture);
    }
}