$passwd = Read-Host "Enter NAV User password" -AsSecureString
$instance = Get-NAVServerInstance

for ($i=0; $i -le 10;$i++)
{
    $TestUserName = 'User' +$i
    New-NAVServerUser $instance.ServerInstance -UserName $TestUserName -Password $passwd -Verbose -LicenseType Full
    New-NAVServerUserPermissionSet $instance.ServerInstance -UserName $TestUserName -PermissionSetId SUPER -Verbose
}

Get-NAVServerUser $instance.ServerInstance 
Get-NAVServerUserPermissionSet $instance.ServerInstance 


