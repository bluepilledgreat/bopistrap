using Bopistrap.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bopistrap.Models
{
    internal class ClientReleaseResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = null!;

        [JsonPropertyName("files")]
        public Dictionary<ClientPlatform, string> Files { get; set; } = null!;
    }
}
