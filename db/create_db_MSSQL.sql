
USE [master]
GO

CREATE DATABASE [GeoNames.org]
    ON     (NAME = N'GeoNames.org_DATA', FILENAME = N'/tmp/db/.MSSQL/GeoNames.org.mdf' )
    LOG ON (NAME = N'GeoNames.org_LOG', FILENAME = N'/tmp/db/.MSSQL/GeoNames.org.ldf');
GO
