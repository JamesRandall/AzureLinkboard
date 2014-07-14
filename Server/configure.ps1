$webconfig = $location.tostring() + "\AzureLinkboard.Web.Api\web.config"
$infrastructurecsdef = $location.tostring() + "\AzureLinkboard.Infrastructure\ServiceDefinition.csdef"
$infrastructurecloudcscfg = $location.tostring() + "\AzureLinkboard.Infrastructure\ServiceConfiguration.Cloud.cscfg"
$infrastructurelocalcscfg = $location.tostring() + "\AzureLinkboard.Infrastructure\ServiceConfiguration.Local.cscfg"
$applicationsupportpackagefolder = Get-ChildItem .\packages\accidentalfish.applicationsupport* -name
$toolspath = '.\packages\' + $applicationsupportpackagefolder + '\tools\net45\'

Add-Type -Path ($toolspath + 'Microsoft.Data.OData.dll')
Add-Type -Path ($toolspath + 'Microsoft.Data.Services.Client.dll')
Import-Module ($toolspath + 'AccidentalFish.ApplicationSupport.Powershell.dll')

Set-ApplicationConfiguration -Configuration $configuration -Target $infrastructurecsdef -Settings $settings
Set-ApplicationConfiguration -Configuration $configuration -Target $infrastructurecloudcscfg -Settings $settings
Set-ApplicationConfiguration -Configuration $configuration -Target $infrastructurelocalcscfg -Settings $settings
Set-ApplicationConfiguration -Configuration $configuration -Target $webconfig -Settings $settings
New-ApplicationResources -Configuration $configuration -Settings $settings
