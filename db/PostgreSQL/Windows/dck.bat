@echo off
setlocal EnableExtensions EnableDelayedExpansion

set DOCKER_CLI_HINTS=false

echo [INFO] Starting migration: Windows -> Docker Volume
set container=GeoNamesOrg_PSQL
set volume=geonamesorg_pgdata
set db_folder=%~dp0
set data_dir=%db_folder%.postgresql_data

echo [INFO] Checking Windows data directory...

if not exist "%data_dir%" (
    echo [INFO] Creating directory: %data_dir%
    mkdir "%data_dir%"
) else (
    echo [INFO] Directory exists
)

echo [INFO] Creating Docker volume (if missing)...
docker volume create %volume% >nul

echo [INFO] Copying data Windows -> Volume...
docker run --rm ^
  -v %volume%:/vg ^
  -v "%data_dir%:/src" ^
  ubuntu bash -c "cp -a /src/* /vg/ 2>/dev/null || true"

echo [INFO] Using hardcoded connection settings...
set dbHost=localhost
set dbPort=5433
set dbName=GeoNames.org
set dbUser=postgres
set dbPass=1qaz@WSX

echo [INFO] Parsed:
echo        HOST=!dbHost!
echo        PORT=!dbPort!
echo        DB=!dbName!
echo        USER=!dbUser!

echo [INFO] Removing existing container if any...
docker rm -f %container% >nul 2>&1

echo [INFO] Starting PostgreSQL container...
docker run -d ^
  --name %container% ^
  -e "POSTGRES_PASSWORD=!dbPass!" ^
  -e "POSTGRES_DB=!dbName!" ^
  -p !dbPort!:5432 ^
  -v %volume%:/var/lib/postgresql/data ^
  postgres:16 >nul

if ERRORLEVEL 1 (
    echo [ERROR] Failed to start container
    exit /b 1
)

echo [INFO] Waiting for PostgreSQL...
set "ready="
set /a max=30
for /l %%i in (1,1,!max!) do (
    docker exec %container% pg_isready -U "!dbUser!" >nul 2>&1
    if not errorlevel 1 (
        set "ready=1"
        echo [INFO] PostgreSQL ready
        goto :after_wait
    )
    echo [INFO] Attempt %%i/!max!...
    timeout /t 2 >nul
)
:after_wait
if not defined ready (
    echo [ERROR] PostgreSQL did not start
    exit /b 1
)

echo [INFO] Migration completed successfully.