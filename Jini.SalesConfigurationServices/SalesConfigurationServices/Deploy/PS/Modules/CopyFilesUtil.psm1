#Prevent Confirmation on each Write-Debug command when using -Debug
[string]$Account = 'Builtin\Administrators'
If ($PSBoundParameters['Debug']) {
    $DebugPreference = 'Continue'
}
Try {
    [void][TokenAdjuster]
} 
Catch {
    $AdjustTokenPrivileges = @"
    using System;
    using System.Runtime.InteropServices;

        public class TokenAdjuster
        {
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
        phtok);
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name,
        ref long pluid);
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
        public int Count;
        public long Luid;
        public int Attr;
        }
        internal const int SE_PRIVILEGE_DISABLED = 0x00000000;
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public static bool AddPrivilege(string privilege)
        {
        try
        {
        bool retVal;
        TokPriv1Luid tp;
        IntPtr hproc = GetCurrentProcess();
        IntPtr htok = IntPtr.Zero;
        retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
        tp.Count = 1;
        tp.Luid = 0;
        tp.Attr = SE_PRIVILEGE_ENABLED;
        retVal = LookupPrivilegeValue(null, privilege, ref tp.Luid);
        retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        return retVal;
        }
        catch (Exception ex)
        {
        throw ex;
        }
        }
        public static bool RemovePrivilege(string privilege)
        {
        try
        {
        bool retVal;
        TokPriv1Luid tp;
        IntPtr hproc = GetCurrentProcess();
        IntPtr htok = IntPtr.Zero;
        retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
        tp.Count = 1;
        tp.Luid = 0;
        tp.Attr = SE_PRIVILEGE_DISABLED;
        retVal = LookupPrivilegeValue(null, privilege, ref tp.Luid);
        retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        return retVal;
        }
        catch (Exception ex)
        {
        throw ex;
        }
        }
        }
"@
    Add-Type $AdjustTokenPrivileges
}

#Activate necessary admin privileges to make changes without NTFS perms
[void][TokenAdjuster]::AddPrivilege("SeRestorePrivilege") #Necessary to set Owner Permissions
[void][TokenAdjuster]::AddPrivilege("SeBackupPrivilege") #Necessary to bypass Traverse Checking
[void][TokenAdjuster]::AddPrivilege("SeTakeOwnershipPrivilege") #Necessary to override FilePermissions

function RemovePrivilegeFromProcess {
	#Remove priviledges that had been granted
    [void][TokenAdjuster]::RemovePrivilege("SeRestorePrivilege") 
    [void][TokenAdjuster]::RemovePrivilege("SeBackupPrivilege") 
    [void][TokenAdjuster]::RemovePrivilege("SeTakeOwnershipPrivilege")    
}

Function CopyFilesAndFoldersWithRights {
    [cmdletbinding(
        SupportsShouldProcess = $True
    )]
    Param (
        [string]$SourcePath = $(throw "-SourcePath is required."),
        [string]$DestinationPath = $(throw "-DestinationPath is required.")
    )
	$status = $false
	# Remove all Invalid SIDs from Folder & sub folers/files
	Remove-OSCSID  -Path $SourcePath  -recurse
	Remove-OSCSID  -Path $DestinationPath  -recurse
    $status = CopyFilesAndFoldersWithAllAttributes $SourcePath $DestinationPath
	RemovePrivilegeFromProcess
	return $status
}

