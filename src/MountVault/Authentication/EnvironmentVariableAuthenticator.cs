using MountAnything;

namespace MountVault.Authentication;

public class EnvironmentVariableAuthenticator : IAuthenticator
{
    public AuthResult Authenticate(VaultDriveInfo driveInfo, IPathHandlerContext context)
    {
        var vaultToken = Environment.GetEnvironmentVariable(driveInfo.VaultTokenEnvironmentVariable);
        if (string.IsNullOrEmpty(vaultToken))
        {
            throw new InvalidOperationException(
                $"The environment variable '{driveInfo.VaultTokenEnvironmentVariable}' is not set");
        }

        return new AuthResult(vaultToken);
    }
}