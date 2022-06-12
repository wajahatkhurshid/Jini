<#	
	.NOTES
	===========================================================================
	 Created on:   	9/21/2015 2:52 PM
	 Created by:   	Mudasar.Ellahi
	 Organization: 	Synergy-IT (Pakistan)
	 Filename:     	
	===========================================================================
	.DESCRIPTION
		Restore website's IIS Configuration & its deployed package.
#>

param (
	[string]$siteName = $(throw "-siteName is required.")
)

# Check if User is Running Script as Administrator
 If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`
    [Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Warning "You do not have Administrator rights to run this script!`nPlease re-run this script as an Administrator!"
    Break
}

$ScriptPath = Split-Path $MyInvocation.MyCommand.Path
$CopyFilesUtil = "CopyFilesUtil.psm1"

if (!(Test-Path $ScriptPath\Modules\$CopyFilesUtil))
{
	throw 'Could not find $CopyFilesUtil in Modules Folder. Please verify that $CopyFilesUtil exists in $ScriptPath\Modules\'
}
Import-Module $ScriptPath\Modules\$CopyFilesUtil

$IISUtil = "IISUtil.psm1"
if (!(Test-Path $ScriptPath\Modules\$IISUtil))
{
	throw 'Could not find $IISUtil in Modules Folder. Please verify that $IISUtil exists in $ScriptPath\Modules\'
}
Import-Module $ScriptPath\Modules\$IISUtil

$_TempBackupFilePath = "_" + $siteName + "_backup.tmp"

#Check if backup folder path is valid available
if (!(Test-Path $_TempBackupFilePath))
{
	throw "Couldn't find file at $PSScriptRoot\$_TempBackupFilePath which contains backup folder path. It should be present to execute script"
}

$BackupFolderPath = Get-Content -Path $_TempBackupFilePath


if (!(Test-Path $BackupFolderPath))
{
	throw "$BackupFolderPath doesn't exists."
}

function RestoreBackup()
{
	$result = $false
	try
	{
		# Init Backup Files Path
		$IISConfigBackupPath = $BackupFolderPath + "\IISConfigurations\"
        $SiteBackupFilesPath = $BackupFolderPath + "\SiteBackup\"

		if (!(Test-Path $SiteBackupFilesPath))
		{
			Write-Warning "$SiteBackupFilesPath doesn't exist! Unable to Restore files."
			return $result
		}
		if (!(Test-Path $IISConfigBackupPath))
		{
			Write-Warning "$IISConfigBackupPath doesn't exist! Unable to Restore IIS Configuration."
			return $result
		}
		
		RestoreIISConfiguration $siteName $IISConfigBackupPath
		$site = Get-IISSite $siteName

		if($site)
		{
            $SitePhysicalPath = $site.physicalPath

			if (Test-Path $SitePhysicalPath)
			{
				Get-ChildItem $SitePhysicalPath | Remove-Item -Recurse -Force
			}

			$result = CopyFilesAndFoldersWithRights $SiteBackupFilesPath $SitePhysicalPath
		}else{
			$result = $false
		}
		
		
	}
	catch [Exception]
	{
		$host.ui.WriteErrorLine("Exception in RestoreBackup")
		$host.ui.WriteErrorLine($_.Exception.Message)
		$result = $false
	}
	return $result
}

$result = RestoreBackup
if($result)
{
    Write-Host "$siteName is restored."    
}else{
    Write-Host "Unable to Restore $siteName"
}

