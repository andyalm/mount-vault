using System.Diagnostics;
using MountAnything;

namespace MountVault.Http;

public class DebugLoggingHandler : DelegatingHandler
{
    private readonly IPathHandlerContext _context;

    public DebugLoggingHandler(IPathHandlerContext context, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        _context = context;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _context.WriteDebug(ToMessage(request));
        Stopwatch timer = new Stopwatch();
        timer.Start();
        var response = base.Send(request, cancellationToken);
        _context.WriteDebug($"{response.StatusCode:D} ({response.StatusCode}) in {timer.ElapsedMilliseconds}ms");

        return response;
    }

    private string ToMessage(HttpRequestMessage request)
    {
        return $"{request.Method.Method} {request.RequestUri}";
    }
}