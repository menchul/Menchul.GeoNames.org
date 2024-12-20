set GeoNamesOrg__ConnectionStrings=Host=localhost;Port=5433;Database=GeoNames.org;Username=postgres;Password=<PASSWORD>;CommandTimeout=10;
set prj=.\src\Menchul.GeoNames.org\Menchul.GeoNames.org.csproj
set src=.\src\Menchul.GeoNames.org.PostgreSQL\Menchul.GeoNames.org.PostgreSQL.csproj
set pkj=Npgsql.EntityFrameworkCore.PostgreSQL
set net=--framework NET9.0

dotnet add "%prj%" package %pkj% --version 9.0.2 %net%

dotnet ef migrations add InitialMigration -p %prj% -s %src% %net%

dotnet ef database update -p "%prj%" -s "%src%" %net%

del /F /S /Q ".\src\Menchul.GeoNames.org\Migrations"

dotnet remove "%prj%" package %pkj%
