using Menchul.GeoNames.org;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Importers
{
    internal class ISOLanguagesImporter : BaseImporter
    {
        public ISOLanguagesImporter(GeoNamesOrgDbContext dbContext, ILogger logger, ImporterParameters importerParameters)
            : base(dbContext, logger, importerParameters)
        {
        }

        protected override string FileURL => __baseUrl + "iso-languagecodes.txt";

        protected override ulong FirstRow => 2;

        protected override async Task ImportData()
        {
            await __dbContext.ISOLanguages.ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();


            string[] lines = await File.ReadAllLinesAsync(LocalFileName, __encoding);

            var languages = new List<ISOLanguage>();
            var duplicates = new List<string>();

            for (ulong i = FirstRow - 1; i < (ulong)lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split('\t');

                try
                {
                    string iso3 = GetNullIfEmpty(values[0]);
                    string iso2 = GetNullIfEmpty(values[1]);

                    if (iso2 != null && iso2.Contains('/'))
                    {
                        iso2 = iso2.Split('/', StringSplitOptions.TrimEntries).Last();
                        iso2 = iso2.Substring(0, 3);
                    }

                    iso3 ??= iso2;

                    bool isoAlreadyExists = languages.Any(x => x.ISO639_3 == iso3);

                    if (isoAlreadyExists)
                    {
                        duplicates.Add(iso3);

                        continue;
                    }

                    var language = new ISOLanguage
                    {
                        ISO639_3 = iso3,
                        ISO639_2 = iso2,
                        ISO639_1 = GetNullIfEmpty(values[2]),
                        Name = values[3]
                    };

                    languages.Add(language);
                }
                catch (Exception exception)
                {
                    __logger.LogError(exception, exception.Message);

                    throw;
                }
            }


            await __dbContext.ISOLanguages.AddRangeAsync(languages);

            await __dbContext.SaveChangesAsync();

            await File.WriteAllLinesAsync("iso-languagecodes_duplicates.txt", duplicates);
        }
    }
}