namespace codecrafters_http_server.Utils;

public sealed class ProcessCancellationMonitor : IDisposable
{
    private bool _cancellationRequested;
    private readonly CancellationTokenSource _source = new();

    public CancellationToken CancellationToken => _source.Token;

    public event Action? OnProcessQuit;

    public ProcessCancellationMonitor()
    {
        HookProcessQuitEvents();
    }
    
    private void HookProcessQuitEvents()
    {
        AppDomain.CurrentDomain.DomainUnload += HandleProcessQuit;
        Console.CancelKeyPress += HandleConsoleCancelKeyPress;
    }
    
    private void HandleConsoleCancelKeyPress(object? s, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        HandleProcessQuit(s, e);
    }
    
    private void HandleProcessQuit(object? s, EventArgs e)
    {
        Cancel();
    }
    
    public void Cancel()
    {
        if (_cancellationRequested)
            return;
        OnProcessQuit?.Invoke();
        _source.Cancel();
        _cancellationRequested = true;
    }

    public void Dispose()
    {
        _source.Dispose();
    }
}