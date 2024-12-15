# GeoNames.org
This is project, that automaticaly download data from Geonames.org (flat TXT files) and inmport them to MS SQL Server database. There are 2 projects:
1. Database.GeoNames.org - this is standard MS SQL Server Database project, that helps you to create new databes for storing data. Tables was created to equals o original TXT files: 1 TXT file from GeoNames.org = 1 table in databes. Field names and types maximum related to them. Also there is no indexes or other optimizations. Main idea was is to much easy import data to MS SQL Server format.
2. Import.GeoNames.org - is a .NET consola application, that download TXT files form http://download.geonames.org/export/dump/ Save them to temporary folder and import to same table in MS SQL Server database.

For the PostgreSQL please ignore error:

Failed executing DbCommand (13ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";

