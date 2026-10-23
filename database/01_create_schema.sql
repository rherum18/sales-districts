IF DB_ID(N'SalesDistrict') IS NULL
    CREATE DATABASE SalesDistrict;
GO
 
USE SalesDistrict;
GO
 
-- Drop in reverse dependency order so the script can be re-run
DROP TABLE IF EXISTS dbo.Store;
DROP TABLE IF EXISTS dbo.DistrictSecondarySalesperson;
DROP TABLE IF EXISTS dbo.District;
DROP TABLE IF EXISTS dbo.Salesperson;
GO
 
CREATE TABLE dbo.Salesperson
(
    SalespersonId INT IDENTITY(1, 1) NOT NULL,
    Name          NVARCHAR(100)      NOT NULL,
 
    CONSTRAINT PK_Salesperson PRIMARY KEY (SalespersonId),
    CONSTRAINT CK_Salesperson_Name_NotBlank CHECK (LEN(TRIM(Name)) > 0)
    -- Names are deliberately not unique: two people can share a name.
);
GO
 
CREATE TABLE dbo.District
(
    DistrictId           INT IDENTITY(1, 1) NOT NULL,
    Name                 NVARCHAR(100)      NOT NULL,
    PrimarySalespersonId INT                NOT NULL,
 
    CONSTRAINT PK_District PRIMARY KEY (DistrictId),
    CONSTRAINT UQ_District_Name UNIQUE (Name),
    CONSTRAINT CK_District_Name_NotBlank CHECK (LEN(TRIM(Name)) > 0),
    CONSTRAINT FK_District_PrimarySalesperson
        FOREIGN KEY (PrimarySalespersonId) REFERENCES dbo.Salesperson (SalespersonId),
 
    -- Lets DistrictSecondarySalesperson reference the pair (district, its primary)
    CONSTRAINT UQ_District_DistrictId_Primary UNIQUE (DistrictId, PrimarySalespersonId)
);
GO
 
CREATE TABLE dbo.DistrictSecondarySalesperson
(
    DistrictId           INT NOT NULL,
    SalespersonId        INT NOT NULL,
    -- Copy of the district's current primary salesperson, kept in sync by the
    -- cascading foreign key below, so the CHECK constraint can compare against it
    PrimarySalespersonId INT NOT NULL,
 
    CONSTRAINT PK_DistrictSecondarySalesperson PRIMARY KEY (DistrictId, SalespersonId),
    CONSTRAINT FK_DSS_Salesperson
        FOREIGN KEY (SalespersonId) REFERENCES dbo.Salesperson (SalespersonId),
    CONSTRAINT FK_DSS_District
        FOREIGN KEY (DistrictId, PrimarySalespersonId)
        REFERENCES dbo.District (DistrictId, PrimarySalespersonId)
        ON UPDATE CASCADE,
    -- If a district's primary is changed to someone who is already secondary there,
    -- the cascaded update violates this check and the whole update is rejected
    CONSTRAINT CK_DSS_NotPrimary CHECK (SalespersonId <> PrimarySalespersonId)
);
GO
 
CREATE TABLE dbo.Store
(
    StoreId    INT IDENTITY(1, 1) NOT NULL,
    Name       NVARCHAR(100)      NOT NULL,
    DistrictId INT                NOT NULL,
 
    CONSTRAINT PK_Store PRIMARY KEY (StoreId),
    CONSTRAINT CK_Store_Name_NotBlank CHECK (LEN(TRIM(Name)) > 0),
    CONSTRAINT FK_Store_District
        FOREIGN KEY (DistrictId) REFERENCES dbo.District (DistrictId)
);
GO
 
-- SQL Server does not index foreign keys automatically.
-- These support lookups such as "all districts for a salesperson" and "all stores in a district".
CREATE INDEX IX_District_PrimarySalespersonId ON dbo.District (PrimarySalespersonId);
CREATE INDEX IX_DSS_SalespersonId             ON dbo.DistrictSecondarySalesperson (SalespersonId);
CREATE INDEX IX_Store_DistrictId              ON dbo.Store (DistrictId);
GO