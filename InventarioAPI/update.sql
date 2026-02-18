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
CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Correo] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [RolId] int NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Nombre])
VALUES (1, N'Admin'),
(2, N'Empleado');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Correo', N'Nombre', N'PasswordHash', N'RolId') AND [object_id] = OBJECT_ID(N'[Usuarios]'))
    SET IDENTITY_INSERT [Usuarios] ON;
INSERT INTO [Usuarios] ([Id], [Correo], [Nombre], [PasswordHash], [RolId])
VALUES (1, N'admin@inventario.com', N'Administrador', N'$2a$11$F6tVnR9l3P1iW8kzTkHkX.0VGdO7K1uwk3qT1YpZI9vVvV5F0.0pO', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Correo', N'Nombre', N'PasswordHash', N'RolId') AND [object_id] = OBJECT_ID(N'[Usuarios]'))
    SET IDENTITY_INSERT [Usuarios] OFF;

CREATE INDEX [IX_Usuarios_RolId] ON [Usuarios] ([RolId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260213232100_Inicial', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Usuarios] DROP CONSTRAINT [FK_Usuarios_Roles_RolId];

DELETE FROM [Usuarios]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Nombre');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Usuarios] ALTER COLUMN [Nombre] nvarchar(100) NOT NULL;

DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Correo');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [Usuarios] ALTER COLUMN [Correo] nvarchar(150) NOT NULL;

CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);

ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260213235442_InicialFinal', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Categorias] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Activa] bit NOT NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
);

CREATE TABLE [Productos] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [CodigoBarras] nvarchar(50) NOT NULL,
    [PrecioCompra] decimal(10,2) NOT NULL,
    [PrecioVenta] decimal(10,2) NOT NULL,
    [Stock] int NOT NULL,
    [StockMinimo] int NOT NULL,
    [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CategoriaId] int NOT NULL,
    CONSTRAINT [PK_Productos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Productos_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Productos_CategoriaId] ON [Productos] ([CategoriaId]);

CREATE UNIQUE INDEX [IX_Productos_CodigoBarras] ON [Productos] ([CodigoBarras]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260214190414_InventarioInicial', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260214212115_ActualizacionProductosV2', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Productos] ADD [FechaVencimiento] datetime2 NULL;

ALTER TABLE [Productos] ADD [Marca] nvarchar(max) NULL;

ALTER TABLE [Productos] ADD [UnidadMedida] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260214214015_AgregarCamposProProducto', N'10.0.0');

COMMIT;
GO

