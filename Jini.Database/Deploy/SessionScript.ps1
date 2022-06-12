Configuration SqlDatabaseDeploy
{	
	Import-DscResource -Name cScriptWithParams
   
    Node $env:COMPUTERNAME
    {
        Write-Verbose "InitialCatalog = $InitialCatalog"
        Write-Verbose "DacPacFile = $DacPacFile"        
        Write-Verbose "SqlPackagePath = $SqlPackagePath"        
        
		# Ensure the CScriptWithParams is present in C:\Program Files\WindowsPowerShell\Modules
		
		
		# Ensure Prereqs for Sql server are installed

		
		# Ensure that other necessary features are installed


		# Set-up gyldendal event log

		
		# Log whats goin on (eventlog)
		
		
         cScriptWithParams DeployDatabase
		{
			GetScript = { @{ Name = "DeployDatabase" } }
            TestScript = { $false }
			SetScript = {
                Write-Verbose "Installation for database = [$InitialCatalog]"
				#$output = & "$SqlPackagePath" /a:Publish  /sf:$rootdir\$DacPacFile /TargetServerName:localhost /tdn:$InitialCatalog 2>&1
                $stdErrLog = "$rootdir\stderr.log"
                $stdOutLog = "$rootdir\stdout.log"
                $dacPacFilePath = "$rootdir$DacPacFile"
                $arguments = [string[]]@(
	                "/a:Publish",
	                "/sf:$dacPacFilePath",
	                "/TargetServerName:localhost",
	                "/tdn:$InitialCatalog"
                    )
	            Start-Process $SqlPackagePath -ArgumentList $arguments -RedirectStandardOutput $stdOutLog -RedirectStandardError $stdErrLog -NoNewWindow -Wait
                Write-Verbose "-------Sql Package Installation Started-------"
                $output = Get-Content $stdErrLog, $stdOutLog
                Foreach ($i in $output)
                {
                    Write-Verbose $i
                }
                Write-Verbose "-------Sql Package Installation Ended-------"
			}

			 cParams =
            @{ 
				InitialCatalog = $InitialCatalog
				DacPacFile = $DacPacFile
				SqlPackagePath = $SqlPackagePath
				rootDir = $PackageDir
            }				
					
		}
    }
}
 
# command for RM
SqlDatabaseDeploy -Verbose
#Start-DscConfiguration -Path .\SqlDatabaseDeploy -Verbose -Wait -Force

