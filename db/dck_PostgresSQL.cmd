@echo off

set DOCKER_CLI_HINTS=false

set db_folder=%~dp0
echo %db_folder%




echo Trying to remove old container "GeoNamesOrg_PSQL"...

docker rm --force --volumes GeoNamesOrg_PSQL





set db_files_folder=%db_folder%.postgresql_data
echo %db_files_folder%


if not exist %db_files_folder% (
    mkdir %db_files_folder%
)




echo.
echo.
echo Running PostgreSQL Server latest container "GeoNamesOrg_PSQL"...


docker run --name GeoNamesOrg_PSQL -e "POSTGRES_PASSWORD=1qaz@WSX" -e "POSTGRES_DB=GeoNames.org" -v %db_files_folder%:/var/lib/postgresql/data -p 5433:5432 -d postgres


if ERRORLEVEL 1 (
    pause
)


@rem username = postgres
@rem real machine IP exmpl:192.168.3.180


@rem https://www.pgadmin.org/docs/pgadmin4/latest/container_deployment.html

rem docker rm --force --volumes PgAdmin

rem docker run --name PgAdmin -e "PGADMIN_DEFAULT_EMAIL=q@q.qq" -e "PGADMIN_DEFAULT_PASSWORD=1" -p 888:80 -v %db_folder%.pgAdmin_cache:/var/lib/pgadmin -d dpage/pgadmin4

@rem or you can use dBeaver
@rem https://dbeaver.io/download/
@rem or you can use Azure Data Studio