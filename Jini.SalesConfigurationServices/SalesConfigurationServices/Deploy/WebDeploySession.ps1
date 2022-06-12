Configuration Site_WebDeploy
{	
	Import-DscResource -Name cScriptWithParams
   
    Node $env:COMPUTERNAME
    {
        Write-Verbose "Params = $Params"
        Write-Verbose "ObjDelimiter = $ObjDelimiter"
        Write-Verbose "KeyDelimiter = $KeyDelimiter"
        Write-Verbose "PackageName = $PackageName"
        Write-Verbose "SiteName = $SiteName"
        Write-Verbose "PhysicalPath = $PhysicalPath"
        Write-Verbose "BindingProtocol = $BindingProtocol"
        Write-Verbose "BindingPort = $BindingPort"
        Write-Verbose "BindingHostName = $BindingHostName"
        Write-Verbose "PackageDir = $PackageDir"
		
        # Ensure that .NET framework features are installed
        WindowsFeature IIS
        {
            Name = "Web-server"
            IncludeAllSubFeature = $true
            Ensure = "Present"
			Source = "\\s-tfs02-v\ReleaseManagementShare\Assets\Windows 2012\sxs"
        }

        Log WebServerLog
        {
          Message = "Starting WebServer node configuration."
        }

        cScriptWithParams SetDeploymentParameters
        {
            GetScript =
            {
                @{ Name = "SetDeploymentParameters" }
            }
            SetScript =
            {
				$ParameterFilePath = $rootDir+"\"+$ParameterFile
                Write-Verbose "Updating ParemeterFile = [$ParameterFilePath]"
                $content = gc $ParameterFilePath
				Write-Verbose "$content"
				$option = [System.StringSplitOptions]::RemoveEmptyEntries
                ($Params.Split($ObjDelimiter, $option) | foreach {
                $keyValue = $_.Split($KeyDelimiter, $option)
				if($keyValue[1] -eq "''"){
					$keyValue[1] = ""
				}
                Write-Verbose "$keyValue"
                $content = $content.Replace($keyValue[0], $keyValue[1])
                } )
				sc -Path $ParameterFilePath -Value $content
            }
            TestScript =
            {
                $false
            }
            cParams =
            @{ 
                ParameterFile = $ParametersFile;
                Params = $Params;
                ObjDelimiter = $ObjDelimiter;
                KeyDelimiter = $KeyDelimiter;
				rootDir = $PackageDir;
            }
        }

         
        cScriptWithParams DeploySite
        {
            GetScript = { @{ Name = "DeploySite" } }
            TestScript = { $false }
            SetScript = {
                Write-Verbose "Installation Package for = [$SiteName]"
				Write-Verbose "Parameters: $rootDir\PS\PublishIIS_Site.ps1 -PackageDir $rootDir -PackageName $PackageName -SiteName $SiteName -PhysicalPath $PhysicalPath  -BindingProtocol $BindingProtocol -BindingPort $BindingPort -BindingHostName $BindingHostName -setParameterFileName $ParameterFile"
                powershell "$rootDir\PS\PublishIIS_Site.ps1" -PackageDir $rootDir -PackageName $PackageName -SiteName $SiteName -PhysicalPath $PhysicalPath  -BindingProtocol $BindingProtocol -BindingPort $BindingPort -BindingHostName $BindingHostName -setParameterFileName $ParameterFile
            }
            cParams =
            @{ 
                ParameterFile = $ParametersFile;
                PackageName = $PackageName;
                SiteName = $SiteName;
                PhysicalPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PhysicalPath);
				BindingProtocol = $BindingProtocol;
                BindingPort = $BindingPort;
                BindingHostName = $BindingHostName;
                rootDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PackageDir);
            }

         DependsOn = "[cScriptWithParams]SetDeploymentParameters"
        }
        
    }
}
 
# command for RM
Site_WebDeploy -Verbose
#Start-DscConfiguration -Path .\Site_WebDeploy -Verbose -Wait -Force

