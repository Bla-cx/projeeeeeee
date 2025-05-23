-- Bu script Identity tablolarını string ID'den int ID'ye dönüştürür
-- DİKKAT: Bu işlem sonrası varsa tüm mevcut verileriniz silinir!
-- Üretim ortamında kullanmadan önce iyi test edin ve yedek alın!

USE etkinliklerydb;
GO

-- Önce mevcut ilişkili tabloları silmemiz gerekiyor
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserTokens')
BEGIN
    DROP TABLE AspNetUserTokens;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    DROP TABLE AspNetUserRoles;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserLogins')
BEGIN
    DROP TABLE AspNetUserLogins;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserClaims')
BEGIN
    DROP TABLE AspNetUserClaims;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoleClaims')
BEGIN
    DROP TABLE AspNetRoleClaims;
END

-- Ana tabloları siliyoruz
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    DROP TABLE AspNetUsers;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    DROP TABLE AspNetRoles;
END

-- Şimdi INT kimliklerle yeniden oluşturuyoruz

-- AspNetRoles (Identity roles) - INT ID
CREATE TABLE AspNetRoles (
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);

-- AspNetUsers (Identity users) - INT ID
CREATE TABLE AspNetUsers (
    Id INT IDENTITY(1,1) NOT NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    -- Custom fields for AppUser
    Ad NVARCHAR(50) NOT NULL DEFAULT (''),
    Soyad NVARCHAR(50) NOT NULL DEFAULT (''),
    Telefon NVARCHAR(20) NULL,
    ProfilResimYolu NVARCHAR(255) NULL,
    KayitTarihi DATETIME NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
);

-- AspNetRoleClaims (Role claims) - INT ID
CREATE TABLE AspNetRoleClaims (
    Id INT IDENTITY(1,1) NOT NULL,
    RoleId INT NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);

-- AspNetUserClaims (User claims) - INT ID
CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) NOT NULL,
    UserId INT NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

-- AspNetUserLogins (User external logins) - INT ID
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId INT NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

-- AspNetUserRoles (User role assignments) - INT ID
CREATE TABLE AspNetUserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

-- AspNetUserTokens (User tokens) - INT ID
CREATE TABLE AspNetUserTokens (
    UserId INT NOT NULL,
    LoginProvider NVARCHAR(128) NOT NULL,
    Name NVARCHAR(128) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

-- Add indexes for better query performance
CREATE INDEX IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims (RoleId);

CREATE UNIQUE INDEX IX_AspNetRoles_NormalizedName ON AspNetRoles (NormalizedName) WHERE NormalizedName IS NOT NULL;

CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims (UserId);

CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins (UserId);

CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles (RoleId);

CREATE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers (NormalizedEmail) WHERE NormalizedEmail IS NOT NULL;

CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
