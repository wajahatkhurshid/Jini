<#	
	.NOTES
	===========================================================================
	 Created on:   	9/21/2015 10:44 AM
	 Created by:   	Mudasar.Ellahi
	 Organization: 	Synergy-IT
	 Filename:     	BackupIIS_Site.ps1
	===========================================================================
	.DESCRIPTION
		Create Backup of website which includes IIS Configuration and deployed package.
#>

# Backup website's IIS Configuration & its deployed package

param (
	[string]$siteName = $(throw "-siteName is required."),
	[string]$backupPath = "D:\Backup"
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

function TakeBackup()
{
	$result = $false
	try
	{
		# Get Site from IIS
		$site = Get-IISSite $siteName
		if (!($site))
		{
			Write-Warning "Failed to find site $siteName. Nothing to Backup."
			return
		}
		else
		{
			Write-Host "Site: " $site.Name "Pool: " $site.applicationPool "Path: " $site.physicalPath

            $site.physicalPath
            if (-not (Test-Path $site.physicalPath))
			{
                $path = $site.physicalPath
				Write-Warning "$path doesn't exists. No BACKUP has been taken."
                return $result
			}
			
			# Set Paths for Backup
			$siteBackupFolder = $backupPath + "\" + $siteName + "_" + (Get-Date).ToString("yyyyMMddHHmmss") + "\"
			
			$IISConfigBackupPath = $siteBackupFolder + "\IISConfigurations\"
			$SiteBackupFilesPath = $siteBackupFolder + "\SiteBackup\"
			
			if (-not (Test-Path $siteBackupFolder))
			{
				New-Item $siteBackupFolder -type directory
			}
			
			if (-not (Test-Path $SiteBackupFilesPath))
			{
				New-Item $SiteBackupFilesPath -type directory
			}
			
			$_TempBackupFilePath = "_" + $siteName + "_backup.tmp"
			New-Item -Name $_TempBackupFilePath -Value $siteBackupFolder -ItemType file -force
			
			BackupIISConfiguration $siteName $IISConfigBackupPath
            Write-Host "Backup of IIS Configuration has been created."

			CopyFilesAndFoldersWithRights $site.physicalPath $SiteBackupFilesPath
            Write-Host "Backup of Deployed Package has been created."
		}
		
		$result = $true
	}
	catch [Exception]
	{
		$host.ui.WriteErrorLine("Exception in TakeBackup")
		$host.ui.WriteErrorLine($_.Exception.Message)
		$result = $false
	}
	return $result
}

# Take Backup
$result = TakeBackup

# Output Result
if ($result -eq $true)
{
	Write-Host "$siteName has been backed up successfully."
}
else
{
	Write-Host "Unable to create backup of $siteName"
}