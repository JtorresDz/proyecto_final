
CREATE PROCEDURE usp_CreateUser_Plain
 @Username NVARCHAR(100),
 @Password NVARCHAR(200),
 @IsAdmin BIT,
 @NewUserId INT OUTPUT
AS
BEGIN
 INSERT INTO Users (Username,Password,IsAdmin)
 VALUES (@Username,@Password,@IsAdmin);
 SET @NewUserId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE usp_GetUserPlain
 @Username NVARCHAR(100),
 @Password NVARCHAR(200)
AS
BEGIN
 SELECT Id, IsAdmin FROM Users
 WHERE Username=@Username AND Password=@Password;
END;
GO

CREATE PROCEDURE usp_CreateEvent
 @Title NVARCHAR(200),
 @Description NVARCHAR(MAX),
 @EventType NVARCHAR(100),
 @Location NVARCHAR(200),
 @StartDate DATETIME2,
 @EndDate DATETIME2,
 @CreatedByUserId INT,
 @NewEventId INT OUTPUT
AS
BEGIN
 INSERT INTO Events VALUES
 (@Title,@Description,@EventType,@Location,@StartDate,@EndDate,@CreatedByUserId);
 SET @NewEventId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE usp_ListEvents
AS
BEGIN
 SELECT * FROM Events;
END;
GO
