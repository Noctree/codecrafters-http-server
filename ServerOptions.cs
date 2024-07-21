namespace codecrafters_http_server;

public class ServerOptions
{
    public int Port { get; set; }
    public int BufferSize { get; set; } = 1024;
    public int MaxConcurrency { get; set; } = Environment.ProcessorCount - 1;
}