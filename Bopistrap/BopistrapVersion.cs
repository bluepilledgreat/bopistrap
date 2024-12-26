using Bopistrap.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bopistrap
{
    /// <summary>
    /// Handles versions such as v1.3.7-dev21
    /// </summary>
    internal class BopistrapVersion : IComparable<BopistrapVersion>
    {
        public Version Version { get; set; }
        public VersionStage Stage { get; set; }
        public int StageIdentifier { get; set; }

        public BopistrapVersion(Version version)
        {
            Version = version;
            Stage = VersionStage.Release;
            StageIdentifier = 0;
        }

        public BopistrapVersion(Version version, VersionStage stage, int stageIdentifier)
        {
            Version = version;
            Stage = stage;
            StageIdentifier = stageIdentifier;
        }

        public int CompareTo(BopistrapVersion? other)
        {
            // -1 = Less than other
            //  0 = Same as other
            // +1 = Greater than other
            int result;

            if (other == null) return 1;

            result = Version.CompareTo(other.Version);
            if (result != 0) return result;

            result = Stage.CompareTo(other.Stage);
            if (result != 0) return result;

            result = StageIdentifier.CompareTo(other.StageIdentifier);
            return result;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (obj is not BopistrapVersion version)
                return false;

            return version == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version, Stage, StageIdentifier);
        }

        public override string ToString()
        {
            string str = Version.ToString();

            if (Stage != VersionStage.Release)
            {
                str += $"-{Stage.GetVersionFriendlyString()}";

                if (StageIdentifier != 0)
                    str += StageIdentifier.ToString();
            }

            return str;
        }

        public static BopistrapVersion Parse(string str)
        {
            // remove version prefix if one is present
            if (str.StartsWith('v'))
                str = str[1..];

            string[] parts = str.Split('-');

            if (parts.Length == 1)
            {
                // v1.0.0
                string versionStr = parts[0];
                Version version = new Version(versionStr);

                return new BopistrapVersion(version);
            }
            else if (parts.Length == 2)
            {
                // v1.0.0-dev or v1.0.0-dev1
                string versionStr = parts[0];
                string stageStr = parts[1];

                Version version = new Version(versionStr);

                VersionStage foundStage = VersionStage.Release;

                foreach (VersionStage stage in Enum.GetValues<VersionStage>())
                {
                    if (stage == VersionStage.Release)
                        continue;

                    string friendlyId = stage.GetVersionFriendlyString();

                    if (stageStr.StartsWith(friendlyId))
                    {
                        stageStr = stageStr[friendlyId.Length..];
                        foundStage = stage;
                    }
                }

                if (foundStage == VersionStage.Release)
                    throw new Exception($"Unknown stage in version {str}");

                int stageId = !string.IsNullOrEmpty(stageStr) ? int.Parse(stageStr) : 0;

                return new BopistrapVersion(version, foundStage, stageId);
            }
            else
            {
                throw new Exception($"Invalid version {str}");
            }
        }

        public static bool TryParse(string str, [NotNullWhen(true)] out BopistrapVersion? version)
        {
            try
            {
                version = Parse(str);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to parse version {str}");
                Logger.WriteLine(ex);

                version = null;
                return false;
            }
        }

        public static bool operator ==(BopistrapVersion? a, BopistrapVersion? b) => a is null ? b is null : a.CompareTo(b) == 0;
        public static bool operator !=(BopistrapVersion? a, BopistrapVersion? b) => !(a == b);

        public static bool operator <(BopistrapVersion? a, BopistrapVersion? b) => a is null ? (b is not null) : a.CompareTo(b) == -1;
        public static bool operator >(BopistrapVersion? a, BopistrapVersion? b) => a is null ? b is null : a.CompareTo(b) == 1;
        public static bool operator <=(BopistrapVersion? a, BopistrapVersion? b) => a < b || a == b;
        public static bool operator >=(BopistrapVersion? a, BopistrapVersion? b) => a > b || a == b;

        public static void PerformTests()
        {
#if DEBUG
            Debug.Assert(Parse("v1.0.0") == Parse("v1.0.0"));
            Debug.Assert(Parse("v1.0.2") > Parse("v1.0.0"));
            Debug.Assert(Parse("v1.0.2") < Parse("v1.0.3"));
            Debug.Assert(Parse("v1.0.2-dev") < Parse("v1.0.3"));
            Debug.Assert(Parse("v1.0.2-dev1") < Parse("v1.0.2"));
            Debug.Assert(Parse("v1.0.2-dev1") > Parse("v1.0.1"));
            Debug.Assert(Parse("v1.0.2-dev1") > Parse("v1.0.2-dev"));
            Debug.Assert(Parse("v1.0.2-alpha") > Parse("v1.0.2-dev"));
            Debug.Assert(Parse("v1.0.2-beta") > Parse("v1.0.2-alpha"));
            Debug.Assert(Parse("v1.0.2-rc") > Parse("v1.0.2-beta"));
            Debug.Assert(Parse("v1.0.2") > Parse("v1.0.2-rc"));
            Debug.Assert(Parse("v1.0.2-rc") > Parse("v1.0.2-dev"));

            Logger.WriteLine("BopistrapVersion tests have passed");
#endif
        }
    }
}
