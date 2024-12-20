set GeoNamesOrg__ConnectionStrings=Server=localhost;Database=GeoNames.org;Persist Security Info=True;User ID=sa;Password=<PASSWORD>;Connect Timeout=10;Encrypt=False;Trust Server Certificate=True;
set prj=.\src\Menchul.GeoNames.org\Menchul.GeoNames.org.csproj
set src=.\src\Menchul.GeoNames.org.MSSQL\Menchul.GeoNames.org.MSSQL.csproj
set pkj=Microsoft.EntityFrameworkCore.SqlServer
set net=--framework NET9.0

dotnet add "%prj%" package %pkj% --version 9.0.0 %net%

dotnet ef migrations add InitialMigration -p %prj% -s %src% %net%

dotnet ef database update -p "%prj%" -s "%src%" %net%

del /F /S /Q ".\src\Menchul.GeoNames.org\Migrations"

dotnet remove "%prj%" package %pkj%
