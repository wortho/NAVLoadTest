$passwd = Read-Host "Enter NAV User password" -AsSecureString
$instance = Get-NAVServerInstance

$TestUserName = 'USER'

New-NAVServerUser $instance.ServerInstance -UserName $TestUserName -Password $passwd -Verbose -LicenseType Full

New-NAVServerUserPermissionSet $instance.ServerInstance -UserName $TestUserName -PermissionSetId SUPER -Verbose

Get-NAVServerUser $instance.ServerInstance 

for ($i=0; $i -le 100;$i++)
{
    $TestUserName = 'USER' +$i
    New-NAVServerUser $instance.ServerInstance -UserName $TestUserName -Password $passwd -Verbose -LicenseType Full
    New-NAVServerUserPermissionSet $instance.ServerInstance -UserName $TestUserName -PermissionSetId SUPER -Verbose
}

Get-NAVServerUser $instance.ServerInstance 
Get-NAVServerUserPermissionSet $instance.ServerInstance
