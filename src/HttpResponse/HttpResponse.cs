using System.Net;
using System.Text;

namespace codecrafters_http_server.HttpResponse;

public class HttpResponse
{
    public HttpResponseHeader ResponseHeader { get; set; }

    public HttpResponse(HttpStatusCode statusCode)
    {
        var statusLine = StatusLine.Create(statusCode);
        ResponseHeader = new HttpResponseHeader(statusLine);
    }

    public string Serialize()
    {
        var sb = new StringBuilder();
        ResponseHeader.Serialize(sb);
        return sb.ToString();
    }
}