--1)
--EXEC sp_configure filestream_access_level, 2
--RECONFIGURE

--)2
--exec sp_fulltext_service 'load_os_resources', 1


--3)
      /****** Object:  Database [NorthPole]    Script Date: 08/05/2010 12:50:27 ******/

--CREATE DATABASE [NorthPole] ON  PRIMARY

--( NAME = N'NorthPoleDB', FILENAME = N'c:\Program files\microsoft sql server\mssql.1\mssql\data\NorthPoleDB.mdf' , SIZE = 2304KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ),

-- FILEGROUP [NorthPoleFS] CONTAINS FILESTREAM  DEFAULT

--( NAME = N'NorthPoleFS', FILENAME = N'c:\Program files\microsoft sql server\mssql.1\mssql\data\NorthPoleFS' )

-- LOG ON

--( NAME = N'NorthPoleLOG', FILENAME = N'c:\Program files\microsoft sql server\mssql.1\mssql\data\NorthPoleLOG.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)

--GO

 

--ALTER DATABASE [NorthPole] SET COMPATIBILITY_LEVEL = 100

--GO

 

--IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))

--begin

--EXEC [NorthPole].[dbo].[sp_fulltext_database] @action = 'enable'

--end

--GO

 

--ALTER DATABASE [NorthPole] SET ANSI_NULL_DEFAULT OFF

--GO

 

--ALTER DATABASE [NorthPole] SET ANSI_NULLS OFF

--GO

 

--ALTER DATABASE [NorthPole] SET ANSI_PADDING OFF

--GO

 

--ALTER DATABASE [NorthPole] SET ANSI_WARNINGS OFF

--GO

 

--ALTER DATABASE [NorthPole] SET ARITHABORT OFF

--GO

 

--ALTER DATABASE [NorthPole] SET AUTO_CLOSE OFF

--GO

 

--ALTER DATABASE [NorthPole] SET AUTO_CREATE_STATISTICS ON

--GO

 

--ALTER DATABASE [NorthPole] SET AUTO_SHRINK OFF

--GO

 

--ALTER DATABASE [NorthPole] SET AUTO_UPDATE_STATISTICS ON

--GO

 

--ALTER DATABASE [NorthPole] SET CURSOR_CLOSE_ON_COMMIT OFF

--GO

 

--ALTER DATABASE [NorthPole] SET CURSOR_DEFAULT  GLOBAL

--GO

 

--ALTER DATABASE [NorthPole] SET CONCAT_NULL_YIELDS_NULL OFF

--GO

 

--ALTER DATABASE [NorthPole] SET NUMERIC_ROUNDABORT OFF

--GO

 

--ALTER DATABASE [NorthPole] SET QUOTED_IDENTIFIER OFF

--GO

 

--ALTER DATABASE [NorthPole] SET RECURSIVE_TRIGGERS OFF

--GO

 

--ALTER DATABASE [NorthPole] SET  ENABLE_BROKER

--GO

 

--ALTER DATABASE [NorthPole] SET AUTO_UPDATE_STATISTICS_ASYNC OFF

--GO

 

--ALTER DATABASE [NorthPole] SET DATE_CORRELATION_OPTIMIZATION OFF

--GO

 

--ALTER DATABASE [NorthPole] SET TRUSTWORTHY OFF

--GO

 

--ALTER DATABASE [NorthPole] SET ALLOW_SNAPSHOT_ISOLATION OFF

--GO

 

--ALTER DATABASE [NorthPole] SET PARAMETERIZATION SIMPLE

--GO

 

--ALTER DATABASE [NorthPole] SET READ_COMMITTED_SNAPSHOT OFF

--GO

 

--ALTER DATABASE [NorthPole] SET HONOR_BROKER_PRIORITY OFF

--GO

 

--ALTER DATABASE [NorthPole] SET  READ_WRITE

--GO

 

--ALTER DATABASE [NorthPole] SET RECOVERY FULL

--GO

 

--ALTER DATABASE [NorthPole] SET  MULTI_USER

--GO

 

--ALTER DATABASE [NorthPole] SET PAGE_VERIFY CHECKSUM 

--GO

 

--ALTER DATABASE [NorthPole] SET DB_CHAINING OFF

--GO


--4) 

--CREATE TABLE DocumentRepository(

--  ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY

--, FileStreamID UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID()

--, DocumentExtension VARCHAR(10)

--, DocumentName VARCHAR(256)

--, Document VARBINARY(MAX) FILESTREAM DEFAULT(0x)

--);


--5)
--\\.psf\Home\Documents
--select COUNT(*) 
--delete
--from documentrepository

INSERT INTO DocumentRepository(DocumentExtension, DocumentName, Document)

SELECT

 'docx' AS DocumentExtension

 , 'Jerry Hinkle.docx' AS DocumentName

 , * FROM OPENROWSET(BULK '\\.psf\Home\Documents\Jerry Hinkle.docx', SINGLE_BLOB)

   AS Document;


INSERT INTO DocumentRepository(DocumentExtension, DocumentName, Document)

SELECT

 'docx' AS DocumentExtension

 , 'Todd Langdon.docx' AS DocumentName

 , * FROM OPENROWSET(BULK '\\.psf\Home\Documents\Todd Langdon.docx', SINGLE_BLOB)

   AS Document;

INSERT INTO DocumentRepository(DocumentExtension, DocumentName, Document)

SELECT

 'docx' AS DocumentExtension

 , 'Eddie would go.docx' AS DocumentName

 , * FROM OPENROWSET(BULK 'c:\dev\Eddie would go.docx', SINGLE_BLOB)

   AS Document;



--6)

CREATE FULLTEXT CATALOG FileStreamFTSCatalog AS DEFAULT;


--7)
     CREATE FULLTEXT INDEX ON dbo.DocumentRepository

(DocumentName, Document TYPE COLUMN DocumentExtension)

KEY INDEX DocumentRepository_PK

ON FileStreamFTSCatalog

WITH CHANGE_TRACKING AUTO;


--8)
SELECT ID, DocumentName, Document

FROM dbo.DocumentRepository

WHERE CONTAINS(Document, 'Eddie');


