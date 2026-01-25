@echo off
setlocal EnableExtensions DisableDelayedExpansion

set DOCKER_CLI_HINTS=false

echo [INFO] Starting migration: Windows -> Docker Volume
set container=GeoNamesOrg_ms_sql_2022_latest
set db_folder=%~dp0
set db_root=%~dp0..
set data_dir=%db_root%\.data

echo [INFO] Checking Windows data directory...

if not exist "%data_dir%" (
    echo [INFO] Creating directory: %data_dir%
    mkdir "%data_dir%"
) else (
    echo [INFO] Directory exists
)

echo [INFO] Using data directory: %data_dir%

echo [INFO] Using hardcoded connection settings...
set dbServer=localhost
set dbPort=1433
set dbName=GeoNames.org
set dbUser=sa
set dbPass=1qaz@WSX

if defined dbServer (
    for /f "tokens=1,2 delims=," %%H in ("%dbServer%") do (
        if not "%%~H"=="" set "dbHost=%%~H"
        if not "%%~I"=="" set "dbPort=%%~I"
    )
)

if not defined dbPass (
    echo [ERROR] Password not found in connection string
    exit /b 1
)
if not defined dbUser set "dbUser=sa"

echo [INFO] Parsed:
echo        HOST=%dbHost%
echo        PORT=%dbPort%
echo        DB=%dbName%
echo        USER=%dbUser%

echo [INFO] Removing existing container if any...
docker rm -f %container% >nul 2>&1

echo [INFO] Starting MSSQL container...
docker run -d ^
  --name %container% ^
  -e "ACCEPT_EULA=Y" ^
  -e "MSSQL_SA_PASSWORD=%dbPass%" ^
  -p %dbPort%:1433 ^
  -v "%db_root%:/tmp/db" ^
  -v "%data_dir%:/var/opt/mssql/data" ^
  mcr.microsoft.com/mssql/server:2022-latest >nul

if ERRORLEVEL 1 (
    echo [ERROR] Failed to start container
    exit /b 1
)

echo [INFO] Waiting for MSSQL...
set ready=
set /a max=30
for /l %%i in (1,1,%max%) do (
    docker exec %container% /opt/mssql-tools18/bin/sqlcmd -S localhost -C -U "%dbUser%" -P "%dbPass%" -Q "select 1" >nul 2>&1
    if not errorlevel 1 (
        set "ready=1"
        echo [INFO] MSSQL ready
        goto :after_wait
    )
    echo [INFO] Attempt %%i/%max%...
    timeout /t 2 >nul
)
:after_wait
if not defined ready (
    echo [ERROR] MSSQL did not start
    exit /b 1
)

echo [INFO] Checking data directory for existing database files...
set volume_has_data=
if exist "%data_dir%\\GeoNames.org.mdf" set "volume_has_data=1"

if defined volume_has_data (
    echo [INFO] Attaching existing database...
    docker exec -i %container% /opt/mssql-tools18/bin/sqlcmd -S localhost -C -U "%dbUser%" -P "%dbPass%" -i /tmp/db/attach_db_MSSQL.sql
) else (
    echo [INFO] Creating database...
    docker exec -i %container% /opt/mssql-tools18/bin/sqlcmd -S localhost -C -U "%dbUser%" -P "%dbPass%" -i /tmp/db/create_db_MSSQL.sql
)

if ERRORLEVEL 1 (
    echo [ERROR] Database setup failed
    exit /b 1
)

echo [INFO] Migration completed successfully.