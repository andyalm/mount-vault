using System.Management.Automation;
using MountAnything;
using MountVault.Authentication;

namespace MountVault;

public class VaultDriveHandler : IDriveHandler, INewDriveParameters<VaultDriveParameters>
{
    public PSDriveInfo NewDrive(PSDriveInfo driveInfo)
    {
        var vaultDrive = new VaultDriveInfo(driveInfo, NewDriveParameters);
        
        AuthenticationFactory.GetAuthenticator(vaultDrive).Validate(vaultDrive);

        return vaultDrive;
    }

    public VaultDriveParameters NewDriveParameters { get; set; } = null!;
}

public class VaultDriveInfo : PSDriveInfo
{
    public VaultDriveInfo(PSDriveInfo driveInfo, VaultDriveParameters parameters) : base(driveInfo)
    {
        var vaultUri = parameters.VaultAddress.StartsWith("https://")
            ? parameters.VaultAddress
            : $"https://{parameters.VaultAddress}";
        VaultAddress = new Uri(vaultUri);
        VaultTokenEnvironmentVariable = parameters.VaultTokenEnvironmentVariable;
        AuthType = parameters.AuthType;
    }

    public Uri VaultAddress { get; }
    public VaultAuthType AuthType { get; }
    public string VaultTokenEnvironmentVariable { get; }
    public AuthResult? CachedAuthResult { get; set; }
}

public enum VaultAuthType
{
    TokenEnv,
    Ldap,
}

public class VaultDriveParameters
{
    [Parameter(Mandatory = true)]
    public string VaultAddress { get; set; } = null!;

    [Parameter]
    public VaultAuthType AuthType { get; set; } = VaultAuthType.TokenEnv;

    [Parameter] 
    public string VaultTokenEnvironmentVariable { get; set; } = "VAULT_TOKEN";
}