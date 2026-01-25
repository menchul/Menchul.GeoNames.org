@echo off
set DOTNET_CLI_TELEMETRY_OPTOUT=false
set prj=..\..\..\src\Menchul.GeoNames.org.PostgreSQL\Menchul.GeoNames.org.PostgreSQL.csproj
set ctx=GeoNamesOrgPostgreSQLDbContext
set net=--framework NET10.0
set conStr=-- --connection "Host=localhost;Port=5433;Database=GeoNames.org;Username=postgres;Password=1qaz@WSX;CommandTimeout=99;Keepalive=1;Include Error Detail=true;Search Path=gno,public;"

@rem dotnet ef migrations remove -f --configuration Release --context %ctx% -p "%prj%" %net%
@rem dotnet ef migrations add InitialCommit --configuration Debug --context %ctx% -p "%prj%" %net%

@if errorlevel 1 (
    exit
)

@rem dotnet ef database drop -f --configuration Release --context %ctx% -p "%prj%" %net% %conStr%
@rem dotnet ef database update  --configuration Release --context %ctx% -p "%prj%" %net% %conStr%
