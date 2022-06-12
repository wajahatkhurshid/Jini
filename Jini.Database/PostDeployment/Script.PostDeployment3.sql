
IF (Not EXISTS (SELECT * FROM [dbo].[RefAccessProvider]
             WHERE [Code] = 1001 or [Identifier]=N'Ekey'))           
BEGIN
INSERT [dbo].[RefAccessProvider] ([Code], [Identifier], [DisplayName]) VALUES (1001, N'Ekey', N'Gyldendal / MU / HR login')
END

IF (Not EXISTS (SELECT * FROM [dbo].[RefAccessProvider]
             WHERE [Code] = 1002 or [Identifier]=N'Unic'))           
BEGIN
INSERT [dbo].[RefAccessProvider] ([Code], [Identifier], [DisplayName]) VALUES (1002, N'Unic', N'UNI-login')
END

IF (Not EXISTS (SELECT * FROM [dbo].[RefAccessProvider]
             WHERE [Code] = 1004 or [Identifier]=N'WAYF'))           
BEGIN
INSERT [dbo].[RefAccessProvider] ([Code], [Identifier], [DisplayName]) VALUES (1004, N'WAYF', N'WAYF')
End

IF (Not EXISTS (SELECT * FROM [dbo].[RefAccessProvider]
             WHERE [Code] = 1005 or [Identifier]=N'IP'))           
BEGIN
INSERT [dbo].[RefAccessProvider] ([Code], [Identifier], [DisplayName]) VALUES (1005, N'IP', N'IP')
End

