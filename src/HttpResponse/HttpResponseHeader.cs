using System.Text;

namespace codecrafters_http_server.HttpResponse;

public class HttpResponseHeader
{
    private const string NewLine = "\r\n";
    public StatusLine StatusLine { get; set; }
    public Dictionary<string, string> Headers { get; } = new(0);
    
    public HttpResponseHeader(StatusLine statusLine)
    {
        StatusLine = statusLine;
    }
    
    public void Serialize(StringBuilder sb)
    {
        sb.Append(StatusLine.ToString());
        sb.Append(NewLine);

        foreach (var (name, value) in Headers)
        {
            sb.Append($"{name}: {value}");
            sb.Append(NewLine);
        }

        sb.Append(NewLine);
    }
}