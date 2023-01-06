using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using MountAnything;
using MountVault.Http;
using MountVault.Secrets;

namespace MountVault;

public class VaultClient : IDisposable
{
    // initialize lazily so errors won't be thrown during construction time (leads to nasty autofac errors)
    private readonly Lazy<HttpClient> _client;

    public VaultClient(VaultDriveInfo vaultDrive, IPathHandlerContext context)
    {
        _client = new Lazy<HttpClient>(() => new HttpClient(new DebugLoggingHandler(context, new HttpClientHandler()))
        {
            BaseAddress = vaultDrive.VaultAddress,
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", vaultDrive.GetVaultToken())
            }
        });
    }

    public string[] ListChildren(ItemPath? secretPath = null)
    {
        var path = "v1/secret";
        if (secretPath != null)
        {
            path += $"/{secretPath}";
        }

        try
        {
            return _client.Value.GetJson<VaultApiResponse<ListResponse>>($"{path}?list=true").Data.Keys;
        }
        catch (HttpRequestException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Array.Empty<string>();
        }
    }

    public Dictionary<string, string> GetSecret(ItemPath secretPath)
    {
        return _client.Value.GetJson<VaultApiResponse<Dictionary<string, string>>>(new ItemPath("v1/secret").Combine(secretPath).ToString()).Data;
    }
    
    public bool SecretExists(ItemPath secretPath)
    {
        try
        {
            _client.Value.GetJson<VaultApiResponse<Dictionary<string, string>>>(new ItemPath("v1/secret").Combine(secretPath).ToString());
            return true;
        }
        catch (HttpRequestException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public void SetSecret(ItemPath secretPath, Dictionary<string,string> secretValues)
    {
        _client.Value.PostJson(new ItemPath("v1/secret").Combine(secretPath).ToString(), secretValues);
    }

    public void DeleteSecret(ItemPath secretPath)
    {
        _client.Value.Delete(new ItemPath("v1/secret").Combine(secretPath).ToString());
    }

    private class DebugLoggingHandler : DelegatingHandler
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

    public void Dispose()
    {
        if (_client.IsValueCreated)
        {
            _client.Value.Dispose();
        }
    }
}