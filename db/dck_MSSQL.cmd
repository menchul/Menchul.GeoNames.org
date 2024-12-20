@echo off

set DOCKER_CLI_HINTS=false

set db_folder=%~dp0
echo %db_folder%




echo Trying to remove old container...

docker rm --force --volumes GeoNamesOrg_ms_sql_2022_latest




echo.
echo.
echo Running MS SQL Server 2022 latest container "GeoNamesOrg_ms_sql_2022_latest"...

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=1qaz@WSX" -p 1433:1433  --name GeoNamesOrg_ms_sql_2022_latest -v %db_folder%:/tmp/db -d mcr.microsoft.com/mssql/server:2022-latest


if ERRORLEVEL 1 (
    pause
    exit
)





echo.
echo.
echo MS SQL Server starting... Please wait 10 sec
timeout 10
echo.
echo.


set db_files_folder=%db_folder%.MSSQL
echo %db_files_folder%

if not exist %db_files_folder% (
    mkdir %db_files_folder%
    docker exec -it GeoNamesOrg_ms_sql_2022_latest /opt/mssql-tools18/bin/sqlcmd -S localhost -C -U sa -P "1qaz@WSX" -i /tmp/db/create_db_MSSQL.sql    
    exit
)



docker exec -it GeoNamesOrg_ms_sql_2022_latest /opt/mssql-tools18/bin/sqlcmd -S localhost -C -U sa -P "1qaz@WSX" -i /tmp/db/attach_db_MSSQL.sql


if ERRORLEVEL 1 (
    pause
)