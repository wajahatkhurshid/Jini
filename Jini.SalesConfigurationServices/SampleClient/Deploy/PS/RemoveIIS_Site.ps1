<#	
	.NOTES
	===========================================================================
	 Created on:   	12/01/2015 2:52 PM
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

$IISUtil = "IISUtil.psm1"
if (!(Test-Path $ScriptPath\Modules\$IISUtil))
{
	throw 'Could not find $IISUtil in Modules Folder. Please verify that $IISUtil exists in $ScriptPath\Modules\'
}
Import-Module $ScriptPath\Modules\$IISUtil

RemoveWebsite $siteName