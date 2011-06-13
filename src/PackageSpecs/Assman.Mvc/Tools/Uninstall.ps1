$project = Get-Project
$msbProject = Get-MSBuildProject

function Remove-Target($Name)
{
	Write-Host "Removing the $Name target from the project..."
	$target = $($msbProject.Xml.Targets | where { $_.Name -ieq $Name })
	if($target -ne $null)
	{
		$msbProject.Xml.RemoveChild($target)
	}
}

function Remove-Import($PartialMatch)
{
	Write-Host "Removing the $PartialMatch import from the project..."
	$import = $($msbProject.Xml.Imports | where { $_.Project.ToLowerInvariant().Contains($PartialMatch.ToLowerInvariant()) })
	if($import -ne $null)
	{
		$msbProject.Xml.RemoveChild($import)
	}
}

function Uninstall
{
	Remove-Target "PreCompileResources"
	Remove-Target "CleanPreCompiledResources"
	Remove-Import "Assman.MSBuild.tasks"

	$project.Save()
}

Uninstall
