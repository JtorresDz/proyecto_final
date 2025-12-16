
CREATE PROCEDURE usp_Admin_ListEvents
AS
SELECT Id, Titulo, Descripcion, Tipo, Inicio FROM Eventos;
GO

CREATE PROCEDURE usp_Admin_CreateEvent
 @Titulo NVARCHAR(100), @Descripcion NVARCHAR(255), @Tipo NVARCHAR(50), @Inicio DATE
AS
INSERT INTO Eventos VALUES(@Titulo,@Descripcion,@Tipo,@Inicio);
GO

CREATE PROCEDURE usp_Admin_UpdateEvent
 @Id INT,@Titulo NVARCHAR(100),@Descripcion NVARCHAR(255),@Tipo NVARCHAR(50),@Inicio DATE
AS
UPDATE Eventos SET Titulo=@Titulo,Descripcion=@Descripcion,Tipo=@Tipo,Inicio=@Inicio WHERE Id=@Id;
GO

CREATE PROCEDURE usp_Admin_DeleteEvent
 @Id INT
AS
DELETE FROM Eventos WHERE Id=@Id;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'usp_Admin_UpdateEvent' AND type = 'P')
    DROP PROCEDURE usp_Admin_UpdateEvent;
GO

CREATE PROCEDURE usp_Admin_UpdateEvent
 @Id INT,
 @Titulo NVARCHAR(100),
 @Descripcion NVARCHAR(255),
 @Tipo NVARCHAR(50),
 @Inicio DATE
AS
BEGIN
    UPDATE Eventos
    SET 
        Titulo = @Titulo,
        Descripcion = @Descripcion,
        Tipo = @Tipo,
        Inicio = @Inicio
    WHERE Id = @Id;
END
GO
