@echo off
set DOTNET_CLI_TELEMETRY_OPTOUT=false
set prj=..\..\..\src\Menchul.GeoNames.org.MSSQL\Menchul.GeoNames.org.MSSQL.csproj
set ctx=GeoNamesOrgMSSQLDbContext
set net=--framework NET10.0
set conStr=-- --connection "Server=localhost;Database=GeoNames.org;Persist Security Info=True;User ID=sa;Password=1qaz@WSX;Connect Timeout=10;Encrypt=False;Trust Server Certificate=True;"

@rem dotnet ef migrations remove -f --configuration Release --context %ctx% -p "%prj%" %net%
@rem dotnet ef migrations add InitialCommit --configuration Debug --context %ctx% -p "%prj%" %net%

@if errorlevel 1 (
    exit
)

@rem dotnet ef database drop -f --configuration Release --context %ctx% -p "%prj%" %net% %conStr%
@rem dotnet ef database update  --configuration Release --context %ctx% -p "%prj%" %net% %conStr%
