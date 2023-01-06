using MountAnything;
using MountVault.Secrets;

namespace MountVault;

public class RootHandler : PathHandler
{
    public RootHandler(ItemPath path, IPathHandlerContext context) : base(path, context) {}

    protected override IItem GetItemImpl()
    {
        return new RootItem();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return SecretRootHandler.CreateItem(Path);
    }
}