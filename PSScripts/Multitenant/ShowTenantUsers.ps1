Import-Module 'C:\Program Files\Microsoft Dynamics NAV\80\Service\NavAdminTool.ps1'
$instance = Get-NAVServerInstance
foreach ($tenant in Get-NAVTenant $instance.ServerInstance)
{
    Get-NAVServerSession -ServerInstance $instance.ServerInstance -Tenant $tenant.Id -WarningAction SilentlyContinue | Format-Table -Property UserID,ClientType, @{Label="Tenant"; Expression={$tenant.Id}}
}