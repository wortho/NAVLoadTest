Import-Module 'C:\Program Files\Microsoft Dynamics NAV\80\Service\NavAdminTool.ps1'
Get-NAVServerInstance | Get-NAVApplication

Get-NAVServerInstance | Get-NAVTenant | Format-Table -Property Id,DatabaseName

Get-NAVServerInstance | Get-NAVServerUser -Tenant cronus0 -WarningAction SilentlyContinue | Format-Table -Property UserName