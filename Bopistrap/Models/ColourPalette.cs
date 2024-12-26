using Avalonia.Media;

namespace Bopistrap.Models
{
    internal struct ColourPalette
    {
        /// <summary>
        /// Used for the gradient on the background
        /// </summary>
        public SolidColorBrush Background { get; set; }

        /// <summary>
        /// Used for the progress bar's border
        /// </summary>
        public SolidColorBrush ProgressBarBorder { get; set; }

        /// <summary>
        /// Used for the progress bar indicator background
        /// </summary>
        public SolidColorBrush IndicatorBackground { get; set; }

        /// <summary>
        /// Used for the stripes on the progress bar indicator
        /// </summary>
        public SolidColorBrush IndicatorForeground { get; set; }

        /// <summary>
        /// Used for the gradient on the window border
        /// </summary>
        public SolidColorBrush WindowBorder { get; set; }
    }
}
