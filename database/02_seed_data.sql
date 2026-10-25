-- Inserts data into the tables created by 01_create_schema.sql
USE SalesDistrict; 
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DELETE FROM dbo.Store;
DELETE FROM dbo.DistrictSecondarySalesperson;
DELETE FROM dbo.District;
DELETE FROM dbo.Salesperson;

SET IDENTITY_INSERT dbo.Salesperson ON;

INSERT INTO dbo.Salesperson (SalespersonId, Name)
VALUES
	(1, N'Anna Jensen'),
	(2, N'Bo Sørensen'),
	(3, N'Anna Jensen'),
	(4, N'Kasper Sørensen'),
	(5, N'Lars Jespersen'),
	(6, N'Morten Klausen');

SET IDENTITY_INSERT dbo.Salesperson OFF;

SET IDENTITY_INSERT dbo.District ON;

INSERT INTO dbo.District (DistrictId, Name, PrimarySalespersonId)
VALUES 
	(1, N'Nordjylland', 1),
	(2, N'Midtjylland', 5),
	(3, N'Sønderjylland', 6),
	(4, N'Sjælland', 3);

SET IDENTITY_INSERT dbo.District OFF;

INSERT INTO dbo.DistrictSecondarySalesperson (DistrictId, SalespersonId, PrimarySalespersonId)
VALUES
	(1, 2, 1),
	(2, 2, 5),
	(4, 5, 3),
	(4, 2, 3),
	(4, 1, 3);

SET IDENTITY_INSERT dbo.Store ON;

INSERT INTO dbo.Store (StoreId, Name, DistrictId)
VALUES
	(1, N'Aalborg', 1),
	(2, N'Hjørring', 1),
	(3, N'Frederikshavn', 1),
	(4, N'Aarhus', 2),
	(5, N'Horsens', 2),
	(6, N'Roskilde', 4),
	(7, N'København', 4),
	(8, N'Næstved', 4),
	(9, N'Slagelse', 4),
	(10, N'Kalundborg', 4);

SET IDENTITY_INSERT dbo.Store OFF;

COMMIT;
GO