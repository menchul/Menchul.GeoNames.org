@echo off
set DOTNET_CLI_TELEMETRY_OPTOUT=false
set prj=..\..\..\src\Menchul.GeoNames.org.Firebird\Menchul.GeoNames.org.Firebird.csproj
set ctx=GeoNamesOrgFirebirdSqlDbContext
set net=--framework net10.0
set conStr=-- --connection "DataSource=localhost;Port=3050;Database=/var/lib/firebird/data/GeoNames.org.fdb;User=SYSDBA;Password=masterkey;Dialect=3;Charset=UTF8;"

@rem dotnet ef migrations remove -f --configuration Release --context %ctx% -p "%prj%" %net%
@rem dotnet ef migrations add InitialCommit --configuration Debug --context %ctx% -p "%prj%" %net%

@if errorlevel 1 (
    exit
)

@rem dotnet ef database drop -f --configuration Release --context %ctx% -p "%prj%" %net% %conStr%
@rem dotnet ef database update  --configuration Release --context %ctx% -p "%prj%" %net% %conStr%
