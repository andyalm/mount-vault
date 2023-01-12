using System.Management.Automation;
using Microsoft.Extensions.DependencyInjection;
using MountAnything;
using MountAnything.Routing;
using MountVault.Authentication;
using MountVault.Secrets;

namespace MountVault;

public class MountVaultProvider : MountAnythingProvider<VaultDriveParameters>
{
    public override Router CreateRouter()
    {
        var router = Router.Create<RootHandler>();
        router.ConfigureServices(services =>
        {
            services.AddDriveInfo<VaultDriveInfo>();
            services.AddTransient(s => AuthenticationFactory.GetAuthenticator(s.GetRequiredService<VaultDriveInfo>()));
            services.AddTransient<VaultClient>();
        });
        router.MapLiteral<SecretRootHandler>(SecretRootHandler.LiteralContainerName, secret =>
        {
            secret.MapRecursive<SecretHandler, SecretPath>();
        });

        return router;
    }
    
    protected override PSDriveInfo NewDrive(PSDriveInfo driveInfo, VaultDriveParameters parameters)
    {
        var vaultDrive = new VaultDriveInfo(driveInfo, parameters);
        
        AuthenticationFactory.GetAuthenticator(vaultDrive).Validate(vaultDrive);

        return vaultDrive;
    }
}
