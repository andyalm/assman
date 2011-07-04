param($installPath, $toolsPath, $package, $project)

$frameworkMoniker = $project.Properties.Item("TargetFrameworkMoniker").Value
$frameworkVersion = $(new-object System.Runtime.Versioning.FrameworkName $frameworkMoniker).Version
Write-Host "$frameworkVersion framework detected"
if($frameworkVersion.Major -lt 4)
{
	Write-Warning "It was detected that your project is targeting .NET 3.5 or below.  To ensure that Assman.dotLess works correctly, please follow the instructions listed here: http://assman.codeplex.com/wikipage?title=dotLess"
}