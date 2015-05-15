Import-Module 'C:\NAVDVD\WindowsPowerShellScripts\Multitenancy\NAVMultitenancySamples.psm1'
Import-Module 'C:\Program Files\Microsoft Dynamics NAV\80\Service\NavAdminTool.ps1'
$instance = Get-NAVServerInstance
$sqlservername = "localhost\NAVDEMO"

for ($i=5; $i -le 5;$i++)
{
    $CompanyName = 'CRONUS' +$i    
    HowTo-MoveCompanyToTenant -ServerInstance $instance.ServerInstance –FromDatabase 'Demo Database NAV (8-0)' -OldTenantName Default -NewTenantName $CompanyName -CompanyName $CompanyName -ToDatabase $CompanyName -DatabaseServer $sqlservername -Verbose

    Get-NAVTenant $instance.ServerInstance -Tenant $CompanyName

    Get-NAVCompany $instance.ServerInstance -Tenant Default

    Remove-NAVCompany -Tenant Default -CompanyName $CompanyName -ServerInstance $instance.ServerInstance -Force -Verbose

    Get-NAVCompany $instance.ServerInstance -Tenant Default

    Get-NAVCompany $instance.ServerInstance -Tenant $CompanyName
}
