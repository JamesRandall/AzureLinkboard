Add-Type -Path '.\packages\accidentalfish.applicationsupport.0.1.0.19\tools\net45\Microsoft.Data.OData.dll'
Add-Type -Path '.\packages\accidentalfish.applicationsupport.0.1.0.19\tools\net45\Microsoft.Data.Services.Client.dll'
Import-Module .\packages\accidentalfish.applicationsupport.0.1.0.19\tools\net45\AccidentalFish.ApplicationSupport.Powershell.dll

Remove-ApplicationResources -Configuration $configuration -Settings $settings
