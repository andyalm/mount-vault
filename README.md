# Mount Vault

An experimental powershell provider that allows you to explore and update vault secrets as a virtual filesystem.

## Installation

```powershell
Install-Module MountVault

# Consider adding the following lines to your profile:
Import-Module MountVault

# Add a PSDrive for every instance of vault you wish to mount. Add this to your profile so that it will always be present
New-PSDrive -Name vault -PSProvider MountVault -Root '' -VaultAddress 127.0.0.1:8500
```

## Authentication

Vault supports a few different authentication mechanisms. Right now, the act of authenticating with vault
is outside the scope of `MountVault`. It expects a `VAULT_TOKEN` environment variable to be set in the session of the powershell instance
its running in. If you would like to change the name of the environment variable that its looking for, you can pass the `-VaultTokenEnvironmentVariable` option on the `New-PSDrive` command.

## Usage

Once you have mounted a PSDrive for your vault instance, you can navigate it.
You can run `dir` within any directory to list the objects within. This essentially provides a self-documenting
way of navigating. Tab completion works as well.

The virtual filesystem is constructed like this (values surrounded by <> are dynamic):

```
 -- secret
    |-- <folder>
        |-- <secret>
        |-- <secret>
    |-- <folder>
        |-- <folder>
            |-- <secret>
                ...
```

### Supported commands

In addition to the obvious (Get-Item and Get-ChildItem), the following commands are currently supported by the `MountVault` provider:

#### Get-ItemProperty

Allows you to get the value(s) of a secret. Example usage:

```
vault:/secret/folder> Get-ItemProperty mysecret

Name                           Value
----                           -----
value1                         mysecretvalue1
value2                         mysecretvalue2
```

#### Set-ItemProperty

Allows you to set a specific value of a secret. Example usage:

```
vault:/secret/folder> Set-ItemProperty mysecret -Name value2 -Value myupdatedvalue

Name                           Value
----                           -----
value1                         mysecretvalue1
value2                         myupdatedvalue
```

#### Clear-ItemProperty

Allows you to remove a specific value of a secret. Example usage:

```
vault:/secret/folder> Clear-ItemProperty mysecret -Name value2

Name                           Value
----                           -----
value1                         mysecretvalue1
```

#### New-Item

Allows you to create a new secret at the given path. Example usage:

```powershell
vault:/secret/folder> New-Item mysecret -Value @{mysecretkey='mysecretvalue'}
```
#### Remove-Item

Allows you to delete a secret at the given path. Example usage:

```powershell
vault:/secret/folder> Remove-Item mysecret
```