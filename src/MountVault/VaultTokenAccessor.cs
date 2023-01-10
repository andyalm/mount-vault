using MountAnything;
using MountVault.Authentication;

namespace MountVault;

public class VaultTokenAccessor
{
    private readonly IAuthenticator _authenticator;
    private readonly VaultDriveInfo _driveInfo;
    private readonly IPathHandlerContext _context;

    public VaultTokenAccessor(IAuthenticator authenticator, VaultDriveInfo driveInfo, IPathHandlerContext context)
    {
        _authenticator = authenticator;
        _driveInfo = driveInfo;
        _context = context;
    }

    public string VaultToken
    {
        get
        {
            if (_driveInfo.CachedAuthResult?.IsExpired == false)
            {
                return _driveInfo.CachedAuthResult.Token;
            }

            _driveInfo.CachedAuthResult = _authenticator.Authenticate(_driveInfo, _context);

            return _driveInfo.CachedAuthResult.Token;
        }
    }
}