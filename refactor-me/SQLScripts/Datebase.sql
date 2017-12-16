USE [$(DatabaseName)];


GO
PRINT N'Creating [dbo].[Authorization]...';


GO

/* Table that contains Authorization information */
CREATE TABLE [dbo].[Authorization] (
    [Id]       NVARCHAR (50)  NOT NULL,
    [Password] NVARCHAR (500) NOT NULL,
    [Class]    NVARCHAR (50)  NOT NULL,
    [Method]   NVARCHAR (10)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC, [Password] ASC, [Class] ASC, [Method] ASC)
);


GO
PRINT N'Creating [dbo].[Product]...';


GO
CREATE TABLE [dbo].[Product] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Name]          NVARCHAR (100)   NOT NULL,
    [Description]   NVARCHAR (500)   NULL,
    [Price]         DECIMAL (18, 2)  NOT NULL,
    [DeliveryPrice] DECIMAL (18, 2)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[ProductOption]...';


GO
CREATE TABLE [dbo].[ProductOption] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [ProductId]   UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (100)   NOT NULL,
    [Description] NVARCHAR (500)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[FK_ProductOption_Product]...';


GO

/* Referencial integrity FK_ProductOption_Product */
ALTER TABLE [dbo].[ProductOption] WITH NOCHECK
    ADD CONSTRAINT [FK_ProductOption_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id]);


/* Stored procedures for all database operations */
GO
PRINT N'Creating [dbo].[sp_Authorize]...';


GO
CREATE PROCEDURE [dbo].[sp_Authorize]
	@Id nvarchar(50),
	@Password nvarchar(500),
	@Class nvarchar(50),
	@Method nvarchar(10)
AS
BEGIN
	SELECT 1 FROM dbo.[Authorization] WHERE Id = @Id AND [Password] = @Password
		AND Class = @Class AND Method = @Method
END
GO
PRINT N'Creating [dbo].[sp_Product_Delete]...';


GO

CREATE PROCEDURE [dbo].[sp_Product_Delete]
	-- Add the parameters for the stored procedure here
	@Id uniqueidentifier
	
AS
BEGIN
	
	SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN TRY
	BEGIN TRAN	

		IF NOT EXISTS(SELECT 1 FROM dbo.Product WHERE Id = @Id)
			THROW 50000,'Product does not exist',1
		ELSE
		BEGIN

			DELETE FROM dbo.ProductOption
			WHERE ProductId = @Id

			DELETE FROM dbo.Product
			WHERE Id = @Id		
		
	END	

	COMMIT TRAN

	RETURN 0

END TRY
BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000),
			@ErrorSeverity INT,
			@ErrorState INT,
			@ErrorNumber INT

	IF @@TRANCOUNT > 0
		ROLLBACK TRAN

	SELECT @ErrorNumber = ERROR_NUMBER(), @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	
	RETURN 1
END CATCH; 
END
GO
PRINT N'Creating [dbo].[sp_Product_GetById]...';


GO
CREATE PROCEDURE [dbo].[sp_Product_GetById]
	@Id uniqueidentifier
	
AS
BEGIN
	
	SET NOCOUNT ON;  

	SELECT [Name],[Description],Price,DeliveryPrice FROM dbo.Product WHERE Id = @Id
	
END
GO
PRINT N'Creating [dbo].[sp_Product_GetByName]...';


GO
CREATE PROCEDURE [dbo].[sp_Product_GetByName]
	@Name nvarchar(100) = null
	
AS
BEGIN
	
	SET NOCOUNT ON;  

	SELECT Id,[Name],[Description],Price,DeliveryPrice FROM dbo.Product 
	WHERE [Name] = @Name OR @Name IS NULL
	
END
GO
PRINT N'Creating [dbo].[sp_Product_Put]...';


GO
CREATE PROCEDURE [dbo].[sp_Product_Put]
	@Id uniqueidentifier = null, 
	@Name nvarchar(100),
	@Description nvarchar(500) = null,
	@Price decimal(18,2),
	@DeliveryPrice decimal(18,2)