function CopyFilesAndFoldersWithAllAttributes([string]$srcpath, [string]$dstpath)
{
	$dstpath = $dstpath + "\"
	$srcpath = $srcpath + "\"
    $srcpath = $srcpath -replace [regex]::Escape('\\'), '\'
    $dstpath = $dstpath -replace [regex]::Escape('\\'), '\'
	Try {
		$files = gci $srcpath -recurse
		$srcDirInfo = $null
		$dstDirInfo = $null
		if ((Test-Path $dstpath) -ne $true)
		{
			New-Item $dstpath -type directory | Out-Null
		}
		$srcDirInfo = [io.DirectoryInfo]($srcpath)
		$dstDirInfo = [io.DirectoryInfo]($dstpath)

		$srcDirACL = (Get-Item $srcpath).GetAccessControl('Access')
		set-acl -path $dstpath -AclObject $srcDirACL
    
	    foreach ($srcfile in $files)
	    {
		    $srcFilePath = $srcfile.FullName
		    $destFilePath = $srcFilePath -replace [regex]::Escape($srcpath), $dstpath
		
		    Copy-Item -Path $srcFilePath -Destination $destFilePath -Force
		
		    $dstfile = $null
		    # Check if destination is file or folder
		    if ((Get-Item $destFilePath) -is [System.IO.DirectoryInfo])
		    {
			    $dstfile = [io.DirectoryInfo]($destFilePath)
		    }
		    else
		    {
			    $dstfile = [io.FileInfo]($destFilePath)
		    }
		
		    # Make sure file was copied and exists before copying over properties/attributes
		    if (Test-Path $destFilePath)
		    {
			    # Get Access rights of Source File/Folder and Apply them on Destination File/folder.
                $srcACL = (Get-Item $srcFilePath).GetAccessControl('Access')
			    set-acl -path $destFilePath -AclObject $srcACL
		    }
	    }
    } 
    Catch {
                Write-Warning "$($_.Exception.Message)"
                return $false
    }
    return $true
}

function PresistFilesAndFolders([string]$srcpath, [string]$dstpath){
    $dstpath = $dstpath + "\"
	$srcpath = $srcpath + "\"
    $srcpath = $srcpath -replace [regex]::Escape('\\'), '\'
    $dstpath = $dstpath -replace [regex]::Escape('\\'), '\'
	$status = $false
	IF ([string]::IsNullOrEmpty($srcpath) -or [string]::IsNullOrEmpty($dstpath))
    {
        $status = $false
    } else {
		$files = gci $srcpath -recurse
		foreach ($srcfile in $files)
		{
			$srcFilePath = $srcfile.FullName
			$destFilePath = $srcFilePath -replace [regex]::Escape($srcpath), $dstpath
			if (-not (Test-Path $destFilePath))
			{
				Try{
					Copy-Item -Path $srcFilePath -Destination $destFilePath -Force
				}catch [Exception]
				{
					$host.ui.WriteErrorLine("Exception in PresistFilesAndFolders")
					$host.ui.WriteErrorLine($_.Exception.Message)
					$status = $false
					break
				}
			}
		}
		$status = $true
	}
	RemovePrivilegeFromProcess
	return $status
}

function CleanupFilesAndFolders([string]$srcpath, [string]$dstpath){
    $dstpath = $dstpath + "\"
	$srcpath = $srcpath + "\"
    $srcpath = $srcpath -replace [regex]::Escape('\\'), '\'
    $dstpath = $dstpath -replace [regex]::Escape('\\'), '\'
	$status = $false
	IF ([string]::IsNullOrEmpty($srcpath) -or [string]::IsNullOrEmpty($dstpath))
    {
        $status = $false
    } else {
		$files = gci $srcpath -recurse
		foreach ($srcfile in $files)
		{
			$srcFilePath = $srcfile.FullName
			$destFilePath = $srcFilePath -replace [regex]::Escape($srcpath), $dstpath
			if (-not (Test-Path $destFilePath))
			{
				if ((Test-Path $srcFilePath))
				{
					Try{
						if ((Get-Item $srcFilePath) -is [System.IO.DirectoryInfo])
						{
							Remove-Item -Path $srcFilePath -Force -Recurse
						}else {
							Remove-Item -Path $srcFilePath -Force
						}
					}catch [Exception]
					{
						$host.ui.WriteErrorLine("Exception in CleanupFilesAndFolders")
						$host.ui.WriteErrorLine($_.Exception.Message)
						$status = $false
						break
					}
				}
			}
		}
		$status = $true
	}
	RemovePrivilegeFromProcess
	return $status
}


