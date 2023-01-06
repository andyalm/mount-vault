#!/usr/bin/env pwsh -NoExit -Interactive -NoLogo -NoProfile

param(
    [Parameter(Position=0,Mandatory=$true)]
    [string]
    $VaultAddress,

    [Parameter()]
    [string]
    $Root = ''
)
$ErrorActionPreference='Stop'
$env:NO_MOUNT_VAULT='1'
dotnet build
if(-not (Get-Alias ls -ErrorAction SilentlyContinue)) {
    New-Alias ls Get-ChildItem
}
if(-not (Get-Alias cat -ErrorAction SilentlyContinue)) {
    New-Alias cat Get-Content
}
Import-Module $([IO.Path]::Combine($PWD,'src','MountVault','bin','Debug','net6.0','Module','MountVault.psd1'))

New-PSDrive -Name vault -PSProvider MountVault -Root $Root -VaultAddress $VaultAddress
cd vault:
