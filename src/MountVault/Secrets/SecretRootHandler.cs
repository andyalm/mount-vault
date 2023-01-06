using System.Reflection.Metadata.Ecma335;
using MountAnything;

namespace MountVault.Secrets;

public class SecretRootHandler : PathHandler
{
    private readonly VaultClient _client;

    public SecretRootHandler(ItemPath path, IPathHandlerContext context, VaultClient client) : base(path, context)
    {
        _client = client;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _client.ListChildren().Select(key => new SecretItem(Path, new ItemPath(key)));
    }

    public static string LiteralContainerName => "secret";

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, LiteralContainerName)
        {
            Description = "Browse the secrets hidden in the vault"
        };
    }
}