AS
BEGIN
	
	SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN TRY
	BEGIN TRAN
	
	IF(@Id IS NULL)
	BEGIN
		SET @Id = NEWID()

		INSERT INTO dbo.Product (Id,[Name],[Description],Price,DeliveryPrice)
			VALUES (@Id,@Name,@Description,@Price,@DeliveryPrice)
	END
	ELSE
	BEGIN

		IF NOT EXISTS(SELECT 1 FROM dbo.Product WHERE Id = @Id)
			THROW 50000,'Product does not exist',1
		ELSE
		BEGIN
			UPDATE dbo.Product
			SET [Name] = @Name, [Description] = @Description, Price = @Price, DeliveryPrice = @DeliveryPrice
			WHERE Id = @Id
		END
		
	END	

	COMMIT TRAN

	RETURN 0

END TRY
BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000),
			@ErrorSeverity INT,
			@ErrorState INT,
			@ErrorNumber INT

	IF @@TRANCOUNT > 0
		ROLLBACK TRAN

	SELECT @ErrorNumber = ERROR_NUMBER(), @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	
	RETURN 1
END CATCH; 
END
GO
PRINT N'Creating [dbo].[sp_ProductOption_Delete]...';


GO
CREATE PROCEDURE [dbo].[sp_ProductOption_Delete]
	@Id uniqueidentifier,
	@ProductId uniqueidentifier
	
AS
BEGIN
	
	SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN TRY
	BEGIN TRAN	

		IF NOT EXISTS(SELECT 1 FROM dbo.ProductOption WHERE Id = @Id)
			THROW 50000,'Product Option does not exist for Product',1
		ELSE
		BEGIN
			DELETE FROM dbo.ProductOption
			WHERE Id = @Id AND ProductId = @ProductId	
		
	END	

	COMMIT TRAN

	RETURN 0

END TRY
BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000),
			@ErrorSeverity INT,
			@ErrorState INT,
			@ErrorNumber INT

	IF @@TRANCOUNT > 0
		ROLLBACK TRAN

	SELECT @ErrorNumber = ERROR_NUMBER(), @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	
	RETURN 1
END CATCH; 
END
GO
PRINT N'Creating [dbo].[sp_ProductOption_Get]...';


GO
CREATE PROCEDURE [dbo].[sp_ProductOption_Get]
	-- Add the parameters for the stored procedure here
	@ProductId uniqueidentifier,
	@Id uniqueidentifier = null	
AS
BEGIN
	
	SET NOCOUNT ON;  

	SELECT Id,[Name],[Description] FROM dbo.ProductOption
	WHERE Productid= @ProductId AND (Id = @Id OR @id IS NULL)
	
END
GO
PRINT N'Creating [dbo].[sp_ProductOption_Put]...';


GO
CREATE PROCEDURE [dbo].[sp_ProductOption_Put]
	-- Add the parameters for the stored procedure here
	@Id uniqueidentifier = null, 
	@ProductId uniqueidentifier,
	@Name nvarchar(100),
	@Description nvarchar(500) = null
AS
BEGIN
	
	SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN TRY
	BEGIN TRAN
	
	IF NOT EXISTS(SELECT 1 FROM dbo.Product WHERE Id = @ProductId)
		THROW 50000,'Product does not exist',1
		
	IF(@Id IS NULL)
	BEGIN
		SET @Id = NEWID()

		INSERT INTO dbo.ProductOption (Id,ProductId,[Name],[Description])
			VALUES (@Id,@ProductId,@Name,@Description)

		END
	ELSE
	BEGIN

		IF NOT EXISTS(SELECT 1 FROM dbo.ProductOption WHERE Id = @Id AND ProductId = @ProductId)
			THROW 50000,'Product Option does not exist for the Product',1
		ELSE
		BEGIN
			UPDATE dbo.ProductOption
			SET [Name] = @Name, [Description] = @Description
			WHERE Id = @Id AND ProductId = @ProductId
		END
		
	END

	COMMIT TRAN

	RETURN 0

END TRY
BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000),
			@ErrorSeverity INT,
			@ErrorState INT,
			@ErrorNumber INT

	IF @@TRANCOUNT > 0
		ROLLBACK TRAN

	SELECT @ErrorNumber = ERROR_NUMBER(), @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
		
	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	
	RETURN 1
END CATCH; 
END
GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[ProductOption] WITH CHECK CHECK CONSTRAINT [FK_ProductOption_Product];


GO
PRINT N'Update complete.';


GO
