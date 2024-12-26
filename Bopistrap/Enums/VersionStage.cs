using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap.Enums
{
    internal enum VersionStage
    {
        Development,
        Alpha,
        Beta,
        ReleaseCandidate,
        Release
    }

    internal static class VersionStageEx
    {
        /// <summary>
        /// String used for version strings
        /// </summary>
        public static string GetVersionFriendlyString(this VersionStage stage)
        {
            return stage switch
            {
                VersionStage.Development => "dev",
                VersionStage.Alpha => "alpha",
                VersionStage.Beta => "beta",
                VersionStage.ReleaseCandidate => "rc",
                VersionStage.Release => "",
                _ => throw new Exception($"Unrecognised VersionStage {stage}")
            };
        }
    }
}
