INSERT INTO dbo.DistrictSecondarySalesperson VALUES (3, 6, 6);

INSERT INTO dbo.DistrictSecondarySalesperson VALUES (1, 4, 2);

UPDATE dbo.District SET PrimarySalespersonId = 2 WHERE DistrictId = 1;

UPDATE dbo.District SET PrimarySalespersonId = 4 WHERE DistrictId = 1;

DELETE FROM dbo.Salesperson WHERE SalespersonId = 1;

INSERT INTO dbo.Salesperson (Name) VALUES (N'   ');

INSERT INTO dbo.District (Name, PrimarySalespersonId) VALUES (N'Nordjylland', 4);