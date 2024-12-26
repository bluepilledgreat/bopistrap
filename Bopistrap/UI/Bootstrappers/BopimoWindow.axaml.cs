using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Bopistrap.Models;
using Bopistrap.UI.ViewModels;
using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace Bopistrap.UI.Bootstrappers;

public partial class BopimoWindow : Window, IBootstrapperDialog
{
    private readonly BopimoWindowViewModel _viewModel;

    private readonly object _animationLock = new object();
    private System.Timers.Timer? _animationTimer;
    private Stopwatch _animationStopwatch = null!;
    private double _animationLastTime = 0;

    public string Message { get => _viewModel.Message; set => _viewModel.Message = value; }
    public double ProgressBarValue { get => _viewModel.ProgressBarValue; set => _viewModel.ProgressBarValue = value; }
    public bool ProgressBarVisible { get => _viewModel.ProgressBarOpacity == 1; set => _viewModel.ProgressBarOpacity = value ? 1 : 0; }
    public string GameVersion { get => _viewModel.GameVersion; set => _viewModel.GameVersion = value; }

    public BopimoWindow()
    {
        InitializeComponent();

        // why can't this be set through axaml?
        RenderOptions.SetBitmapInterpolationMode(Logo, Avalonia.Media.Imaging.BitmapInterpolationMode.HighQuality);

#if DEBUG
        this.AttachDevTools();
#endif

        _viewModel = new BopimoWindowViewModel();
        DataContext = _viewModel;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (!Design.IsDesignMode)
        {
            // TODO: find a better way of doing this.
            _animationStopwatch = new Stopwatch();
            _animationStopwatch.Start();

            _animationTimer = new System.Timers.Timer();
            _animationTimer.Elapsed += OnTimedEvent;
            _animationTimer.Interval = 1.0 / 30.0;
            _animationTimer.Enabled = true;
        }

        base.OnLoaded(e);
    }

    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        lock (_animationLock)
        {
            double currentTime = _animationStopwatch.Elapsed.TotalSeconds;
            double delta = currentTime - _animationLastTime;
            _animationLastTime = currentTime;

            //double newX = _viewModel.BackgroundDestinationRect.Rect.X + 0.5;
            //double newY = _viewModel.BackgroundDestinationRect.Rect.Y - 0.5;

            double newX = _viewModel.BackgroundDestinationRect.Rect.X + 15 * delta;
            double newY = _viewModel.BackgroundDestinationRect.Rect.Y - 15 * delta;

            if (newX > 135)
                newX -= 135;

            if (newY < -135)
                newY += 135;

            _viewModel.BackgroundDestinationRect = new RelativeRect(new Rect(newX, newY, 135, 135), RelativeUnit.Absolute);
        }
    }

    // https://github.com/VitalElement/AvalonStudio.Shell/blob/master/src/AvalonStudio.Shell/Controls/MetroWindow.cs
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (TitleBar.IsPointerOver && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }

        base.OnPointerPressed(e);
    }

    private void OnCloseClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //Close();
        Program.StopBootstrapper();
    }

    private void OnMinimiseClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
}