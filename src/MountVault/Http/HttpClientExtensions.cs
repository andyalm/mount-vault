using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MountVault.Http;

public static class HttpClientExtensions
{
    public static TResponseBody GetJson<TResponseBody>(this HttpClient client, string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = client.Send(request);

        ThrowIfNotSuccessful(response, request);
        
        using var responseStream = response.Content.ReadAsStream();
        return JsonSerializer.Deserialize<TResponseBody>(responseStream, Options)!;
    }

    public static TResponseBody PostJson<TResponseBody>(this HttpClient client, string uri, object postBody)
    {
        var response = client.PostJson(uri, postBody);

        using var responseStream = response.Content.ReadAsStream();
        return JsonSerializer.Deserialize<TResponseBody>(responseStream, Options)!;
    }
    
    public static HttpResponseMessage PostJson(this HttpClient client, string uri, object postBody)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = JsonContent.Create(postBody, options: Options);
        var response = client.Send(request);

        ThrowIfNotSuccessful(response, request);

        return response;
    }

    private static void ThrowIfNotSuccessful(HttpResponseMessage response, HttpRequestMessage request)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Error response {response.StatusCode:D} ({response.StatusCode}) from {request.Method} {request.RequestUri}",
                null, response.StatusCode);
        }
    }

    public static HttpResponseMessage Get(this HttpClient client, string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = client.Send(request);
        ThrowIfNotSuccessful(response, request);

        return response;
    }

    public static void Delete(this HttpClient client, string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        var response = client.Send(request);
        ThrowIfNotSuccessful(response, request);
    }
    
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
}