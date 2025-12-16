ALTER PROCEDURE usp_Admin_UpdateEvent
 @Id INT,
 @Titulo NVARCHAR(200),
 @Descripcion NVARCHAR(MAX),
 @Tipo NVARCHAR(100),
 @Localizacion NVARCHAR(200),
 @Inicio DATETIME2,
 @Final DATETIME2,
 @UserId INT
AS
BEGIN
    UPDATE Eventos
    SET
        Titulo = @Titulo,
        Descripcion = @Descripcion,
        Tipo = @Tipo,
        Localizacion = @Localizacion,
        Inicio = @Inicio,
        Final = @Final
    WHERE Id = @Id
      AND CreadoPorId = @UserId;
END
GO
