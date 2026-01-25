#!/bin/bash
set -x #echo on


set DOCKER_CLI_HINTS=false

echo Trying to remove old container "GeoNamesOrg_PSQL"...

docker rm --force --volumes GeoNamesOrg_PSQL


db_folder=/var/GeoNames.org/db/.postgresql_data
echo $db_folder

if [ ! -d $db_folder ]; then
    mkdir $db_folder
fi




echo Running PostgreSQL Server latest container "GeoNamesOrg_PSQL"...

docker run --name GeoNamesOrg_PSQL -e "POSTGRES_PASSWORD=1qaz@WSX" -e "POSTGRES_DB=GeoNames.org" -v $db_folder:/var/lib/postgresql/data -p 5433:5432 -d postgres