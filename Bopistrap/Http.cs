using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal static class Http
    {
        private static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.All, AllowAutoRedirect = false });
            client.DefaultRequestHeaders.Add("User-Agent", $"Bopistrap/{Bootstrapper.Version}");
            return client;
        }

        public static HttpClient Client { get; } = CreateHttpClient();
    }
}
