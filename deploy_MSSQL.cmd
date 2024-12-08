set GeoNamesOrg__ConnectionStrings=Server=localhost;Database=GeoNames.org;Persist Security Info=True;User ID=sa;Password=<PASSWORD>;Connect Timeout=10;Encrypt=False;Trust Server Certificate=True;
set prj=.\src\Menchul.GeoNames.org\Menchul.GeoNames.org.csproj
set src=.\src\Menchul.GeoNames.org.MSSQL\Menchul.GeoNames.org.MSSQL.csproj
set pkj=Microsoft.EntityFrameworkCore.SqlServer

dotnet add "%prj%" package %pkj% --version 9.0.0 --framework NET9.0

dotnet ef migrations add InitialMigration -p %prj% -s %src% --framework NET9.0

dotnet ef database update -p "%prj%" -s "%src%" --framework NET9.0

del /F /S /Q ".\src\Menchul.GeoNames.org\Migrations"

dotnet remove "%prj%" package %pkj%