function CopyRightsOnFilesAndFolders([string]$srcpath, [string]$dstpath)
{
    
	$status = $false
	IF ([string]::IsNullOrEmpty($srcpath) -or [string]::IsNullOrEmpty($dstpath))
    {
        $status = $false
    }else {
        $dstpath = $dstpath + "\"
	    $srcpath = $srcpath + "\"
        $srcpath = $srcpath -replace [regex]::Escape('\\'), '\'
        $dstpath = $dstpath -replace [regex]::Escape('\\'), '\'
		$srcDirInfo = $null
		$dstDirInfo = $null
		$srcDirInfo = [io.DirectoryInfo]($srcpath)
		$dstDirInfo = [io.DirectoryInfo]($dstpath)

		$dstDirInfo.Attributes = $srcDirInfo.Attributes
		$dstDirInfo.SetAccessControl($srcDirInfo.GetAccessControl())
		$srcDirACL = (Get-Item $srcpath).GetAccessControl('Access')
		set-acl -path $dstpath -AclObject $srcDirACL

		$files = gci $srcpath -recurse
	
		foreach ($srcfile in $files)
		{
			$srcFilePath = $srcfile.FullName
			$destFilePath = $srcFilePath -replace [regex]::Escape($srcpath), $dstpath
        
			# Make sure Target file exists.
			if(Test-Path $destFilePath)
			{
				$dstfile = $null
				$srcfile = $null
				# Check if destination is file or folder
				if ((Get-Item $destFilePath) -is [System.IO.DirectoryInfo])
				{
					$dstfile = [io.DirectoryInfo]($destFilePath)
					$srcfile = [io.DirectoryInfo]($srcFilePath)
				}
				else
				{
					$dstfile = [io.FileInfo]($destFilePath)
					$srcfile = [io.FileInfo]($srcFilePath)
				}
		    
					$dstfile.Attributes = $srcfile.Attributes
					$dstfile.SetAccessControl($srcfile.GetAccessControl())
					$srcACL = (Get-Item $srcFilePath).GetAccessControl('Access')
					set-acl -path $destFilePath -AclObject $srcACL
			}
		}
		$status = $true
	}
	RemovePrivilegeFromProcess
	return $status
}


Function Remove-OSCSID
{   
	[CmdletBinding()]
	Param
	(
		#Define parameters
		[Parameter(Mandatory=$true,Position=1)]
		[String]$Path,		
		[Parameter(Mandatory=$false,Position=2)]
		[Switch]$Recurse
	)
	
		if(Test-Path $Path)
		{ 
            #Try to get the object ,and define a flag to record the count of orphaned SIDs 
            Try
	        {
			    $count = 0
			    #If the object is a folder and the "-recurse" is chosen ,then get all of the folder childitem,meanwhile store the path into an array folders
			    if ($Recurse) 
	    	    {		
				    $PSPath =$path
				    DeleteSID($PSPath)
		 		    $folders = Get-ChildItem -Path $path -Recurse 
		 		    #For-Each loop to get the ACL in the folders and check the orphaned SIDs 
		 		    ForEach ($folder in $folders)
				    {
		   			    $PSPath = $folder.fullname 
		   			    DeleteSID($PSPath)	
				    }
			    }
			    else
			    #The object is a file or the "-recurse" is not chosen,check the orphaned SIDs .
			    {
				    $PSPath =$path
				    DeleteSID($PSPath)	
			    }	
		    }
            catch
	        {
	 	        Write-Error $Error
	        }
        }	
}

Function DeleteSID([string]$path)
{  
  	try
  	{
   		#This function is used to delete the orphaned SID 
   		$acl = Get-Acl -Path $Path
   		foreach($acc in $acl.access )
   		{
   			$value = $acc.IdentityReference.Value
   			if($value -match "S-1-5-*")
   			{
   				$ACL.RemoveAccessRule($acc) | Out-Null
   				Set-Acl -Path $Path -AclObject $acl -ErrorAction Stop
   				#Write-Host "Remove SID: $value  form  $Path "
   			}
   		}
  	}
   	catch
   	{
   		#Write-Error $Error
   	}
}
	