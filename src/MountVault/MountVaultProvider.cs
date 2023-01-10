using Autofac;
using MountAnything;
using MountAnything.Routing;
using MountVault.Authentication;
using MountVault.Secrets;

namespace MountVault;

public class MountVaultProvider : IMountAnythingProvider
{
    public Router CreateRouter()
    {
        var router = Router.Create<RootHandler>();
        router.RegisterServices(builder =>
        {
            builder.Register(c => (VaultDriveInfo)c.Resolve<IPathHandlerContext>().DriveInfo);
            builder.Register(c =>
                AuthenticationFactory.GetAuthenticator(c.Resolve<VaultDriveInfo>()));
            builder.RegisterType<VaultClient>();
        });
        router.MapLiteral<SecretRootHandler>(SecretRootHandler.LiteralContainerName, secret =>
        {
            secret.MapRecursive<SecretHandler, SecretPath>();
        });

        return router;
    }

    public IDriveHandler GetDriveHandler()
    {
        return new VaultDriveHandler();
    }
}
