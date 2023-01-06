using MountAnything;

namespace MountVault.Secrets;

public record SecretValue(string Name, string Value) : IItemProperty
{
    object IItemProperty.Value => Value;
}