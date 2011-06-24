param($installPath, $toolsPath, $package, $project)

$project = Get-MSBuildProject

function Get-InstalledPackagePath($Name)
{
	$package = Get-Package $Name -First 1
	$componentModel = Get-VSComponentModel
	$repositorySettings = $componentModel.GetService([NuGet.VisualStudio.IRepositorySettings])
	$pathResolver = New-Object NuGet.DefaultPackagePathResolver($repositorySettings.RepositoryPath)
	return $pathResolver.GetInstallPath($package)
}

function Install
{
	$projectDir = Split-Path $project.FullPath -Parent
	pushd $projectDir

	$assmanPackageDir = Get-InstalledPackagePath "Assman"
	$assmanPackageDir = Resolve-Path $assmanPackageDir -Relative

	# Add <Import> for Assman.MSBuild.tasks
	Write-Host "Adding the Assman.MSBuild.tasks import to the project..."
	$project.Xml.AddImport("$assmanPackageDir\Tools\Assman.MSBuild.tasks") | Out-Null

	# Add PreCompileResources target
	Write-Host "Adding the PreCompileResources target to the project..."
	$target = $project.Xml.AddTarget("PreCompileResources")
	$target.AfterTargets = "Build"
	$target.Condition = "`$(PreCompileResources)=='true'"
	$task = $target.AddTask("PreCompileResources")
	$task.SetParameter("WebRoot", '$(WebProjectOutputDir)')

	# Add CleanPreCompiledResources target
	Write-Host "Adding the CleanPreCompileResources target to the project..."
	$target = $project.Xml.AddTarget("CleanPreCompiledResources")
	$target.AfterTargets = "Clean"
	$task = $target.AddTask("Delete")
	$task.SetParameter("Files", '$(WebProjectOutputDir)\bin\Assman.compiled')

	# Call save against Get-Project instead of the Get-MSBuildProject
	# because then VS won't prompt you to reload the project
	$(Get-Project).Save()

	popd
}

Install