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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [Ad] nvarchar(50) NOT NULL DEFAULT N'',
        [Soyad] nvarchar(50) NOT NULL DEFAULT N'',
        [Telefon] nvarchar(20) NULL,
        [ProfilResimYolu] nvarchar(255) NULL,
        [KayitTarihi] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Kategoriler] (
        [KategoriId] int NOT NULL IDENTITY,
        [KategoriAdi] nvarchar(100) NOT NULL,
        [Aciklama] nvarchar(500) NULL,
        CONSTRAINT [PK_Kategoriler] PRIMARY KEY ([KategoriId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Sehirler] (
        [SehirId] int NOT NULL IDENTITY,
        [SehirAdi] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Sehirler] PRIMARY KEY ([SehirId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Bildirimler] (
        [BildirimId] int NOT NULL IDENTITY,
        [KullaniciId] int NOT NULL,
        [Mesaj] nvarchar(500) NOT NULL,
        [Tarih] datetime2 NOT NULL,
        [Okundu] bit NOT NULL,
        CONSTRAINT [PK_Bildirimler] PRIMARY KEY ([BildirimId]),
        CONSTRAINT [FK_Bildirimler_AspNetUsers_KullaniciId] FOREIGN KEY ([KullaniciId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Loglar] (
        [LogId] int NOT NULL IDENTITY,
        [KullaniciId] int NULL,
        [IslemTipi] nvarchar(50) NOT NULL,
        [Aciklama] nvarchar(500) NULL,
        [IP] nvarchar(50) NULL,
        [Tarih] datetime2 NOT NULL,
        CONSTRAINT [PK_Loglar] PRIMARY KEY ([LogId]),
        CONSTRAINT [FK_Loglar_AspNetUsers_KullaniciId] FOREIGN KEY ([KullaniciId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Etkinlikler] (
        [EtkinlikId] int NOT NULL IDENTITY,
        [EtkinlikAdi] nvarchar(200) NOT NULL,
        [Aciklama] nvarchar(max) NULL,
        [Tarih] datetime2 NOT NULL,
        [BaslangicSaati] time NULL,
        [BitisSaati] time NULL,
        [KategoriId] int NOT NULL,
        [SehirId] int NOT NULL,
        [Adres] nvarchar(500) NOT NULL,
        [ResimYolu] nvarchar(255) NULL,
        [OrganizatorId] int NOT NULL,
        [BiletFiyati] decimal(10,2) NOT NULL,
        [ToplamKapasite] int NOT NULL,
        [KalanKapasite] int NOT NULL,
        [OlusturulmaTarihi] datetime2 NOT NULL,
        [AktifMi] bit NOT NULL,
        CONSTRAINT [PK_Etkinlikler] PRIMARY KEY ([EtkinlikId]),
        CONSTRAINT [FK_Etkinlikler_AspNetUsers_OrganizatorId] FOREIGN KEY ([OrganizatorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Etkinlikler_Kategoriler_KategoriId] FOREIGN KEY ([KategoriId]) REFERENCES [Kategoriler] ([KategoriId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Etkinlikler_Sehirler_SehirId] FOREIGN KEY ([SehirId]) REFERENCES [Sehirler] ([SehirId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Rezervasyonlar] (
        [RezervasyonId] int NOT NULL IDENTITY,
        [EtkinlikId] int NOT NULL,
        [KullaniciId] int NOT NULL,
        [BiletSayisi] int NOT NULL,
        [ToplamTutar] decimal(10,2) NOT NULL,
        [RezervasyonTarihi] datetime2 NOT NULL,
        [OdemeDurumu] bit NOT NULL,
        [BarkodNo] nvarchar(50) NULL,
        CONSTRAINT [PK_Rezervasyonlar] PRIMARY KEY ([RezervasyonId]),
        CONSTRAINT [FK_Rezervasyonlar_AspNetUsers_KullaniciId] FOREIGN KEY ([KullaniciId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Rezervasyonlar_Etkinlikler_EtkinlikId] FOREIGN KEY ([EtkinlikId]) REFERENCES [Etkinlikler] ([EtkinlikId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE TABLE [Sepet] (
        [SepetId] int NOT NULL IDENTITY,
        [KullaniciId] int NOT NULL,
        [EtkinlikId] int NOT NULL,
        [BiletSayisi] int NOT NULL,
        [EklenmeTarihi] datetime2 NOT NULL,
        CONSTRAINT [PK_Sepet] PRIMARY KEY ([SepetId]),
        CONSTRAINT [FK_Sepet_AspNetUsers_KullaniciId] FOREIGN KEY ([KullaniciId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Sepet_Etkinlikler_EtkinlikId] FOREIGN KEY ([EtkinlikId]) REFERENCES [Etkinlikler] ([EtkinlikId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Bildirimler_KullaniciId] ON [Bildirimler] ([KullaniciId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Etkinlikler_KategoriId] ON [Etkinlikler] ([KategoriId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Etkinlikler_OrganizatorId] ON [Etkinlikler] ([OrganizatorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Etkinlikler_SehirId] ON [Etkinlikler] ([SehirId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Kategoriler_KategoriAdi] ON [Kategoriler] ([KategoriAdi]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Loglar_KullaniciId] ON [Loglar] ([KullaniciId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Rezervasyonlar_BarkodNo] ON [Rezervasyonlar] ([BarkodNo]) WHERE [BarkodNo] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Rezervasyonlar_EtkinlikId] ON [Rezervasyonlar] ([EtkinlikId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Rezervasyonlar_KullaniciId] ON [Rezervasyonlar] ([KullaniciId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Sehirler_SehirAdi] ON [Sehirler] ([SehirAdi]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Sepet_EtkinlikId] ON [Sepet] ([EtkinlikId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    CREATE INDEX [IX_Sepet_KullaniciId] ON [Sepet] ([KullaniciId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521014532_InitialMigrationWithIntIds'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250521014532_InitialMigrationWithIntIds', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521180530_AddIptalAndKullanildiColumns'
)
BEGIN
    ALTER TABLE [Rezervasyonlar] ADD [Iptal] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521180530_AddIptalAndKullanildiColumns'
)
BEGIN
    ALTER TABLE [Rezervasyonlar] ADD [Kullanildi] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250521180530_AddIptalAndKullanildiColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250521180530_AddIptalAndKullanildiColumns', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250522015531_FixIdentityIntKeys'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Telefon');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [AspNetUsers] SET [Telefon] = N'''' WHERE [Telefon] IS NULL');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [Telefon] nvarchar(20) NOT NULL;
    ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [Telefon];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250522015531_FixIdentityIntKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250522015531_FixIdentityIntKeys', N'8.0.0');
END;
GO

COMMIT;
GO

