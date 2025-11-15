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
CREATE TABLE [User] (
    [Id] bigint NOT NULL IDENTITY,
    [Forename] nvarchar(100) NOT NULL,
    [Surname] nvarchar(100) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [IsActive] bit NOT NULL,
    [DateOfBirth] datetime2 NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_User_Email] ON [User] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251112152618_InitialSql', N'9.0.6');

ALTER TABLE [User] DROP CONSTRAINT [PK_User];

EXEC sp_rename N'[User]', N'Users', 'OBJECT';

EXEC sp_rename N'[Users].[IX_User_Email]', N'IX_Users_Email', 'INDEX';

ALTER TABLE [Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251112154253_RenameUserToUsers', N'9.0.6');

CREATE TABLE [ActivityLogs] (
    [Id] uniqueidentifier NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [Action] nvarchar(100) NOT NULL,
    [UserId] bigint NULL,
    [UserName] nvarchar(200) NULL,
    [PerformedBy] nvarchar(200) NULL,
    [Details] nvarchar(1000) NULL,
    CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_ActivityLogs_UserId] ON [ActivityLogs] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251115144633_AddActivityLogs', N'9.0.6');

COMMIT;
GO

