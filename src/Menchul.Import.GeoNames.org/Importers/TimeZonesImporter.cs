using Menchul.GeoNames.org;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Importers
{
    internal sealed class TimeZonesImporter : BaseImporter
    {
        public TimeZonesImporter(GeoNamesOrgDbContext dbContext, ILogger logger, ImporterParameters importerParameters)
            : base(dbContext, logger, importerParameters)
        {
        }

        protected override string FileURL => __baseUrl + "timeZones.txt";

        protected override ulong FirstRow => 2;

        protected override async Task ImportData()
        {
            await __dbContext.TimeZones.ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();


            string[] lines = await File.ReadAllLinesAsync(LocalFileName, __encoding);

            for (ulong i = FirstRow - 1; i < (ulong)lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split('\t');

                string name = values[1];
                string country = values[0];
                decimal gmt = decimal.Parse(values[2], __numberFormatInfo);
                decimal dst = decimal.Parse(values[3], __numberFormatInfo);
                decimal raw = decimal.Parse(values[4], __numberFormatInfo);

                try
                {
                    var entity = new Menchul.GeoNames.org.Models.TimeZone
                    {
                        Name = name,
                        CountryCode = country,
                        GMTOffset = gmt,
                        DSTOffset = dst,
                        RawOffset = raw
                    };

                    await __dbContext.TimeZones.AddAsync(entity);
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