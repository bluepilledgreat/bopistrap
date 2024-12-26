using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Models
{
    internal interface IBootstrapperDialog
    {
        public string Message { get; set; }

        public double ProgressBarValue { get; set; }
        public bool ProgressBarVisible { get; set; }

        public string GameVersion { get; set; }
    }
}
