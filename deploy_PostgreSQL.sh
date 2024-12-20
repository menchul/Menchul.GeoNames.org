#!/bin/bash
set -x #echo on

clear

DOTNET_CLI_TELEMETRY_OPTOUT=false
export GeoNamesOrg__ConnectionStrings="Host=localhost;Port=5433;Database=GeoNames.org;Username=postgres;Password=1qaz@WSX;CommandTimeout=10;"
prj=src/Menchul.GeoNames.org/Menchul.GeoNames.org.csproj
src=src/Menchul.GeoNames.org.PostgreSQL/Menchul.GeoNames.org.PostgreSQL.csproj
pkj=Npgsql.EntityFrameworkCore.PostgreSQL
net="--framework NET9.0"

echo ***** Adding project package

dotnet add $prj package $pkj --version 9.0.2 $net

echo ***** Adding migration...

dotnet ef migrations add InitialMigration -p $prj -s $src $net

echo ***** Database uprading...

dotnet ef database update -p $prj -s "$src" $net

echo ***** Removing migrations...

rm -f -r -d src/Menchul.GeoNames.org/Migrations

echo ***** Removing project package....

dotnet remove $prj package $pkj