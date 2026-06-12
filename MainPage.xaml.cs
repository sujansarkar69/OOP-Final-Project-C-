using System.Diagnostics;
using PasswordResetBruteForce_GUI.Models;
using PasswordResetBruteForce_GUI.Services;

namespace PasswordResetBruteForce_GUI;

public partial class MainPage : ContentPage
{
    private readonly PasswordGenerator _passwordGenerator;
    private readonly PasswordHasherService _hasher;
    private readonly PasswordValidator _validator;
    private readonly BruteForceGenerator _bruteForceGenerator;
    private readonly SingleThreadBruteForcer _singleThreadBruteForcer;
    private readonly MultiThreadBruteForcer _multiThreadBruteForcer;
    private readonly PerformanceLogger _performanceLogger;

    private string _createdPassword = "";
    private string _targetHash = "";

    private CancellationTokenSource? _cancellationTokenSource;
    private Stopwatch? _uiStopwatch;
    private IDispatcherTimer? _timer;

    private int _selectedThreadCount = 1;

    private const int MaxLength = 6;

    public MainPage()
    {
        InitializeComponent();

        _passwordGenerator = new PasswordGenerator();
        _hasher = new PasswordHasherService();
        _validator = new PasswordValidator(_hasher);
        _bruteForceGenerator = new BruteForceGenerator(PasswordGenerator.CharacterSet);
        _singleThreadBruteForcer = new SingleThreadBruteForcer(_bruteForceGenerator, _validator);
        _multiThreadBruteForcer = new MultiThreadBruteForcer(_bruteForceGenerator, _validator);
        _performanceLogger = new PerformanceLogger();

        SetupTimer();
        SetupThreadSlider();
    }

    private void SetupTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(200);

        _timer.Tick += (s, e) =>
        {
            if (_uiStopwatch != null)
            {
                ElapsedTimeLabel.Text = $"{_uiStopwatch.Elapsed.TotalSeconds:F2} seconds";
            }
        };
    }

    private void SetupThreadSlider()
    {
        int maxThreads = Math.Max(1, Environment.ProcessorCount);

        ThreadSlider.Minimum = 1;
        ThreadSlider.Maximum = maxThreads;
        ThreadSlider.Value = Math.Max(1, maxThreads - 1);

        _selectedThreadCount = (int)Math.Round(ThreadSlider.Value);
        ThreadCountLabel.Text = $"Threads: {_selectedThreadCount} / Max CPU: {maxThreads}";
    }

    private void OnThreadSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        _selectedThreadCount = (int)Math.Round(e.NewValue);
        ThreadSlider.Value = _selectedThreadCount;

        int maxThreads = Math.Max(1, Environment.ProcessorCount);
        ThreadCountLabel.Text = $"Threads: {_selectedThreadCount} / Max CPU: {maxThreads}";
    }

    private void OnCreatePasswordClicked(object? sender, EventArgs e)
    {
        _createdPassword = _passwordGenerator.GeneratePassword();
        _targetHash = _hasher.HashPassword(_createdPassword);

        CreatedPasswordLabel.Text = _createdPassword;
        HashLabel.Text = _targetHash;

        FoundPasswordLabel.Text = "Not found yet";
        AttackProgressBar.Progress = 0;
        ProgressLabel.Text = "0%";
        AttemptsLabel.Text = "0";
        ElapsedTimeLabel.Text = "0 seconds";
    }

    private async void OnStartSingleThreadClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_targetHash))
        {
            await DisplayAlertAsync("Error", "Please create a password first.", "OK");
            return;
        }

        await StartAttackAsync("Single-thread");
    }

    private async void OnStartMultiThreadClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_targetHash))
        {
            await DisplayAlertAsync("Error", "Please create a password first.", "OK");
            return;
        }

        await StartAttackAsync($"Multi-thread ({_selectedThreadCount} threads)");
    }

    private async Task StartAttackAsync(string attackType)
    {
        ResetAttackUi();

        _cancellationTokenSource = new CancellationTokenSource();

        _uiStopwatch = Stopwatch.StartNew();
        _timer?.Start();

        Progress<double> progress = new Progress<double>(value =>
        {
            AttackProgressBar.Progress = value;
            ProgressLabel.Text = $"{value * 100:F2}%";
        });

        Progress<long> attemptsProgress = new Progress<long>(attempts =>
        {
            AttemptsLabel.Text = attempts.ToString();
        });

        AttackResult result;

        if (attackType.StartsWith("Single-thread"))
        {
            result = await _singleThreadBruteForcer.StartAsync(
                _targetHash,
                MaxLength,
                progress,
                attemptsProgress,
                _cancellationTokenSource.Token);
        }
        else
        {
            result = await _multiThreadBruteForcer.StartAsync(
                _targetHash,
                MaxLength,
                _selectedThreadCount,
                progress,
                attemptsProgress,
                _cancellationTokenSource.Token);
        }

        _uiStopwatch.Stop();
        _timer?.Stop();

        if (result.IsFound)
        {
            FoundPasswordLabel.Text = result.FoundPassword;
            AttackProgressBar.Progress = 1;
            ProgressLabel.Text = "100%";
        }
        else
        {
            FoundPasswordLabel.Text = "Stopped or not found";
        }

        AttemptsLabel.Text = result.Attempts.ToString();
        ElapsedTimeLabel.Text = $"{result.ElapsedTime.TotalSeconds:F3} seconds";

        _performanceLogger.AddLog(attackType, result);
        LogEditor.Text = _performanceLogger.GetLogs();
    }

    private void ResetAttackUi()
    {
        FoundPasswordLabel.Text = "Searching...";
        AttackProgressBar.Progress = 0;
        ProgressLabel.Text = "0%";
        AttemptsLabel.Text = "0";
        ElapsedTimeLabel.Text = "0 seconds";
    }

    private void OnStopClicked(object? sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();

        _uiStopwatch?.Stop();
        _timer?.Stop();

        FoundPasswordLabel.Text = "Stopped by user";
    }
}