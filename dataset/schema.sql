CREATE TABLE Mitarbeiter (
	MitID char(3) PRIMARY KEY,
	MitName varchar(20) NOT NULL,
	MitVorname varchar(20) NULL,
	MitGebDat date NOT NULL,
	MitJob varchar(20) NOT NULL,
	MitStundensatz smallmoney NULL,
    MitEinsatzort varchar(20) NULL
);

CREATE TABLE Kunde (
	KunNr int PRIMARY KEY,
	KunName varchar(20) NOT NULL,
	KunOrt varchar(20) NOT NULL,
	KunPLZ char(5) NOT NULL,
	KunStrasse varchar(20) NOT NULL
);

CREATE TABLE Ersatzteil (
	EtID char(5) PRIMARY KEY,
	EtBezeichnung varchar(100) NOT NULL,
	EtPreis smallmoney NOT NULL,
	EtAnzLager int NOT NULL,
	EtHersteller varchar(30) NOT NULL
);

CREATE TABLE Auftrag (
	Aufnr int PRIMARY KEY,
	MitID char(3) NULL,
	KunNr int NOT NULL,
	AufDat date NOT NULL,
	ErlDat date DEFAULT getdate() NULL,
	Dauer decimal(5,1) NULL,
	Anfahrt int NULL,
	Beschreibung varchar(200) COLLATE Latin1_General_CI_AS NULL,
	CONSTRAINT FK__Auftrag__Kunde__KunNr FOREIGN KEY (KunNr) REFERENCES Kunde(KunNr),
	CONSTRAINT FK__Auftrag__Mitarbeiter__MitID FOREIGN KEY (MitID) REFERENCES Mitarbeiter(MitID)
);

CREATE TABLE Montage (
	EtID char(5) NOT NULL,
	AufNr int NOT NULL,
	Anzahl int NOT NULL,
	CONSTRAINT PK__Montage PRIMARY KEY (EtID,AufNr),
	CONSTRAINT FK__Montage__Auftrag__AufNr FOREIGN KEY (AufNr) REFERENCES Auftrag(Aufnr),
	CONSTRAINT FK__Montage__Ersatzteil__EtID FOREIGN KEY (EtID) REFERENCES Ersatzteil(EtID)
);