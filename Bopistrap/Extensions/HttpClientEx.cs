using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private static bool IsRedirectCode(HttpStatusCode code)
        {
            return code is HttpStatusCode.Found or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect or HttpStatusCode.Moved;
        }

        public static async Task<HttpResponseMessage> GetFollowAsync(this HttpClient client, string? requestUri, HttpCompletionOption completionOption, CancellationToken token)
        {
            HttpResponseMessage response;

            while (true)
            {
                response = await client.GetAsync(requestUri, completionOption, token);

                if (!IsRedirectCode(response.StatusCode))
                    break;

                if (!response.Headers.TryGetValues("Location", out IEnumerable<string>? locationValues))
                    throw new Exception($"Location header not found in the headers of {requestUri}");

                requestUri = locationValues.First();
                response.Dispose();
            }

            return response;
        }
    }
}
