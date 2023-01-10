using System.Net;
using System.Text.Json.Serialization;
using MountAnything;
using MountVault.Http;

namespace MountVault.Authentication;

public class LdapAuthenticator : IAuthenticator
{
    public void Validate(VaultDriveInfo driveInfo)
    {
        if (string.IsNullOrEmpty(driveInfo.Credential?.UserName))
        {
            throw new InvalidOperationException("You must pass credentials to the PS drive when using the ldap AuthType");
        }
    }

    public AuthResult Authenticate(VaultDriveInfo driveInfo, IPathHandlerContext context)
    {
        using var client = new HttpClient(new DebugLoggingHandler(context, new HttpClientHandler()))
        {
            BaseAddress = driveInfo.VaultAddress
        };

        var response = client.PostJson<Response>($"v1/auth/ldap/login/{WebUtility.UrlEncode(driveInfo.Credential.UserName)}", new
        {
            password = driveInfo.Credential.GetNetworkCredential().Password
        });

        return new AuthResult(response.Auth.ClientToken, response.Auth.LeaseDuration);
    }

    private record Response(AuthResponse Auth);
    
    public record AuthResponse
    {
        [JsonPropertyName("client_token")] 
        public string ClientToken { get; init; } = null!;

        [JsonPropertyName("lease_duration")]
        public uint LeaseDuration { get; init; }
        
        public bool Renewable { get; init; }
    }
}

