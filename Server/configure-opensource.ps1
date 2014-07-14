$location = Get-Location
$configuration = $location.tostring() + "\configuration.xml"
$settings = $location.tostring() + "\settings-opensource.xml"
.\configure.ps1
