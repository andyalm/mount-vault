using System.Collections;
using System.Management.Automation;
using System.Net;
using System.Text.Json;
using MountAnything;
using MountAnything.Content;

namespace MountVault.Secrets;

public class SecretHandler : PathHandler,
    IContentReaderHandler,
    ISetItemPropertiesHandler,
    IClearItemPropertiesHandler,
    INewItemHandler,
    IRemoveItemHandler
{
    private readonly VaultClient _client;
    private readonly SecretPath _secretPath;

    public SecretHandler(ItemPath path, IPathHandlerContext context, VaultClient client, SecretPath secretPath) : base(path, context)
    {
        _client = client;
        _secretPath = secretPath;
    }

    protected override IItem? GetItemImpl()
    {
        WriteDebug($"SecretHandler.GetItemImpl(SecretPath={_secretPath.Path})");
        var children = _client.ListChildren(_secretPath.Path);
        if (children.Any())
        {
            return new SecretItem(ParentPath, _secretPath.Path, isContainer:true);
        }

        try
        {
            // just make sure we don't 404
            _client.GetSecret(_secretPath.Path);

            return new SecretItem(ParentPath, _secretPath.Path, isContainer: false);
        }
        catch (HttpRequestException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _client.ListChildren(_secretPath.Path).Select(key => new SecretItem(Path, _secretPath.Path.Combine(key)));
    }

    public override IEnumerable<IItemProperty> GetItemProperties(HashSet<string> propertyNames, Func<ItemPath, string> pathResolver)
    {
        return _client.GetSecret(_secretPath.Path)
            .Where(pair => propertyNames.Count == 0 || propertyNames.Contains(pair.Key))
            .Select(pair => new SecretValue(pair.Key, pair.Value));
    }

    public IStreamContentReader GetContentReader()
    {
        var secret = _client.GetSecret(_secretPath.Path);
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(secret, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return new StreamContentReader(new MemoryStream(jsonBytes));
    }

    public void SetItemProperties(ICollection<IItemProperty> propertyValues)
    {
        var secretValues = _client.GetSecret(_secretPath.Path);
        foreach (var itemProperty in propertyValues)
        {
            secretValues[itemProperty.Name] = itemProperty.Value.ToString()!;
        }
        _client.SetSecret(_secretPath.Path, secretValues);
    }

    public void ClearItemProperties(ICollection<string> propertyToClear)
    {
        var secretValues = _client.GetSecret(_secretPath.Path);
        foreach (var propertyName in propertyToClear)
        {
            secretValues.Remove(propertyName);
        }
        _client.SetSecret(_secretPath.Path, secretValues);
    }

    public IItem NewItem(string? itemTypeName, object? newItemValue)
    {
        if (_client.SecretExists(_secretPath.Path))
        {
            throw new InvalidOperationException($"The secret '{_secretPath.Path}' already exists");
        }
        
        newItemValue ??= new Hashtable();
        var newItemValues = newItemValue switch
        {
            Hashtable hash => hash.Cast<DictionaryEntry>().ToDictionary(p => (string)p.Key, p => p.Value?.ToString() ?? string.Empty),
            PSObject psObject => ToDictionary(psObject),
            _ => ToDictionary(new PSObject(newItemValue))
        };
        
        _client.SetSecret(_secretPath.Path, newItemValues);

        return new SecretItem(ParentPath, _secretPath.Path, isContainer: false);
    }

    public void RemoveItem()
    {
        switch(GetItem(Freshness.Guaranteed))
        {
            case SecretItem{IsContainer:false}:
                _client.DeleteSecret(_secretPath.Path);
                break;
            case SecretItem{IsContainer:true}:
                throw new InvalidOperationException("Deleting folders is not currently supported");
            default:
                throw new InvalidOperationException($"Secret '{_secretPath.Path}' does not exist");
        }
    }

    private Dictionary<string, string> ToDictionary(PSObject psObject)
    {
        return psObject.Properties.ToDictionary(p => p.Name, p => p.Value?.ToString() ?? string.Empty);
    }
}