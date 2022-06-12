Import-Module WebAdministration

# AppCmd is required for IIS Configuration backup. Verify that it is installed on machine.
$AppCmd = "$env:windir\system32\inetsrv\appcmd.exe"
if (!(Test-Path $AppCmd))
{
	throw "Could not find AppCmd. Install IIS and its Management Tools."
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

# Get IIS Site
function Get-IISSite([string]$siteName)
{
	#Get-WebApplication
	foreach ($webapp in get-childitem IIS:\Sites)
	{
		$name = $webapp.name
		if ($name -eq $siteName)
		{
			return $webapp
		}
	}
}

# Detect if the Site is installed 
function Get-IISSiteExists([string]$AppSiteName)
{
	Import-Module WebAdministration
	#Get-WebApplication
	
	foreach ($webapp in get-childitem IIS:\Sites)
	{
		$name = $webapp.name
		if ($name -eq $AppSiteName)
		{
			$retVal = "" | Select-Object -Property Valid, Path
			$retVal.Valid = $true
			$retVal.Path = $webapp.physicalPath
			return $retVal
		}
	}
}

function CreateSite([string]$siteName, [string]$physicalPath, [string]$appPoolName, [string]$bindingHostName, [string]$bindingPort, [string]$bindingProtocol, [string]$CertSubject,  [bool]$enableIntegratedSecurity)
{
	$result = $false
	try
	{
		IF ([string]::IsNullOrEmpty($physicalPath))
		{
			$host.ui.WriteErrorLine("To create new website -physicalPath is required")
			return $result
		}
		
		# Add App Pool with the user for integrated security
		$pool = New-Item ("IIS:\AppPools\" + $appPoolName) -Force
		$pool.managedRuntimeVersion = "v4.0"
		
		#allows the site to be created if no identity user is provided
		IF ($enableIntegratedSecurity -eq $true)
		{
			$cred = $null
			try{
				$cred = Get-Credential
			}
			catch
			{
			}

			IF ($cred -ne $null)
			{
				$pool.processModel.identityType = 3
				$pool.processModel.userName = $cred.UserName
				$pool.processModel.password = $cred.GetNetworkCredential().password
			}else {
				Write-Warning "You didn't provided service account for Integrated Security, You will have to set it manually in IIS."
			}
		}
		
		$pool | set-item
		
		Write-Host "App pool created"
		
        $websitePhyicalPath = $($physicalPath)

		# Add Website
		#.$AppCmd add site /name:$siteName /physicalPath:$websitePhyicalPath
        .$AppCmd add site /name:$siteName /physicalPath:$($websitePhyicalPath)

		Write-Host "website created"

		#Associate pool with the website
		.$AppCmd set site $siteName /applicationDefaults.applicationPool:$appPoolName
		Write-Host "App Pool associated"
		
		#Add the bindings
		$NewWebBinding = New-WebBinding -Name $bindingHostName -IPAddress "*" -Port $bindingPort -HostHeader $bindingHostName -Protocol $bindingProtocol
		Write-Host 'Binding Added'
		#for https add certificate
		if ($bindingProtocol -eq "https")
		{
			# Getting Certificate
			$cert = gci 'CERT:\LocalMachine\My' | Where-Object { $_.subject -ilike "*$CertSubject*" }
			$thumbprint = $cert.Thumbprint.ToString()
			Write-Host 'Certificate Thumbprint: ' $thumbprint
			#Add Certificate
			$AddSSLCertToWebBinding = (Get-WebBinding $sitename -Port $bindingPort -Protocol $bindingProtocol).AddSslCertificate($thumbprint, "MY")
			Write-Host 'Certificate assigned to website'
		}
		$result = $true
	}
	catch [Exception]
	{
		$host.ui.WriteErrorLine('Exception in CreateSite')
		$host.ui.WriteErrorLine($_.Exception.Message)
		$result = $false
	}
	return $result
}

function Update-IISSite([string]$AppSiteName, [bool]$enableIntegratedSecurity)
{
	#if force parameter is true then update Site 
	IF ($enableIntegratedSecurity -eq $true)
	{
		$site = Get-IISSite $AppSiteName
		
		# Get Service Account from User for AppPool Identity. 
		$cred = $null
		try{
			$cred = Get-Credential
		}
		catch
		{
		}

		IF ($cred -ne $null)
		{
			$pool = get-item ("IIS:\AppPools\" + $site.applicationPool)
			$pool.processModel.identityType = 3
			$pool.processModel.userName = $cred.UserName
			$pool.processModel.password = $cred.GetNetworkCredential().password
			
			$pool | set-item
		}
		else {
			Write-Warning "You didn't provided service account for Integrated Security, You will have to set it manually in IIS."
		}
	}
}

function BackupIISConfiguration([string]$siteName, [string]$IISConfigBackupPath)
{
    if (-not ($IISConfigBackupPath.EndsWith('\')))
	{
		$IISConfigBackupPath = $IISConfigBackupPath + "\"
	}

    $site = Get-IISSite $siteName

	$PoolConfigBackupFile = ($siteName + "_IISPool_Configuration.xml")
	$SiteConfigBackupFile = ($siteName + "_IISSite_Configuration.xml")

	#$AuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_Authentication.xml")
	#$WindowsAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_WindowsAuthentication.xml")
	#$DigestAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_DigestAuthentication.xml")
	#$BasicAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_BasicAuthentication.xml")
	#$AnonymousAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_AnonymousAuthentication.xml")

	$PoolConfigBackupFilePath = $IISConfigBackupPath + $PoolConfigBackupFile
	$SiteConfigBackupFilePath = $IISConfigBackupPath + $SiteConfigBackupFile
    
    

	if (-not (Test-Path $IISConfigBackupPath))
	{
		New-Item $IISConfigBackupPath -type directory
	}

	.$AppCmd list apppool $site.applicationPool /config /xml > $PoolConfigBackupFilePath
	.$AppCmd list site $siteName /config /xml > $SiteConfigBackupFilePath

	#.$AppCmd list config $siteName -section:system.web/authentication /clr:4 /config /xml > $AuthConfigBackupFilePath
	#.$AppCmd list config $siteName -section:system.webServer/security/authentication/windowsAuthentication /clr:4 /config /xml > $WindowsAuthConfigBackupFilePath
	#.$AppCmd list config $siteName -section:system.webServer/security/authentication/digestAuthentication /clr:4 /config /xml > $DigestAuthConfigBackupFilePath
	#.$AppCmd list config $siteName -section:system.webServer/security/authentication/basicAuthentication /clr:4 /config /xml > $BasicAuthConfigBackupFilePath
	#.$AppCmd list config $siteName -section:system.webServer/security/authentication/anonymousAuthentication /clr:4 /config /xml > $AnonymousAuthConfigBackupFilePath

}

function RestoreIISConfiguration([string]$siteName, [string]$IISConfigBackupPath)
{
    if (-not ($IISConfigBackupPath.EndsWith('\')))
	{
		$IISConfigBackupPath = $IISConfigBackupPath + "\"
	}

	$site = Get-IISSite $siteName

	#$AuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_Authentication.xml")
	#$WindowsAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_WindowsAuthentication.xml")
	#$DigestAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_DigestAuthentication.xml")
	#$BasicAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_BasicAuthentication.xml")
	#$AnonymousAuthConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_AnonymousAuthentication.xml")

	$PoolConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_IISPool_Configuration.xml")
	$SiteConfigBackupFilePath = $IISConfigBackupPath + ($siteName + "_IISSite_Configuration.xml")

	if (-not (Test-Path $IISConfigBackupPath))
	{
		Write-Warning "$IISConfigBackupPath Doesn't Exists."
		return $false
	}

	if (!(Test-Path $PoolConfigBackupFilePath))
	{
		Write-Warning "Pool Configugration file isn't available."
	}
	else
	{
		if ($site)
		{
			Write-Host "Deleting OLD IIS AppPool of $siteName"
			.$AppCmd delete apppool $site.applicationPool
		}
		type $PoolConfigBackupFilePath | .$AppCmd add apppool /in
		Write-Host "Pool Configuration is restored."
	}

	if (!(Test-Path $SiteConfigBackupFilePath))
	{
		Write-Warning "Site Configugration file isn't available."
	}
	else
	{
		if ($site)
		{
			Write-Host "Deleting OLD IIS Site of $siteName"
			.$AppCmd delete site $siteName
		}
		type $SiteConfigBackupFilePath | .$AppCmd add site /in
		Write-Host "Site Configuration is restored."
	}
    #RestoreIISSection $siteName '/section:system.web/authentication' $AuthConfigBackupFilePath
    #RestoreIISSection $siteName '/section:system.webServer/security/authentication/windowsAuthentication' $WindowsAuthConfigBackupFilePath
    #RestoreIISSection $siteName '/section:system.webServer/security/authentication/digestAuthentication'  $DigestAuthConfigBackupFilePath
    #RestoreIISSection $siteName '/section:system.webServer/security/authentication/basicAuthentication' $BasicAuthConfigBackupFilePath
    #RestoreIISSection $siteName '/section:system.webServer/security/authentication/anonymousAuthentication' $AnonymousAuthConfigBackupFilePath

}

function RemoveWebsite([string] $siteName)
{
	$site = Get-IISSite $siteName

	if (!($site))
	{
		Write-Warning "Site Doesn't exists."
		return
	}

	$message  = 'Remove Website'
	$question = "Are you sure you want to REMOVE $siteName & its Deployed Package?"
    $question += "`n`n!!!This Action is not reversible!!!!"

	$choices = New-Object Collections.ObjectModel.Collection[Management.Automation.Host.ChoiceDescription]
	$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Delete'))
	$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&No'))

	$decision = $Host.UI.PromptForChoice($message, $question, $choices, 1)
    $path = $site.physicalPath
	if ($decision -eq 0) {
		Write-Host "Removing Deployed Package $path"
		if (Test-Path $path)
		{
			try{
				Remove-Item $path -Recurse -Force
			}catch{
				Write-Host "Some error Occured while removing $path. Remove it manually."
			}
		}
		else {
			Write-Host "Path: $path. Doesn't exists."
		}
		
		Write-Host "Removing Website $siteName"
		.$AppCmd delete site $siteName
		Write-Host "Removing App Pool of $siteName"
		.$AppCmd delete apppool $site.applicationPool
		Write-Host "Website Removed."
	} else {
	  Write-Host 'Nothing Removed.'
	}
}

function RestoreIISSection([string]$siteName, [string] $sectionPath, [string] $filePath){
    $sectionName = $sectionPath.SubString($sectionPath.LastIndexOf("/") + 1)
    if (!(Test-Path $filePath))
	{
		Write-Warning "$sectionName Configuration file isn't available."
	}
	else
	{
        $appcmdInvk = "$AppCmd unlock config $sectionPath -commit:apphost"
        Invoke-Expression $appcmdInvk | Out-Null
        $appcmdInvk = "$AppCmd 'set config $siteName $sectionPath /clr:4 /config /in < $filePath /commit:apphost'"
        Invoke-Expression $appcmdInvk | Out-Null
		Write-Host "$sectionName Configuration is restored."
	}
}


function MSDeployPackage ([string]$siteName, [string]$package, [string]$skipDelete){
    # Build Argument List for MSDeploy
	$arguments = [string[]]@(
	"-verb:sync",
	"-source:package='$package'",
	"-dest:auto",
	"-setParam:name='IIS Web Application Name',value='$($siteName)' $skipDelete")
			
	#Deploy package using MSDeploy
	Start-Process $MSDeploy -ArgumentList $arguments -NoNewWindow -Wait
}

function MSDeployPackageUsingParamFile ([string]$setParametersFile, [string]$package){
    # Build Argument List for MSDeploy
			$arguments = [string[]]@(
			"-verb:sync",
			"-source:package='$package'",
			"-dest:auto",
			"-setParamFile:$setParametersFile")
			
			#Deploy package using MSDeploy
			Start-Process $MSDeploy -ArgumentList $arguments -NoNewWindow -Wait
}