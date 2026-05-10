IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509175334_Inicial'
)
BEGIN
    CREATE TABLE [Alumnos] (
        [AlumnoId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellido] nvarchar(100) NOT NULL,
        [FechaNacimiento] datetime2 NOT NULL,
        [Grado] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Alumnos] PRIMARY KEY ([AlumnoId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509175334_Inicial'
)
BEGIN
    CREATE TABLE [Materias] (
        [MateriaId] int NOT NULL IDENTITY,
        [NombreMateria] nvarchar(100) NOT NULL,
        [Docente] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Materias] PRIMARY KEY ([MateriaId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509175334_Inicial'
)
BEGIN
    CREATE TABLE [Expedientes] (
        [ExpedienteId] int NOT NULL IDENTITY,
        [AlumnoId] int NOT NULL,
        [MateriaId] int NOT NULL,
        [NotaFinal] decimal(5,2) NOT NULL,
        [Observaciones] nvarchar(500) NULL,
        CONSTRAINT [PK_Expedientes] PRIMARY KEY ([ExpedienteId]),
        CONSTRAINT [FK_Expedientes_Alumnos_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Alumnos] ([AlumnoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Expedientes_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509175334_Inicial'
)
BEGIN
    CREATE INDEX [IX_Expedientes_AlumnoId] ON [Expedientes] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509175334_Inicial'
)
BEGIN
    CREATE INDEX [IX_Expedientes_MateriaId] ON [Expedientes] ([MateriaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509175334_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260509175334_Inicial', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509212643_AgregarGradoMateria'
)
BEGIN
    ALTER TABLE [Materias] ADD [Grado] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509212643_AgregarGradoMateria'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260509212643_AgregarGradoMateria', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509220842_AgregarTablaCarreras'
)
BEGIN
    CREATE TABLE [Carreras] (
        [CarreraId] int NOT NULL IDENTITY,
        [NombreCarrera] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Carreras] PRIMARY KEY ([CarreraId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260509220842_AgregarTablaCarreras'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260509220842_AgregarTablaCarreras', N'10.0.7');
END;

COMMIT;
GO

