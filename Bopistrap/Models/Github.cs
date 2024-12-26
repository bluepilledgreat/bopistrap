using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bopistrap.Models
{
    internal class GithubAsset
    {
        [JsonPropertyName("name")]
        public string FileName { get; set; } = null!;

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; } = null!;
    }

    internal class GithubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = null!;

        [JsonPropertyName("assets")]
        public GithubAsset[] Assets { get; set; } = null!;
    }
}
