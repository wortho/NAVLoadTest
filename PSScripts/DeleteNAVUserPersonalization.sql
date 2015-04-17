SELECT [User Security ID], [User Name]
  FROM [Demo Database NAV (8-0)].[dbo].[User]
  WHERE [User Name] LIKE 'USER%'

    
DELETE FROM
[dbo].[User Personalization]
WHERE [User SID] IN (SELECT [User Security ID]
  FROM [Demo Database NAV (8-0)].[dbo].[User]
  WHERE [User Name] LIKE 'USER%')
