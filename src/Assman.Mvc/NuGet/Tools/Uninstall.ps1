$project = Get-MSBuildProject

function Remove-Target($Name)
{
	Write-Host "Removing the $Name target from the project..."
	$target = $($project.Xml.Targets | where { $_.Name -ieq $Name })
	if($target -ne $null)
	{
		$project.Xml.RemoveChild($target)
	}
}

function Remove-Import($PartialMatch)
{
	Write-Host "Removing the $PartialMatch import from the project..."
	$import = $($project.Xml.Imports | where { $_.Project.ToLowerInvariant().Contains($PartialMatch.ToLowerInvariant()) })
	if($import -ne $null)
	{
		$project.Xml.RemoveChild($import)
	}
}

function Uninstall
{
	Remove-Target "PreCompileResources"
	Remove-Target "CleanPreCompiledResources"
	Remove-Import "Assman.MSBuild.tasks"

	# Call save against Get-Project instead of the Get-MSBuildProject
	# because then VS won't prompt you to reload the project
	$(Get-Project).Save()
}

Uninstall
