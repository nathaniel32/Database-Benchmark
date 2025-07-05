
-- TVF for QueryBasic
--IF OBJECT_ID (N'dbo.GetBasicAuftragData', N'TF') IS NOT NULL
    DROP FUNCTION dbo.GetBasicAuftragData;
--GO

CREATE FUNCTION dbo.GetBasicAuftragData (@startDate DATE, @endDate DATE)
RETURNS TABLE
AS
RETURN
(
    SELECT a.Aufnr, a.AufDat, a.ErlDat, a.Dauer, m.MitName, k.KunName
    FROM Auftrag a
    LEFT JOIN Mitarbeiter m ON a.MitID = m.MitID
    JOIN Kunde k ON a.KunNr = k.KunNr
    WHERE a.AufDat >= @startDate AND a.AufDat < @endDate
);
GO

-- TVF for QueryAggregate
--IF OBJECT_ID (N'dbo.GetAggregateMitarbeiterData', N'TF') IS NOT NULL
    DROP FUNCTION dbo.GetAggregateMitarbeiterData;
--GO

CREATE FUNCTION dbo.GetAggregateMitarbeiterData ()
RETURNS TABLE
AS
RETURN
(
    SELECT m.MitID, m.MitName, SUM(a.Dauer) AS TotalDauer, SUM(mg.Anzahl) AS TotalMontage
    FROM Mitarbeiter m
    LEFT JOIN Auftrag a ON m.MitID = a.MitID
    LEFT JOIN Montage mg ON a.Aufnr = mg.AufNr
    GROUP BY m.MitID, m.MitName
);
GO

-- TVF for QueryDetail
--IF OBJECT_ID (N'dbo.GetDetailAuftragData', N'TF') IS NOT NULL
    DROP FUNCTION dbo.GetDetailAuftragData;
--GO

CREATE FUNCTION dbo.GetDetailAuftragData (@startDate DATE, @endDate DATE)
RETURNS TABLE
AS
RETURN
(
    SELECT a.Aufnr, a.AufDat, e.EtBezeichnung, mg.Anzahl, e.EtPreis, (mg.Anzahl * e.EtPreis) AS Gesamtpreis
    FROM Auftrag a
    JOIN Montage mg ON a.Aufnr = mg.AufNr
    JOIN Ersatzteil e ON mg.EtID = e.EtID
    WHERE a.AufDat BETWEEN @startDate AND @endDate
);
GO

-- TVF for QueryHeavyJoin
--IF OBJECT_ID (N'dbo.GetHeavyJoinData', N'TF') IS NOT NULL
    DROP FUNCTION dbo.GetHeavyJoinData;
--GO

CREATE FUNCTION dbo.GetHeavyJoinData (@startDate DATE, @endDate DATE)
RETURNS TABLE
AS
RETURN
(
    SELECT
        m.MitName,
        k.KunName,
        a.Aufnr,
        a.AufDat,
        e.EtBezeichnung,
        mg.Anzahl,
        (mg.Anzahl * e.EtPreis) AS Gesamtpreis
    FROM
        Montage mg
    JOIN
        Auftrag a ON mg.Aufnr = a.Aufnr
    JOIN
        Mitarbeiter m ON a.MitID = m.MitID
    JOIN
        Kunde k ON a.KunNr = k.KunNr
    JOIN
        Ersatzteil e ON mg.EtID = e.EtID
    WHERE
        a.AufDat BETWEEN @startDate AND @endDate
);
GO
