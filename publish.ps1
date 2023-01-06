$ErrorActionPreference='Stop'

dotnet publish src/MountVault
Get-ChildItem -Recurse ./bin/MountVault | Select-Object Name
Publish-Module -Path ./bin/MountVault -NuGetApiKey $env:NuGetApiKey
