IF DB_ID('LakotelepUgyfelkezelo') IS NOT NULL
	DROP DATABASE LakotelepUgyfelkezelo;
GO

use master;
	
CREATE DATABASE LakotelepUgyfelkezelo;
GO
USE LakotelepUgyfelkezelo;
GO

CREATE TABLE Operator(
	Felhasznalonev VARCHAR(20) PRIMARY KEY,
	Jelszo VARCHAR(20) NOT NULL
);
GO

CREATE TABLE Lepcsohaz(
    Id INTEGER PRIMARY KEY IDENTITY(1,1),
    Iranyitoszam INTEGER NOT NULL,
    Varos VARCHAR(60) NOT NULL,
    Utca VARCHAR(60) NOT NULL,
    HazSzam VARCHAR(10) NOT NULL,
    LepcsohazKod VARCHAR(10) NOT NULL,
);
GO

CREATE TABLE Ugyfel (
    Id INTEGER PRIMARY KEY IDENTITY(1,1),
    Nev VARCHAR(120) NOT NULL,
    SzuletesiIdo Date,
    SzuletesiHely VARCHAR(60),
    Cegszam VARCHAR(60),
    AnyjaNeve VARCHAR(120),
    CegkepviseloNeve VARCHAR(120),
    LepcsohazId INTEGER NOT NULL,
    LakohelyEmelet INTEGER NOT NULL,
    LakohelyLakas VARCHAR(10) NOT NULL,
    SzamlazasiCim VARCHAR(120),
    Cegszekhely VARCHAR(120),
    Okmanyszam VARCHAR(60),
    Telefonszam VARCHAR(30),
    Bankszamlaszam VARCHAR(30),
    ElofizetoEgyeni TINYINT NOT NULL,
    IgenybevetelJogcime INTEGER NOT NULL,
    SzolgaltatasKezdoIdopontja Date,
    HatarozottIdejuSzolgaltatasVege Date,
    MegrendeltCsomagok INTEGER NOT NULL,
    DijbefizetesModja INTEGER NOT NULL,
    DifbefizetesUtemezese INTEGER NOT NULL,
    CONSTRAINT UgyfelToLepcsohaz 
        FOREIGN KEY (LepcsohazId) 
        REFERENCES Lepcsohaz (Id),
);
GO

INSERT INTO Operator VALUES('ch0kee','1tux9P');



GO
