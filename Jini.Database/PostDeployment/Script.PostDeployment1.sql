
/* Insert Db Version */
INSERT INTO [dbo].[DbVersion]
           ([VersionNo]
           ,[SequenceNo]
           ,[CreatedDate]
           ,[Note]
           ,[ScriptName])
     VALUES
           ('1.0'
           ,1
           ,GETDATE()
           ,'Note'
           ,'Script')

/* Data has to be inserted in the correct order */

DECLARE @code AS INT
DECLARE @displayName AS NVARCHAR(100)
DECLARE @ExternalIdentifier AS int 
DECLARE @PeriodTypeName AS NVARCHAR(100)
DECLARE @showPercentage AS int


DECLARE @loopCounter INT;
SET @loopCounter = 1;

WHILE @loopCounter <= 2
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1001
		set @displayName  = 'Abonnement'
		set @ExternalIdentifier  = 1
		set @PeriodTypeName  = 'binding'
	END 
	ELSE  
	BEGIN
		set @code  = 1002
		set @displayName  = 'Leje'
		set @ExternalIdentifier  = 2
		set @PeriodTypeName  = 'adgang'
	END 

	/* RefSalesForm */
	IF (NOT EXISTS(SELECT * FROM RefSalesForm WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefSalesForm
		(
			Code,
			DisplayName,
			ExternalIdentifier,
			PeriodTypeName
		)
		VALUES
		(
			@code,
			@displayName,
			@ExternalIdentifier,
			@PeriodTypeName
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefSalesForm
			SET 
			 DisplayName =@displayName,
			 ExternalIdentifier = @ExternalIdentifier,
			 PeriodTypeName = @PeriodTypeName
		WHERE Code = @code
	END 

   SET @loopCounter = @loopCounter + 1;
END


/*Ref Trial Period Unit Type*/

SET @loopCounter = 1;
WHILE @loopCounter <= 3
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1002
		set @displayName  = 'måneders'
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @code  = 1003
		set @displayName  = 'dages'
	END 
	ELSE  if (@loopCounter = 3)
	BEGIN
		set @code  = 1004
		set @displayName  = 'timers'
	END 

	IF (NOT EXISTS(SELECT * FROM RefTrialPeriodUnitType WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefTrialPeriodUnitType
		(

			Code,
			DisplayName
		)
		VALUES
		(
			@code,
			@displayName
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefTrialPeriodUnitType
			SET 
			 DisplayName =@displayName
		WHERE Code = @code
	END 

	SET @loopCounter = @loopCounter + 1;
END


/*Ref Trial Count Unit Type*/

SET @loopCounter = 1;
WHILE @loopCounter <= 3
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1001
		set @displayName  = 'per år'
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @code  = 1002
		set @displayName  = 'per måned'
	END 
	ELSE  if (@loopCounter = 3)
	BEGIN
		set @code  = 1003
		set @displayName  = 'for altid'
	END 

	IF (NOT EXISTS(SELECT * FROM RefTrialCountUnitType WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefTrialCountUnitType
		(

			Code,
			DisplayName
		)
		VALUES
		(
			@code,
			@displayName
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefTrialCountUnitType
			SET 
			 DisplayName =@displayName
		WHERE Code = @code
	END 

	SET @loopCounter = @loopCounter + 1;
END


/* RefPeriodUnitType */

SET @loopCounter = 1;
WHILE @loopCounter <= 4
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1001
		set @displayName  = 'års'
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @code  = 1002
		set @displayName  = 'måneders'
	END 
	ELSE  if (@loopCounter = 3)
	BEGIN
		set @code  = 1003
		set @displayName  = 'dages'
	END 
	ELSE  if (@loopCounter = 4)
	BEGIN
		set @code  = 1004
		set @displayName  = 'timers'
	END 

	/* RefSalesForm */
	IF (NOT EXISTS(SELECT * FROM RefPeriodUnitType WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefPeriodUnitType
		(

			Code,
			DisplayName
		)
		VALUES
		(
			@code,
			@displayName
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefPeriodUnitType
			SET 
			 DisplayName =@displayName
		WHERE Code = @code
	END 

   SET @loopCounter = @loopCounter + 1;
END

/*Add binding for 6 months */
INSERT INTO [dbo].[RefPeriod]
([UnitValue]
,[RefPeriodUnitTypeCode])
VALUES
(6,
1002)
/*Add binding  for 1 year */
INSERT INTO [dbo].[RefPeriod]
([UnitValue]
,[RefPeriodUnitTypeCode])
VALUES
(1,
1001)
/*Add binding  for 2 year */
INSERT INTO [dbo].[RefPeriod]
([UnitValue]
,[RefPeriodUnitTypeCode])
VALUES
(2,
1001)
/*Add binding  for 3 year */
INSERT INTO [dbo].[RefPeriod]
([UnitValue]
,[RefPeriodUnitTypeCode])
VALUES
(3,
1001)


/* RefTrialAccessForm */

SET @loopCounter = 1;
WHILE @loopCounter <= 2
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1001
		set @displayName  = 'Skole'
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @code  = 1004
		set @displayName  = 'Enkeltbruger'
	END 

	/* RefTrialAccessForm */
	IF (NOT EXISTS(SELECT * FROM RefTrialAccessForm WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefTrialAccessForm
		(
			Code,
			DisplayName
		)
		VALUES
		(
			@code,
			@displayName
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefTrialAccessForm
			SET 
			 DisplayName =@displayName
		WHERE Code = @code
	END 

   SET @loopCounter = @loopCounter + 1;
END


/* RefAccessForm */

SET @loopCounter = 1;
WHILE @loopCounter <= 5
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1001
		set @displayName  = 'Skolelicens'
		set @ExternalIdentifier = 1
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @code  = 1002
		set @displayName  = 'Klasselicens'
		set @ExternalIdentifier = 2
	END 
	ELSE  if (@loopCounter = 3)
	BEGIN
		set @code  = 1003
		set @displayName  = 'Underviserlicens'
		set @ExternalIdentifier = NULL
	END 
	ELSE  if (@loopCounter = 4)
	BEGIN
		set @code  = 1004
		set @displayName  = 'Enkeltbrugerlicens'
	END 
	ELSE  if (@loopCounter = 5)
	BEGIN
		set @code  = 1005
		set @displayName  = 'Friteksfelt'
	END 

	/* RefSalesForm */
	IF (NOT EXISTS(SELECT * FROM RefAccessForm WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefAccessForm
		(
			Code,
			DisplayName,
			ExternalIdentifier
		)
		VALUES
		(
			@code,
			@displayName,
			@ExternalIdentifier
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefAccessForm
			SET 
			 DisplayName =@displayName,
			 ExternalIdentifier =@ExternalIdentifier
		WHERE Code = @code
	END 

   SET @loopCounter = @loopCounter + 1;
END


/* RefPriceModel */

-- Skole Price Models 
declare  @RefAccessFormCode as int
SET @loopCounter = 1;
WHILE @loopCounter <= 8
BEGIN
	if (@loopCounter = 1)
	BEGIN
		set @code  = 1001
		--skole added for distinction
		set @displayName  = 'Hele skolen (enhedspris)'
		set @RefAccessFormCode = 1001
		set @showPercentage = 0
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @code  = 1002
		set @displayName  = 'Antal elever'
		set @RefAccessFormCode = 1001
		set @showPercentage = 0
	END 
	ELSE  if (@loopCounter = 3)
	BEGIN
		set @code  = 1003
		set @displayName  = 'Procentdel af elever'
		set @RefAccessFormCode = 1001
		set @showPercentage = 1
	END 
	ELSE  if (@loopCounter = 4)
	BEGIN
		set @code  = 1004
		set @displayName  = 'Antal elever på relevante klassetrin'
		set @RefAccessFormCode = 1001
		set @showPercentage = 0
	END 
	ELSE  if (@loopCounter = 5)
	BEGIN
		set @code  = 1005
		set @displayName  = 'Antal klasser på relevante klassetrin'
		set @RefAccessFormCode = 1001
		set @showPercentage = 0
	END 
	ELSE  if (@loopCounter = 6)
	BEGIN
		set @code  = 1006
		set @displayName  = 'Antal klasser'
		set @RefAccessFormCode = 1002
		set @showPercentage = 0
	END 
	ELSE  if (@loopCounter = 7)
	BEGIN
		set @code  = 1007
		set @displayName  = 'Antal elever i klasser'
		set @RefAccessFormCode = 1002
		set @showPercentage = 0
	END 
	ELSE  if (@loopCounter = 8)
	BEGIN
		set @code  = 1008
		set @displayName  = 'Hele skolen (enhedspris)'
		set @RefAccessFormCode = 1003
		set @showPercentage = 0
	END 

	/* RefSalesForm */
	IF (NOT EXISTS(SELECT * FROM RefPriceModel WHERE Code = @code)) 
	BEGIN 
		INSERT INTO RefPriceModel
		(
			Code,
			DisplayName,
			RefAccessFormCode,
			ShowPercentage
		)
		VALUES
		(
			@code,
			@displayName,
			@RefAccessFormCode,
			@showPercentage
		)
	END 
	ELSE 
	BEGIN 
		UPDATE RefPriceModel
			SET 
			 DisplayName =@displayName,
			 RefAccessFormCode =@RefAccessFormCode,
			 ShowPercentage = @showPercentage
		WHERE Code = @code
	END 

   SET @loopCounter = @loopCounter + 1;
END

/* Seller */
declare  @sellerName varchar(500)
declare @webshopId int
SET @loopCounter = 1;
WHILE @loopCounter <= 4
BEGIN

	if (@loopCounter = 1)
	BEGIN
		set @sellerName  = 'Gyldendal Uddannelse'
		set @webshopId=2
	END 
	ELSE  if (@loopCounter = 2)
	BEGIN
		set @sellerName  = 'MunksgaardDanmark'
		set @webshopId=5
	END 
	ELSE  if (@loopCounter = 3)
	BEGIN
		set @sellerName  = 'HansReitzelsForlag'
		set @webshopId=3
	END 
	ELSE  if (@loopCounter = 4)
	BEGIN
		set @sellerName  = 'Nonfiktion'
		set @webshopId=2
	END 

	IF (NOT EXISTS(SELECT * FROM Seller WHERE Name =  @sellerName)) 
	BEGIN 
	INSERT INTO Seller
	(
		Name,
		WebShopId
	)
	VALUES
	(
		 @sellerName,
		 @webshopId
	)
	END
	ELSE
	BEGIN
	UPDATE	Seller Set WebShopId=@webshopId where Name=@sellerName
	END
	SET @loopCounter = @loopCounter + 1;
END



