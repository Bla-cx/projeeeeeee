USE etkinliklerydb;
GO

-- Önce tüm foreign key kısıtlamalarını silelim
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'
ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.foreign_keys;

EXEC sp_executesql @sql;

-- Şimdi Identity tablolarını sırayla silelim
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

-- Diğer tüm tabloları temizle - sys.tables kullanarak
DECLARE @tableName nvarchar(256)
DECLARE @dropStatement nvarchar(512)
DECLARE tableCursor CURSOR FOR
    SELECT name
    FROM sys.tables
    WHERE type = 'U' -- Sadece user tablolarını al
    AND name NOT IN ('__EFMigrationsHistory', 'AspNetRoles', 'AspNetUsers',
                     'AspNetUserTokens', 'AspNetUserRoles', 'AspNetUserLogins',
                     'AspNetUserClaims', 'AspNetRoleClaims') -- Migration geçmişini ve zaten silmeye çalıştığımız tabloları koruyalım

OPEN tableCursor
FETCH NEXT FROM tableCursor INTO @tableName
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @dropStatement = 'DROP TABLE [' + @tableName + ']'
    BEGIN TRY
        EXEC sp_executesql @dropStatement
    END TRY
    BEGIN CATCH
        PRINT 'Could not drop ' + @tableName
    END CATCH
    FETCH NEXT FROM tableCursor INTO @tableName
END

CLOSE tableCursor
DEALLOCATE tableCursor

-- Ana tabloları siliyoruz
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    DROP TABLE AspNetUsers;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    DROP TABLE AspNetRoles;
END 