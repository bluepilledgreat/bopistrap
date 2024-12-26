using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bopistrap.Extensions
{
    public static class JsonSerializerEx
    {
        public static T DeserializeSafe<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json) ?? throw new Exception($"JsonSerializer.Deserialize<{typeof(T).Name}> returned null");
        }
    }
}
