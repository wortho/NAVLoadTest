Set-ExecutionPolicy unrestricted
Import-Module 'C:\Program Files\Microsoft Dynamics NAV\80\Service\NavAdminTool.ps1'
$instance = Get-NAVServerInstance
$sqlservername = "localhost\NAVDEMO"

Export-NAVApplication –DatabaseServer $sqlservername –DatabaseName 'Demo Database NAV (8-0)’ –DestinationDatabaseName 'DemoNAVApp' –Force

Set-NavServerInstance $instance.ServerInstance –Stop
 Set-NAVServerConfiguration -ServerInstance $instance.ServerInstance -Keyname MultiTenant  -KeyValue "True"
 Set-NAVServerConfiguration -ServerInstance $instance.ServerInstance -Keyname DatabaseName -KeyValue "DemoNAVApp"
 Set-NavServerInstance $instance.ServerInstance –Start

 Mount-NAVApplication -DatabaseServer $sqlservername -DatabaseName ‘DemoNAVApp’ -ServerInstance $instance.ServerInstance
 Get-NAVApplication $instance.ServerInstance

 Mount-NAVTenant -ServerInstance $instance.ServerInstance -Id Default –DatabaseName ‘Demo Database NAV (8-0)’ -OverWriteTenantIDInDatabase –AllowAppDatabaseWrite

 Get-NAVTenant $instance.ServerInstance

 Get-NAVCompany $instance.ServerInstance -Tenant Default