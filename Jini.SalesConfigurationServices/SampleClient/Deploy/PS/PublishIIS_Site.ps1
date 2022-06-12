<#	
	.NOTES
	===========================================================================
	 Created on:   	9/21/2015 10:44 AM
	 Created by:   	Mudasar.Ellahi
	 Organization: 	Synergy-IT
	 Filename:     	PublishIIS_Site.ps1
	===========================================================================
	.DESCRIPTION
		Create Backup of website which includes IIS Configuration and deployed package.
#>

param (
	[string]$siteName = $(throw "-siteName is required."),
	[string]$physicalPath,
	[string]$bindingProtocol,
	[string]$bindingHostName,
	[string]$bindingPort,
	[string]$packageName,
	[string]$setParameterFileName,
	[string]$packageDir,
	[string]$CertSubject = "CN=*.gyldendal.dk",
	[switch]$enableIntegratedSecurity = $false,
    [switch]$skipDeleteFiles,
	[switch]$force,
    [switch]$backupMandatory = $false
)

# Check if User is Running Script as Administrator
If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`
[Security.Principal.WindowsBuiltInRole] "Administrator"))
{
	Write-Warning "You do not have Administrator rights to run this script!`nPlease re-run this script as an Administrator!"
	Break
}

$ScriptPath = Split-Path $MyInvocation.MyCommand.Path

$IISUtil = "IISUtil.psm1"
if (!(Test-Path $ScriptPath\Modules\$IISUtil))
{
	throw 'Could not find $IISUtil in Modules Folder. Please verify that $IISUtil exists in $ScriptPath\Modules\'
}
Import-Module $ScriptPath\Modules\$IISUtil

$CopyFilesUtil = "CopyFilesUtil.psm1"

if (!(Test-Path $ScriptPath\Modules\$CopyFilesUtil))
{
	throw 'Could not find $CopyFilesUtil in Modules Folder. Please verify that $CopyFilesUtil exists in $ScriptPath\Modules\'
}
Import-Module $ScriptPath\Modules\$CopyFilesUtil

#Declare skip deletion of files variable
$skipDelete = ''
if($skipDeleteFiles){
	$skipDelete = '-enableRule:DoNotDeleteRule'
}

# Get Installation Path of MSDeploy from registry
$MSDeployKey = 'HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy\3'
if (!(Test-Path $MSDeployKey))
{
	throw "Could not find MSDeploy. Install MS Deploy and re-run this file"
}
$InstallPath = (Get-ItemProperty $MSDeployKey).InstallPath
if (!$InstallPath -or !(Test-Path $InstallPath))
{
	throw "Could not find MSDeploy. Install MS Deploy and re-run this file"
}
#Grab the path to MSDeploy.exe
$MSDeploy = Join-Path $InstallPath "msdeploy.exe"
if (!(Test-Path $MSDeploy))
{
	throw "Could not find MSDeploy. Install MS Deploy and re-run this file"
}

#Treating All Errors as Terminating
$ErrorActionPreference = 'Stop'
#Name the app pool the same as the site
[string]$appPoolName = $siteName
$blnContinue = $false


#Resolve the relative path to the package directory.
$fullPackagePath = Resolve-Path $packageDir

#Append Full path to the .zip package file
$package = Join-Path $fullPackagePath $packageName

#Check if Package path is valid
if (!(Test-Path $package))
{
	throw "Could not find $package Please Provide valid package file."
}

function TestBackupPath(){

    if($backupMandatory) 
    {
        $_TempBackupFilePath = "_" + $siteName + "_backup.tmp"
        #Check if backup folder path is valid available
        if (!(Test-Path $_TempBackupFilePath))
        {
	        throw "Couldn't find file at $PSScriptRoot\$_TempBackupFilePath which contains backup folder path. It should be present to execute script. Create Backup First."
        }

        $BackupFolderPath = Get-Content -Path $_TempBackupFilePath
        if (!(Test-Path $BackupFolderPath))
        {
	        throw "$BackupFolderPath doesn't exists."
        }
        return $BackupFolderPath
    }
}

try
{
    $isNewSite = $false
	# Check if Site already Exists in IIS
	$siteExists = Get-IISSiteExists($siteName)
	if (-not $siteExists.Valid)
	{
        $isNewSite = $true
		Write-Host 'Creating Site: $siteName and its pool'
		$blnContinue = CreateSite $siteName $physicalPath $appPoolName $bindingHostName $bindingPort $bindingProtocol $CertSubject  $enableIntegratedSecurity
	}
	else
	{
		Write-Host "The Site: $siteName already exists"
        #Check Backup Path
        #$BackupFolderPath = TestBackupPath
		$blnContinue = $true
	}

	if ($blnContinue -eq $true)
	{
        if ($force -eq $true -and $isNewSite -eq $false) {
		    Update-IISSite $siteName $enableIntegratedSecurity
        }
		Write-Host "Installing package" $package
		
		#Check if paramters file is entered
		if ($setParameterFileName.Length -eq 0)
		{
            MSDeployPackage $siteName $package $skipDelete
		}
		else
		{
			#Append Full path to the .xml parameter file
			$setParametersFile = Join-Path $fullPackagePath $setParameterFileName
			Write-Host "Using Parameters File" $setParametersFile
            MSDeployPackageUsingParamFile $setParametersFile $package
		}

        write-host "`n"
		Write-Host "Package: $package of Site: $siteName is successfully deployed."
		write-host "`n"
        
        
		IF ([string]::IsNullOrEmpty($BackupFolderPath))
        {
            return $true
        }
		$site = Get-IISSite $siteName
		$sitePhysicalPath = $site.physicalPath

        $BackupFolderPath = $BackupFolderPath + "\SiteBackup\"
		# Copy Rights from Backup folder to new Deployed folder.
        CopyRightsOnFilesAndFolders $BackupFolderPath $sitePhysicalPath
	}
	else
	{
		$host.ui.WriteErrorLine("Error: Deployment aborted!")
		write-host "`n"
	}
}
catch [Exception]
{
	$host.ui.WriteErrorLine("Exception in Deployment")
	$host.ui.WriteErrorLine($_.Exception.Message)
}

#To-do Write an entry to the registry of the host machine: Application name + version + install date
