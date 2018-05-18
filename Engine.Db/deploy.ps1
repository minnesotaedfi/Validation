<#
.SYNOPSIS
Deploy databases from this package.
.DESCRIPTION
Deploy databases from this package, based on the infomration contained in the PublishProfile.
Options based on Profile additional properties:
To update existing databases based on a Regular Expression, add the TargetDatabaseNameMatch property, with a value of the Regular Expresion to the Property Group in the sqlproj file.
To Restore from a backup before applying the dacpac, add the RestoreFromBackup property, with a value of the path to the backup file to the Property Group in the sqlproj file.
.PARAMETER DacpacPath
The path to the dacpac file.
.PARAMETER PublishProfilePath
The path to the xml profile.
.PARAMETER WhatIf
Performs a WhatIf deployment without modifying anything.
.EXAMPLE
# Deploy a dacpac
.\deploy.ps1 -DacpacPath "MyDacPac.dacpac" -PublishProfilePath "MyProfile.publish.xml" -WhatIf
#>
[CmdletBinding()]
Param(
[String]$DacpacPath = "Engine.Db.dacpac",
[String]$PublishProfilePath = "OctopusDeploy.publish.xml",
[Switch]$WhatIf
)
<#
Deploy databases from this package.
.description
Deploy databases from this package.
#>
Import-Module "$(Split-Path -parent $PSCommandPath)\DLP_Deploy_Dacpac_v2.psm1"

Install-DacPac -DacpacPath $DacpacPath -PublishProfilePath $PublishProfilePath -WhatIf:$WhatIf