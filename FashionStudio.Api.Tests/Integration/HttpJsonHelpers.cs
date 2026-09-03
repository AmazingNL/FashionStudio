using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FashionStudio.Api.Tests.Integration
{
    public static class HttpJsonHelpers
    {
        // The server serializes with camelCase property names and string enum values
        // (Program.cs registers JsonStringEnumConverter globally) — the default
        // System.Text.Json options used by ReadFromJsonAsync<T> match neither, so without
        // this a response like {"id": 1, "status": "New"} would either leave a C# `Id`
        // property at its default value or throw trying to parse "New" as a raw enum number.
        public static readonly JsonSerializerOptions CaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static Task<T?> ReadAsAsync<T>(this HttpResponseMessage response) =>
            response.Content.ReadFromJsonAsync<T>(CaseInsensitive);
    }
}
