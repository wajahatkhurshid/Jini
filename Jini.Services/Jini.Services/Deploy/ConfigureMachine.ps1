#
# ConfigureMachine.ps1
#
Configuration ConfigureMachine {

	Node $env:COMPUTERNAME
    {
        # Ensure that .NET framework features are installed
        WindowsFeature IIS
        {
            Name = "Web-server"
            IncludeAllSubFeature = $true
            Ensure = "Present"
			Source = "\\s-tfs02-v\ReleaseManagementShare\Assets\Windows 2012\sxs"
        }

		Package WebDeploy
        {
            Name = "Web Deploy 3.5"
			Path = "\\ci.gyldendal.local\Assets\webdeploy-3.5\WebDeploy_amd64_en-US.msi"        
            ProductId = "1A81DA24-AF0B-4406-970E-54400D6EC118"
            Ensure = "Present"			
            #DependsOn = "[WindowsFeature]WebServerRole"
         }


		# Ensure the CScriptWithParams is present in C:\Program Files\WindowsPowerShell\Modules
		File DSCResourceFolder {
            SourcePath = "\\ci.gyldendal.local\Assets\Deployment-scripts\DSC_ColinsALMCorner.com"
            DestinationPath = "$PSHOME\modules\"
            Recurse = $true
            Type = "Directory"
        }
	}
}

# command for RM
ConfigureMachine -Verbose
#Start-DscConfiguration -Path .\ConfigureMachine -Verbose -Wait -Force