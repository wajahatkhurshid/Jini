<#	
	.NOTES
	===========================================================================
	 Created on:   	10/13/2015 10:44 AM
	 Created by:   	Mudasar.Ellahi
	 Organization: 	Synergy-IT
	 Filename:     	DeleteBackup_Site.ps1
	===========================================================================
	.DESCRIPTION
		Delete Temp Backup of Website.
#>


param (
	[string]$siteName = $(throw "-siteName is required.")
)

$_TempBackupFilePath = "_" + $siteName + "_backup.tmp"

#Check if backup folder path is valid available
if (!(Test-Path $_TempBackupFilePath))
{
	throw "Couldn't find file $PSScriptRoot\$_TempBackupFilePath which contains backup folder path. It should be present to execute script"
}

$BackupFolderPath = Get-Content -Path $_TempBackupFilePath


if (!(Test-Path $BackupFolderPath))
{
	throw "$BackupFolderPath doesn't exists."
}

try
{
    # Remove Backup folder
    Remove-Item -Path $BackupFolderPath -Force -Recurse
    # Remove temp file which contains backup folder path
    Remove-Item -Path $_TempBackupFilePath
    Write-Host "Backup Removed Successfully"
}
catch [Exception]
{
	$host.ui.WriteErrorLine("Unable to Delete Backup of site: $siteName")
	$host.ui.WriteErrorLine($_.Exception.Message)
	$result = $false
}