using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bopistrap.Extensions
{
    internal static class HttpClientEx
    {
        public static async Task<T> GetFromJsonSafeAsync<T>(this HttpClient client, string? requestUri, CancellationToken token)
        {
            return await client.GetFromJsonAsync<T>(requestUri, token) ?? throw new Exception($"HttpClient.GetFromJsonAsync<{typeof(T).Name}> returned null");
        }
    }
}
