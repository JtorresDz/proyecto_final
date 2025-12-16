CREATE TABLE Inscripciones (
    Id INT IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL,
    EventoId INT NOT NULL,
    FechaRegistro DATETIME2 DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Inscripciones_Usuario
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),

    CONSTRAINT FK_Inscripciones_Evento
        FOREIGN KEY (EventoId) REFERENCES Eventos(Id),

    CONSTRAINT UQ_Usuario_Evento
        UNIQUE (UsuarioId, EventoId)
);


CREATE OR ALTER PROCEDURE usp_RegisterToEvent
    @UsuarioId INT,
    @EventoId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Evitar doble inscripción
    IF EXISTS (
        SELECT 1 FROM Inscripciones
        WHERE UsuarioId = @UsuarioId AND EventoId = @EventoId
    )
    BEGIN
        RAISERROR('El usuario ya está inscrito en este evento', 16, 1);
        RETURN;
    END

    INSERT INTO Inscripciones (UsuarioId, EventoId)
    VALUES (@UsuarioId, @EventoId);
END
GO


CREATE OR ALTER PROCEDURE usp_IsUserRegistered
    @UsuarioId INT,
    @EventoId INT
AS
BEGIN
    SELECT COUNT(1)
    FROM Inscripciones
    WHERE UsuarioId = @UsuarioId
      AND EventoId = @EventoId;
END
GO


CREATE OR ALTER PROCEDURE usp_CancelRegistration
    @UsuarioId INT,
    @EventoId INT
AS
BEGIN
    DELETE FROM Inscripciones
    WHERE UsuarioId = @UsuarioId
      AND EventoId = @EventoId;
END
GO