using System.Net;
using codecrafters_http_server.Utils;

namespace codecrafters_http_server.HttpResponse;

public record StatusLine(Version Version, HttpStatusCode StatusCode, string ReasonPhrase)
{
    private static readonly Dictionary<HttpStatusCode, string> EnumCache =
        EnumValueExtractor.GetEnumValues<HttpStatusCode>(StringConversion.ToUpper);

    public static StatusLine Create(HttpStatusCode statusCode) =>
        new(HttpVersion.Version11, statusCode, EnumCache[statusCode]);
    public override string ToString()
    {
        return $"HTTP/{Version.Major}.{Version.Minor} {(int)StatusCode} {ReasonPhrase}";
    }
}