using MountAnything;

namespace MountVault.Authentication;

public static class AuthenticationFactory
{
    public static IAuthenticator GetAuthenticator(VaultDriveInfo driveInfo)
    {
        return driveInfo.AuthType switch
        {
            VaultAuthType.Ldap => new LdapAuthenticator(),
            VaultAuthType.TokenEnv => new EnvironmentVariableAuthenticator(),
            _ => throw new InvalidOperationException($"The VaultAuthType '{driveInfo.AuthType}' is not currently supported")
        };
    }
}