using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_http_server;
using codecrafters_http_server.HttpResponse;
using codecrafters_http_server.Utils;
using HttpResponseHeader = codecrafters_http_server.HttpResponse.HttpResponseHeader;

public class HttpServer : IDisposable
{
    private StringBuilderObjectPool _sbPool;
    private readonly TcpListener _listener;
    private readonly SafeArrayPool<byte> _arrayPool;
    private readonly ServerOptions _serverOptions;
    
    public int Port => _serverOptions.Port;

    public HttpServer(ServerOptions options)
    {
        _serverOptions = options;
        _listener = new TcpListener(IPAddress.Any, options.Port);
        _arrayPool = new(options.BufferSize, options.MaxConcurrency);
        _sbPool = new StringBuilderObjectPool(options.MaxConcurrency, options.BufferSize);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Logger.Info($"Starting server on port {Port}");
        _listener.Start();
        _listener.Server.NoDelay = true;

        try
        {
            await RunAsyncCore(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            //Ignore cancellation
        }
    }

    private async Task RunAsyncCore(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var socket = await _listener.AcceptSocketAsync(cancellationToken);
            var response = await ProcessRequest(socket, cancellationToken);
            
            if (cancellationToken.IsCancellationRequested)
                break;

            await SendResponse(socket, response, cancellationToken);
            await socket.DisconnectAsync(true, cancellationToken);
        }
    }

    private async ValueTask<HttpResponse> ProcessRequest(Socket socket, CancellationToken cancellationToken)
    {
        //Just return a 200 OK for now
        Logger.Info($"Processing request from {socket.RemoteEndPoint}");
        return new HttpResponse(HttpStatusCode.OK);
    }

    private async Task SendResponse(Socket socket, HttpResponse response, CancellationToken cancellationToken)
    {
        Logger.Info($"Sending response to {socket.RemoteEndPoint} ({response.ResponseHeader.StatusLine.ReasonPhrase})");
        using var sb = _sbPool.Rent();
        await SendHeader(socket, response.ResponseHeader, cancellationToken);
    }

    private async Task SendHeader(Socket socket, HttpResponseHeader header, CancellationToken cancellationToken)
    {
        using var sb = _sbPool.Rent();
        header.Serialize(sb);
        var headerContent = sb.Value.ToString();
        var responseLength = headerContent.Length; //Header is ASCII, so length is already correct
        var bufferSize = _serverOptions.BufferSize;
        var parts = responseLength / bufferSize + (responseLength % bufferSize != 0 ? 1 : 0);
        
        //Safer, since compiler implicitly wraps usage in try/finally block with using statement
        using var rentedBuffer = _arrayPool.Rent(bufferSize);
        
        for (var i = 0; i < parts; i++)
        {
            var startIndex = i * bufferSize;
            var charCount = Math.Min(bufferSize, responseLength - startIndex);
            var writtenBytes = Encoding.ASCII.GetBytes(headerContent, i * bufferSize, charCount, rentedBuffer, 0);
            if (i == parts - 1)
            {
                await socket.SendAsync(rentedBuffer.Array.AsMemory(0, writtenBytes), SocketFlags.None, cancellationToken);
            }
            else
            {
                await socket.SendAsync(rentedBuffer.Array.AsMemory(0, writtenBytes), SocketFlags.Partial, cancellationToken);
            }
        }
    }

    public void Dispose()
    {
        Logger.Info("Server disposed");
        _listener.Dispose();
    }
}
