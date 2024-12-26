using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Bopistrap.Enums;
using Bopistrap.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.UI.ViewModels
{
    internal partial class BopimoWindowViewModel : ViewModelBase
    {
        public BopimoWindowViewModel()
        {
            // TODO: use a resource dictionary for this
            BootstrapperStyle style = Settings.Default.User.BootstrapperStyle;
            Logo = ImageHelper.LoadFromResource(style.GetLogoPath());
            Icon = ImageHelper.LoadIconFromResource(style.GetIconPath());
            ColourPalette = style.GetColourPalette();
        }

        [ObservableProperty]
        private string _message = "Welcome to Bopistrap!";

        [ObservableProperty]
        private double _progressBarValue = 0;

        [ObservableProperty]
        private double _progressBarOpacity = 0;

        [ObservableProperty]
        private string _gameVersion = "";

        public string BopistrapVersion => $"Bopistrap v{Bootstrapper.Version}";

        // Customisability
        public Bitmap Logo { get; }
        public WindowIcon Icon { get; }
        public ColourPalette ColourPalette { get; }
        public Color BackgroundGradientTop => Color.FromArgb(0, ColourPalette.Background.Color.R, ColourPalette.Background.Color.G, ColourPalette.Background.Color.B);
        public Color BackgroundGradientBottom => Color.FromArgb(0x99, ColourPalette.Background.Color.R, ColourPalette.Background.Color.G, ColourPalette.Background.Color.B);

        // TODO: is there any better way to animate an imagebrush's DestinationRect?
        [ObservableProperty]
        private RelativeRect _backgroundDestinationRect = new RelativeRect(new Rect(0, 0, 135, 135), RelativeUnit.Absolute);
    }
}
