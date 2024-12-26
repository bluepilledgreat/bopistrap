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
        public static HttpClient Client { get; } = new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.All, AllowAutoRedirect = false });
    }
}
