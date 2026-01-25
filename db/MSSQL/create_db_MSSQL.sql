
USE [master]
GO

CREATE DATABASE [GeoNames.org]
    ON     (NAME = N'GeoNames.org_DATA', FILENAME = N'/var/opt/mssql/data/GeoNames.org.mdf' )
    LOG ON (NAME = N'GeoNames.org_LOG', FILENAME = N'/var/opt/mssql/data/GeoNames.org.ldf');
GO
