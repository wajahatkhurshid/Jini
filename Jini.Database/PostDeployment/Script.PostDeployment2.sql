DECLARE @loginName AS NVARCHAR(100)
DECLARE @SqlStatement AS NVARCHAR(500)

SET @loginName = 'GYLDENDAL\sa-jini-d'

If not Exists (select loginname from master.dbo.syslogins 
    where name = @loginName)
Begin
Set @SqlStatement = 'CREATE LOGIN [' + @loginName + '] 	FROM WINDOWS WITH DEFAULT_DATABASE=     ['+ DB_NAME()+'], DEFAULT_LANGUAGE=[us_english]'
	print 'Creating Login: ' + @loginName
EXEC sp_executesql @SqlStatement
END

If not Exists (select @loginName from sys.database_principals
    where name = @loginName)
Begin
Set @SqlStatement = 'CREATE USER [' + @loginName + '] FOR LOGIN['+@loginName + ']'
print 'Creating USER: ' + @loginName
EXEC sp_executesql @SqlStatement
END

Set @SqlStatement = 'GRANT CONNECT TO [' + @loginName + ']'
print 'GRANT Connect To USER: ' + @loginName
EXEC sp_executesql @SqlStatement

Set @SqlStatement = 'ALTER ROLE [db_datawriter] ADD MEMBER [' + @loginName + ']'
print 'DataWriter USER: ' + @loginName
EXEC sp_executesql @SqlStatement

Set @SqlStatement = 'ALTER ROLE [db_datareader] ADD MEMBER [' + @loginName + ']'
print 'DataReader USER: ' + @loginName
EXEC sp_executesql @SqlStatement