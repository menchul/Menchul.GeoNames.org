# Menchul.GeoNames.org

Entity Framework Core library and data import tool for geographical data from the [GeoNames.org](https://www.geonames.org/) portal.

## Target Frameworks
* **.NET 8.0 (LTS)**
* **.NET 10.0 (LTS)**

## Solution Structure

### Core Library
* **`Menchul.GeoNames.org`**: Core Entity Framework Core model, entities, configurations, and `DbContext` for GeoNames.org data.

### Database Providers
* **`Menchul.GeoNames.org.Firebird`**: EF Core migrations and `DbContext` implementation for Firebird SQL.
* **`Menchul.GeoNames.org.MSSQL`**: EF Core migrations and `DbContext` implementation for Microsoft SQL Server.
* **`Menchul.GeoNames.org.PostgreSQL`**: EF Core migrations and `DbContext` implementation for PostgreSQL (Npgsql).
* **`Menchul.GeoNames.org.SQLite`**: EF Core migrations and `DbContext` implementation for SQLite.

### Import Utility
* **`Menchul.Import.GeoNames.org`**: CLI application (`net10.0`) that automatically downloads dump files from GeoNames, extracts and parses them, and bulk-imports data into the target database.

## Notes

For PostgreSQL, you can safely ignore the initial migration history check error:
```text
Failed executing DbCommand (13ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";
```
