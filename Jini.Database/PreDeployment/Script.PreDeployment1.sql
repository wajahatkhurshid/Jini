/*
 Pre-Deployment Script Template							
*/

/* Delete datafrom the reference tables */

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'RefPeriod')
BEGIN
    DELETE FROM RefPeriod DBCC CHECKIDENT ('dbo.[RefPeriod]',RESEED, 0)
END