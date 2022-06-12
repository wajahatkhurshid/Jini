/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
             WHERE TABLE_NAME = N'ProductAccessProvider' ))           
BEGIN
   drop table [dbo].[ProductAccessProvider];
END

Go

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
             WHERE TABLE_NAME = N'RefAccessProvider' ))           
BEGIN
   delete from [dbo].[RefAccessProvider];
END

Go

IF (Not EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
             WHERE TABLE_NAME = N'Product' ))           
BEGIN
  
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsExternalLogin] [bit] NOT NULL,
	[Isbn] [varchar](13) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO

IF (Not EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
             WHERE TABLE_NAME = N'ProductAccessProvider' ))           
BEGIN

   CREATE TABLE [dbo].[ProductAccessProvider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccessProviderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[ProductAccessProvider]  WITH CHECK ADD  CONSTRAINT [FK_AccessProvider] FOREIGN KEY([AccessProviderId])
REFERENCES [dbo].[RefAccessProvider] ([Code])


ALTER TABLE [dbo].[ProductAccessProvider] CHECK CONSTRAINT [FK_AccessProvider]


ALTER TABLE [dbo].[ProductAccessProvider]  WITH CHECK ADD  CONSTRAINT [FK_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])


ALTER TABLE [dbo].[ProductAccessProvider] CHECK CONSTRAINT [FK_Product]
END

GO


