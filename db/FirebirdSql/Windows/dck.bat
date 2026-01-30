@echo off
setlocal EnableExtensions DisableDelayedExpansion

set DOCKER_CLI_HINTS=false

echo [INFO] Checking Docker...
docker info >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not available. Start Docker Desktop and ensure the script is run from a terminal that has access to Docker.
    echo [ERROR] If "Access is denied" to dockerDesktopLinuxEngine pipe - run this script from cmd or PowerShell started outside IDE, with Docker Desktop running.
    exit /b 1
)

echo [INFO] Starting migration: Windows -^> Docker Volume
set container=GeoNamesOrg_Firebird_latest
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
set dbHost=localhost
set dbPort=3050
set dbName=GeoNames.org
set dbUser=SYSDBA
set dbPass=masterkey

if not defined dbPass (
    echo [ERROR] Password not set
    exit /b 1
)

echo [INFO] Parsed:
echo        HOST=%dbHost%
echo        PORT=%dbPort%
echo        DB=%dbName%
echo        USER=%dbUser%

echo [INFO] Removing existing container if any...
docker rm -f %container% >nul 2>&1

echo [INFO] Starting Firebird container...
docker run -d ^
  --name %container% ^
  -e "FIREBIRD_ROOT_PASSWORD=%dbPass%" ^
  -e "FIREBIRD_DATABASE=%dbName%.fdb" ^
  -p %dbPort%:3050 ^
  -v "%db_root%:/tmp/db" ^
  -v "%data_dir%:/var/lib/firebird/data" ^
  firebirdsql/firebird:latest >nul

if ERRORLEVEL 1 (
    echo [ERROR] Failed to start container
    exit /b 1
)

echo [INFO] Waiting for Firebird...
timeout /t 25 >nul

echo [INFO] Checking connection to Firebird database...
set /a max_tries=20
set connected=
for /l %%t in (1,1,%max_tries%) do (
    (echo SELECT 1 FROM RDB$DATABASE; & echo quit;) | docker exec -i %container% isql -u %dbUser% -p %dbPass% -q 127.0.0.1:/var/lib/firebird/data/%dbName%.fdb >nul 2>&1
    if not errorlevel 1 (
        set connected=1
        echo [INFO] Firebird ready, database connection OK
        goto :connected
    )
    echo [INFO] Attempt %%t/%max_tries%...
    timeout /t 3 >nul
)
:connected
if not defined connected (
    echo [ERROR] Could not connect to Firebird database. Last isql attempt:
    (echo SELECT 1 FROM RDB$DATABASE; & echo quit;) | docker exec -i %container% isql -u %dbUser% -p %dbPass% -q 127.0.0.1:/var/lib/firebird/data/%dbName%.fdb
    exit /b 1
)

echo [INFO] Migration completed successfully.
