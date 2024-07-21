using codecrafters_http_server.Utils;

namespace codecrafters_http_server;

public static class Program
{
    private const int CodeCraftersTestPort = 4221;
    public static async Task Main(string[] args)
    {
        using var monitor = new ProcessCancellationMonitor();
        var options = new ServerOptions()
        {
            Port = CodeCraftersTestPort,
            BufferSize = 1024,
            MaxConcurrency = 3
        };

        using var server = new HttpServer(options);
        
        monitor.OnProcessQuit += static () =>
        {
            Logger.Warning("User requested server shutdown");
        };
        
        Logger.Info("Server started");
        await server.RunAsync(monitor.CancellationToken);
        Logger.Info("Server stopped");
    }
}