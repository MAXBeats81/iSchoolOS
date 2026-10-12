using System.Globalization;
using CommunityToolkit.Maui.Media;
using iSchoolOS.Services;

namespace iSchoolOS;

public partial class MainPage : ContentPage
{
    private readonly CampusDataService dataService = new();
    private readonly VoiceCommandService voiceCommandService = new();
    private readonly BlockchainService blockchainService = new();
    private CancellationTokenSource? cancellationTokenSource;
    private readonly ISpeechToText speechToText = SpeechToText.Default;

    public MainPage()
    {
        InitializeComponent();
        NetworkLabel.Text = Connectivity.Current.NetworkAccess.ToString();
    }

    private async void RefreshData_Clicked(object? sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Loading campus data...";
            var data = await dataService.GetCampusDataAsync();
            StudentsLabel.Text = $"Students Present: {data.StudentsPresent}";
            OccupancyLabel.Text = $"Room Occupancy: {data.RoomOccupancy}%";
            ParkingLabel.Text = $"Available Parking: {data.AvailableParking}";
            TemperatureLabel.Text = $"Temperature: {data.Temperature} °C";
            EvaluateCampus(data);
            NetworkLabel.Text = Connectivity.Current.NetworkAccess.ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void EvaluateCampus(Models.CampusData data)
    {
        if (data.RoomOccupancy >= 90)
        {
            StatusLabel.Text = "WARNING: HIGH ROOM OCCUPANCY";
        }
        else if (data.AvailableParking <= 10)
        {
            StatusLabel.Text = "WARNING: LIMITED PARKING";
        }
        else if (data.Temperature >= 35)
        {
            StatusLabel.Text = "WARNING: HIGH TEMPERATURE";
        }
        else
        {
            StatusLabel.Text = "SYSTEM STATUS: NORMAL";
        }
    }

    private async void SpeakCommand_Clicked(object? sender, EventArgs e)
    {
        try
        {
            // Windows desktop check for speech compatibility during local testing
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                await DisplayAlertAsync("Desktop Testing", "Speech-to-text requires a mobile device (Android/iOS). Simulating voice command: 'Show campus status'!", "OK");
                await ProcessVoiceCommand("Show campus status");
                return;
            }

            VoiceResultLabel.Text = "Listening... Speak now.";

            bool microphoneGranted = await Permissions.RequestAsync<Permissions.Microphone>() == PermissionStatus.Granted;
            if (!microphoneGranted)
            {
                VoiceResultLabel.Text = "Microphone permission denied.";
                await DisplayAlertAsync("Microphone Permission", "Please allow microphone access in the device settings.", "OK");
                return;
            }

            var recognitionResult = await speechToText.ListenAsync(CultureInfo.CurrentCulture, null);

            if (recognitionResult.IsSuccessful)
            {
                string recognizedText = recognitionResult.Text;
                VoiceResultLabel.Text = $"Recognized: {recognizedText}";
                await ProcessVoiceCommand(recognizedText);
            }
            else
            {
                VoiceResultLabel.Text = "Speech could not be recognized.";
                await DisplayAlertAsync("Speech Recognition", recognitionResult.Exception?.Message ?? "No speech was recognized.", "OK");
            }
        }
        catch (Exception ex)
        {
            VoiceResultLabel.Text = "Speech recognition error.";
            await DisplayAlertAsync("Voice Error", ex.Message, "OK");
        }
    }

    private async Task ProcessVoiceCommand(string command)
    {
        string intent = voiceCommandService.ProcessCommand(command);
        switch (intent)
        {
            case "STATUS":
                ShowStatus_Clicked(this, EventArgs.Empty);
                break;
            case "PARKING":
                await DisplayAlertAsync("Available Parking", ParkingLabel.Text, "OK");
                blockchainService.AddTransaction("Parking availability viewed");
                UpdateBlockchainDisplay();
                break;
            case "EMERGENCY":
                Emergency_Clicked(this, EventArgs.Empty);
                break;
            default:
                await DisplayAlertAsync("Unknown Command", $"The application heard:\n\n{command}\n\nbut does not know what action to perform.", "OK");
                break;
        }
    }

    private async void ShowStatus_Clicked(object? sender, EventArgs e)
    {
        blockchainService.AddTransaction("Campus status viewed");
        UpdateBlockchainDisplay();
        await DisplayAlertAsync("Campus Status", $"{StudentsLabel.Text}\n{OccupancyLabel.Text}\n{ParkingLabel.Text}\n{TemperatureLabel.Text}\n\n{StatusLabel.Text}", "OK");
    }

    private async void Emergency_Clicked(object? sender, EventArgs e)
    {
        blockchainService.AddTransaction("Emergency mode activated");
        UpdateBlockchainDisplay();
        await DisplayAlertAsync("Emergency Mode", "Emergency mode activated.", "OK");
    }

    private void UpdateBlockchainDisplay()
    {
        var latest = blockchainService.GetLatestBlock();
        BlockchainLabel.Text = $"Block #{latest.Index}\n" +
                               $"Transaction: {latest.Transaction}\n" +
                               $"Hash: {latest.Hash[..12]}...\n" +
                               $"Chain Valid: {blockchainService.VerifyChain()}";
    }
}