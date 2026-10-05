using Menchul.GeoNames.org;
using Menchul.GeoNames.org.MSSQL;
using Menchul.GeoNames.org.PostgreSQL;
using Menchul.GeoNames.org.SQLite;
using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class DatabaseVerifier
{
    private readonly ILogger __logger;

    public DatabaseVerifier(ILogger logger)
    {
        __logger = logger;
    }

    public async Task<IReadOnlyList<TableCountResult>> VerifySqLiteAsync(string connectionString)
    {
        const string verifyLog = $"Querying table counts from {IntegrationTestConstants.DatabaseServers.SQLite}...";
        __logger.LogInformation(verifyLog);

        var factory = new GeoNamesOrgSQLiteDbContextFactory(connectionString);
        await using var dbContext = factory.CreateDbContext();

        IReadOnlyList<TableCountResult> results = await QueryTableCountsAsync(dbContext);

        return results;
    }

    public async Task<IReadOnlyList<TableCountResult>> VerifyPostgreSqlAsync(string connectionString)
    {
        const string verifyLog = $"Querying table counts from {IntegrationTestConstants.DatabaseServers.PostgreSQL}...";
        __logger.LogInformation(verifyLog);

        var factory = new GeoNamesOrgPostgreSQLDbContextFactory(connectionString);
        await using GeoNamesOrgPostgreSQLDbContext dbContext = factory.CreateDbContext();

        IReadOnlyList<TableCountResult> results = await QueryTableCountsAsync(dbContext);

        return results;
    }

    public async Task<IReadOnlyList<TableCountResult>> VerifyMsSqlAsync(string connectionString)
    {
        const string verifyLog = $"Querying table counts from {IntegrationTestConstants.DatabaseServers.MSSQL}...";
        __logger.LogInformation(verifyLog);

        var factory = new GeoNamesOrgMSSQLDbContextFactory(connectionString);
        await using GeoNamesOrgMSSQLDbContext dbContext = factory.CreateDbContext();

        IReadOnlyList<TableCountResult> results = await QueryTableCountsAsync(dbContext);

        return results;
    }

    private async Task<IReadOnlyList<TableCountResult>> QueryTableCountsAsync(GeoNamesOrgDbContext dbContext)
    {
        var list = new List<TableCountResult>();

        long geoNamesCount = await dbContext.GeoNames.LongCountAsync();
        var geoNamesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.GeoNames, geoNamesCount);
        list.Add(geoNamesItem);

        long altNamesCount = await dbContext.AlternateNamesV2.LongCountAsync();
        var altNamesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.AlternateNamesV2, altNamesCount);
        list.Add(altNamesItem);

        long countriesCount = await dbContext.Countries.LongCountAsync();
        var countriesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.Countries, countriesCount);
        list.Add(countriesItem);

        long featureCodesCount = await dbContext.FeatureCodes.LongCountAsync();
        var featureCodesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.FeatureCodes, featureCodesCount);
        list.Add(featureCodesItem);

        long isoLanguagesCount = await dbContext.ISOLanguages.LongCountAsync();
        var isoLanguagesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.ISOLanguages, isoLanguagesCount);
        list.Add(isoLanguagesItem);

        long timeZonesCount = await dbContext.TimeZones.LongCountAsync();
        var timeZonesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.TimeZones, timeZonesCount);
        list.Add(timeZonesItem);

        long continentsCount = await dbContext.Continents.LongCountAsync();
        var continentsItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.Continents, continentsCount);
        list.Add(continentsItem);

        long featureClassesCount = await dbContext.FeatureClasses.LongCountAsync();
        var featureClassesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.FeatureClasses, featureClassesCount);
        list.Add(featureClassesItem);

        long featureCodeNamesCount = await dbContext.FeatureCodeNames.LongCountAsync();
        var featureCodeNamesItem = new TableCountResult(GeoNamesOrgDbContext.TableNames.FeatureCodeNames, featureCodeNamesCount);
        list.Add(featureCodeNamesItem);

        foreach (TableCountResult item in list)
        {
            string rowLog = $"  - {item.TableName}: {item.RowCount:N0} rows";
            __logger.LogInformation(rowLog);
        }

        return list;
    }
}