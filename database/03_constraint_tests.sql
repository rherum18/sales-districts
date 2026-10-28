-- Manual verification of the business rules enforced by the 01_create_schema
-- Run after 01_create_schema.sql and 02_seed_data.sql
USE SalesDistrict;
GO

SET XACT_ABORT ON;
GO

-- Test 1: A district's primary salesperson cannot also be a secondary in that district
-- Expected: CHECK constraint "CK_DSS_NotPrimary"
BEGIN TRANSACTION;
INSERT INTO dbo.DistrictSecondarySalesperson (DistrictId, SalespersonId, PrimarySalespersonId)
VALUES (3, 6, 6);
ROLLBACK TRANSACTION;
GO

-- Test 2: The copied primary on a secondary row must match the district's actual primary
-- Expected: FOREIGN KEY constraint "FK_DSS_District"
BEGIN TRANSACTION;
INSERT INTO dbo.DistrictSecondarySalesperson (DistrictId, SalespersonId, PrimarySalespersonId)
VALUES (1, 4, 2);
ROLLBACK TRANSACTION;
GO

-- Test 3: A district's current secondary cannot be made its primary
-- Expected: CHECK constraint "CK_DSS_NotPrimary" (raised by the cascaded update)
BEGIN TRANSACTION;
UPDATE dbo.District SET PrimarySalespersonId = 2 WHERE DistrictId = 1;
ROLLBACK TRANSACTION;
GO

-- Test 4: Changing a district's primary cascades to its secondary rows
-- Expected: row updated with PrimarySalespersonId = 4
BEGIN TRANSACTION;
UPDATE dbo.District SET PrimarySalespersonId = 4 WHERE DistrictId = 1;

SELECT DistrictId, SalespersonId, PrimarySalespersonId
FROM dbo.DistrictSecondarySalesperson
WHERE DistrictId = 1;
ROLLBACK TRANSACTION;
GO

-- Test 5: A salesperson who is primary in a district cannot be deleted
-- Expected: REFERENCE constraint "FK_District_PrimarySalesperson"
BEGIN TRANSACTION;
DELETE FROM dbo.Salesperson WHERE SalespersonId = 6;
ROLLBACK TRANSACTION;
GO

-- Test 6: A salesperson who is secondary in a district cannot be deleted
-- Expected: REFERENCE constraint "FK_DSS_Salesperson"
BEGIN TRANSACTION;
DELETE FROM dbo.Salesperson WHERE SalespersonId = 2;
ROLLBACK TRANSACTION;
GO

-- Test 7: A salesperson's name cannot be blank
-- Expected: CHECK constraint "CK_Salesperson_Name_NotBlank"
BEGIN TRANSACTION;
INSERT INTO dbo.Salesperson (Name) VALUES (N'   ');
ROLLBACK TRANSACTION;
GO

-- Test 8: District names are unique
-- Expected: UNIQUE KEY constraint "UQ_District_Name"
BEGIN TRANSACTION;
INSERT INTO dbo.District (Name, PrimarySalespersonId) VALUES (N'Nordjylland', 4);
ROLLBACK TRANSACTION;
GO

-- Test 9: Store names are unique within a district
-- Expected: UNIQUE KEY constraint "UQ_Store_District_Name"
BEGIN TRANSACTION;
INSERT INTO dbo.Store (Name, DistrictId) VALUES (N'Aalborg', 1);
ROLLBACK TRANSACTION;
GO

-- Test 10: The same store name is allowed in a different district
-- Expected: 1 row updated with store name = Aalborg
BEGIN TRANSACTION;
INSERT INTO dbo.Store (Name, DistrictId) VALUES (N'Aalborg', 2);
ROLLBACK TRANSACTION;
GO

-- Final check: nothing was committed and no transaction is left open
-- Expected: 6 salespersons, 5 secondary rows, 10 stores, district 1 primary = 1, 0 open transactions
SELECT
    (SELECT COUNT(*) FROM dbo.Salesperson)                  AS Salespersons,
    (SELECT COUNT(*) FROM dbo.DistrictSecondarySalesperson) AS SecondaryRows,
    (SELECT COUNT(*) FROM dbo.Store)                        AS Stores,
    (SELECT PrimarySalespersonId FROM dbo.District WHERE DistrictId = 1) AS District1Primary,
    @@TRANCOUNT                                             AS OpenTransactions;
GO
