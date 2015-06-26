$instance = Get-NAVServerInstance
$TestUserName = 'USER'

Remove-NAVServerUser $instance.ServerInstance -UserName $TestUserName -Force

Get-NAVServerUser $instance.ServerInstance 

for ($i=0; $i -le 100;$i++)
{
    $TestUserName = 'User' +$i
    Remove-NAVServerUser $instance.ServerInstance -UserName $TestUserName -Force
}

Get-NAVServerUser $instance.ServerInstance 
