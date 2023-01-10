using System.Management.Automation;
using MountAnything;

namespace MountVault.Authentication;

public interface IAuthenticator
{
    void Validate(VaultDriveInfo driveInfo) { }

    AuthResult Authenticate(VaultDriveInfo driveInfo, IPathHandlerContext context);
}