Import-Module 'C:\Program Files\Microsoft Dynamics NAV\80\Service\NavAdminTool.ps1'

$passwd = Read-Host "Enter NAV User password" -AsSecureString
$instance = Get-NAVServerInstance

foreach ($Tenant in Get-NAVTenant $instance.ServerInstance)
{

    $TestUserName = 'USER'

    New-NAVServerUser $instance.ServerInstance -Tenant $Tenant.Id -UserName $TestUserName -Password $passwd -Verbose -LicenseType Full

    New-NAVServerUserPermissionSet $instance.ServerInstance -Tenant $Tenant.Id -UserName $TestUserName -PermissionSetId SUPER -Verbose

    Get-NAVServerUser $instance.ServerInstance -Tenant $Tenant.Id

    for ($i=0; $i -le 5;$i++)
    {
        $TestUserName = 'USER' +$i
        New-NAVServerUser $instance.ServerInstance -Tenant $Tenant.Id -UserName $TestUserName -Password $passwd -Verbose -LicenseType Full
        New-NAVServerUserPermissionSet $instance.ServerInstance -Tenant $Tenant.Id -UserName $TestUserName -PermissionSetId SUPER -Verbose
    }

    Get-NAVServerUser $instance.ServerInstance -Tenant $Tenant.Id
}

