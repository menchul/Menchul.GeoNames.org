set GeoNamesOrg__ConnectionStrings=Host=localhost;Port=5432;Database=GeoNames.org;Username=postgres;Password=<PASSWORD>;CommandTimeout=10;
set prj=.\src\Menchul.GeoNames.org\Menchul.GeoNames.org.csproj
set src=.\src\Menchul.GeoNames.org.PostgreSQL\Menchul.GeoNames.org.PostgreSQL.csproj
set pkj=Npgsql.EntityFrameworkCore.PostgreSQL

dotnet add "%prj%" package %pkj% --version 9.0.2 --framework NET9.0

dotnet ef migrations add InitialMigration -p %prj% -s %src% --framework NET9.0

dotnet ef database update -p "%prj%" -s "%src%" --framework NET9.0

del /F /S /Q ".\src\Menchul.GeoNames.org\Migrations"

dotnet remove "%prj%" package %pkj%
