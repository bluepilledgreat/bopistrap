using Bopistrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.UI.Bootstrappers
{
    internal class DummyWindow : IBootstrapperDialog
    {
        public string Message { get; set; } = "";
        public double ProgressBarValue { get; set; } = 0;
        public bool ProgressBarVisible { get; set; } = false;
        public string GameVersion { get; set; } = "";
    }
}
