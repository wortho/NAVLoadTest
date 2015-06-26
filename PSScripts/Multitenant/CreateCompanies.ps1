Set-ExecutionPolicy unrestricted
Import-Module 'C:\Program Files\Microsoft Dynamics NAV\80\Service\NavAdminTool.ps1'
$instance = Get-NAVServerInstance
$company = Get-NAVCompany $instance.ServerInstance

for ($i=0; $i -le 5;$i++)
{
    $CompanyName = 'CRONUS' +$i    
    Copy-NAVCompany -DestinationCompanyName $CompanyName –SourceCompanyName $company.CompanyName -ServerInstance $instance.ServerInstance
}