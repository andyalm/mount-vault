using System.Management.Automation;
using MountAnything;

namespace MountVault.Secrets;

public class SecretItem : Item
{
    public SecretItem(ItemPath parentPath, ItemPath keyPath, bool? isContainer = true) : base(parentPath, new PSObject())
    {
        FullKey = keyPath.FullName;
        ItemName = keyPath.Name;
        IsContainer = isContainer ?? true;
        IsPartial = isContainer == null;
    }

    [ItemProperty]
    public string FullKey { get; }
    public override string ItemName { get; }
    public override bool IsContainer { get; }
    public override bool IsPartial { get; }
}