using System.Management.Automation;
using MountAnything;

namespace MountVault;

public class VaultDriveHandler : IDriveHandler, INewDriveParameters<VaultDriveParameters>
{
    public PSDriveInfo NewDrive(PSDriveInfo driveInfo)
    {
        var vaultAddress = NewDriveParameters.VaultAddress.StartsWith("https://")
            ? NewDriveParameters.VaultAddress
            : $"https://{NewDriveParameters.VaultAddress}";

        return new VaultDriveInfo(driveInfo, new Uri(vaultAddress), NewDriveParameters.VaultTokenEnvironmentVariable);
    }

    public VaultDriveParameters NewDriveParameters { get; set; } = null!;
}

public class VaultDriveInfo : PSDriveInfo
{
    public VaultDriveInfo(PSDriveInfo driveInfo, Uri vaultAddress, string vaultTokenEnvironmentVariable) : base(driveInfo)
    {
        VaultAddress = vaultAddress;
        VaultTokenEnvironmentVariable = vaultTokenEnvironmentVariable;
    }

    public Uri VaultAddress { get; }
    public string VaultTokenEnvironmentVariable { get; }
    
    public string GetVaultToken()
    {
        var vaultToken = Environment.GetEnvironmentVariable(VaultTokenEnvironmentVariable);
        if (string.IsNullOrEmpty(vaultToken))
        {
            throw new InvalidOperationException(
                $"The environment variable '{VaultTokenEnvironmentVariable}' is not set");
        }

        return vaultToken;
    }
}

public class VaultDriveParameters
{
    [Parameter(Mandatory = true)]
    public string VaultAddress { get; set; } = null!;

    [Parameter] 
    public string VaultTokenEnvironmentVariable { get; set; } = "VAULT_TOKEN";
}