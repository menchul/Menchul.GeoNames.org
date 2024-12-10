using Menchul.GeoNames.org;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Importers
{
    internal sealed class CountriesImporter : BaseImporter
    {
        public CountriesImporter(GeoNamesOrgDbContext dbContext, ILogger logger, ImporterParameters importerParameters)
            : base(dbContext, logger, importerParameters)
        {
        }

        protected override string FileURL => __baseUrl + "countryInfo.txt";
        protected override ulong FirstRow => 51;

        protected override async Task ImportData()
        {
            await __dbContext.Countries.ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();


            string[] lines = await File.ReadAllLinesAsync(LocalFileName, __encoding);

            for (ulong i = FirstRow - 1; i < (ulong)lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split('\t');

                try
                {
                    string iso2 = values[0];

                    var country = new Country
                    {
                        ISO2 = iso2,
                        ISO3 = values[1],
                        ISONumeric = ushort.Parse(values[2]),
                        Fips = GetNullIfEmpty(values[3]),
                        Name = values[4],
                        Capital = GetNullIfEmpty(values[5]),
                        Area = decimal.Parse(values[6], __numberFormatInfo),
                        Population = uint.Parse(values[7]),
                        ContinentISO2 = values[8],
                        TLD = GetNullIfEmpty(values[9]),
                        CurrencyCode = GetNullIfEmpty(values[10]),
                        CurrencyName = GetNullIfEmpty(values[11]),
                        PhoneCode = GetNullIfEmpty(values[12]),
                        PostalCodeFormat = GetNullIfEmpty(values[13]),
                        PostalCodeRegex = GetNullIfEmpty(values[14]),
                        Languages = GetNullIfEmpty(values[15]),
                        GeoNameId = uint.Parse(values[16]),
                        Neighbours = GetNullIfEmpty(values[17]),
                        EquivalentFipsCode = GetNullIfEmpty(values[18])
                    };

                    await __dbContext.Countries.AddAsync(country);
                }
                catch (Exception exception)
                {
                    __logger.LogError(exception, exception.Message);

                    throw;
                }
            }


            await __dbContext.SaveChangesAsync();
        }
    }
}