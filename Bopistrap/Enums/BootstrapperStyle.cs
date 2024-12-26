using Avalonia.Media;
using Bopistrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Enums
{
    internal enum BootstrapperStyle
    {
        Purple,
        Green,
        Red
    }

    internal static class BootstrapperStyleEx
    {
        private static readonly Dictionary<BootstrapperStyle, ColourPalette> _colourPaletteMap = new()
        {
            [BootstrapperStyle.Purple] = new ColourPalette
            {
                Background = new SolidColorBrush(Color.FromRgb(0x82, 0x0C, 0x9B)),
                ProgressBarBorder = new SolidColorBrush(Color.FromRgb(0x45, 0x00, 0x85)),
                IndicatorBackground = new SolidColorBrush(Color.FromRgb(0x71, 0x1A, 0xC4)),
                IndicatorForeground = new SolidColorBrush(Color.FromRgb(0x61, 0x00, 0xBD)),
                WindowBorder = new SolidColorBrush(Color.FromRgb(0x36, 0x09, 0x3F)),
            },
            [BootstrapperStyle.Green] = new ColourPalette // Hue change of -162
            {
                Background = new SolidColorBrush(Color.FromRgb(0x22, 0x90, 0x0B)),
                ProgressBarBorder = new SolidColorBrush(Color.FromRgb(0x18, 0x85, 0x00)), // Original: 22900B
                IndicatorBackground = new SolidColorBrush(Color.FromRgb(0x3A, 0xC4, 0x1A)), // Original: 22900B
                IndicatorForeground = new SolidColorBrush(Color.FromRgb(0x23, 0xBD, 0x00)), // Original: 76CA4F
                WindowBorder = new SolidColorBrush(Color.FromRgb(0x14, 0x51, 0x06)),
            },
            [BootstrapperStyle.Red] = new ColourPalette // Hue change of +68
            {
                Background = new SolidColorBrush(Color.FromRgb(0x9C, 0x0C, 0x11)),
                ProgressBarBorder = new SolidColorBrush(Color.FromRgb(0x85, 0x00, 0x2E)),
                IndicatorBackground = new SolidColorBrush(Color.FromRgb(0xC4, 0x1A, 0x56)),
                IndicatorForeground = new SolidColorBrush(Color.FromRgb(0xBD, 0x00, 0x43)),
                WindowBorder = new SolidColorBrush(Color.FromRgb(0x3F, 0x09, 0x0B)),
            }
        };

        public static Uri GetLogoPath(this BootstrapperStyle style)
        {
            return new Uri($"avares://Bopistrap/Resources/Logos/{style}.png");
        }

        public static Uri GetIconPath(this BootstrapperStyle style)
        {
            return new Uri($"avares://Bopistrap/Resources/Logos/{style}.ico");
        }

        public static ColourPalette GetColourPalette(this BootstrapperStyle style)
        {
            return _colourPaletteMap[style];
        }
    }
}
