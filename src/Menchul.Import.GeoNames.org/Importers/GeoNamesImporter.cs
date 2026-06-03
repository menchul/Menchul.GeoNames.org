using EFCore.BulkExtensions;
using Menchul.GeoNames.org;
using Menchul.GeoNames.org.Models;
using Menchul.Import.GeoNames.org.Importers.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Importers
{
    internal class GeoNamesImporter : BaseImporter
    {
        private const uint __magicNumber = 100_000;
        private static readonly HashSet<char> __featureClassesAllowed = ['A', 'P'];
        private static readonly HashSet<string> __featureCodesDisabled = ["ADM1H", "ADM2H", "ADM3H", "ADM4H", "ADM5H", "ADMDH", "HSTS", "PCLH", "PPLCH", "PPLH", "RGNH"];
        internal static readonly HashSet<string> __nonExistingCountries = ["YU", ((char)82 + (char)85).ToString()];

        public GeoNamesImporter(GeoNamesOrgDbContext dbContext, ILogger<GeoNamesImporter> logger, ImporterParameters importerParameters)
            : base(dbContext, logger, importerParameters)
        {
        }

        public override byte Order => 4;

        protected override string FileURL => __baseUrl + "allCountries.zip";

        protected override async Task ImportData()
        {
            __logger.LogDebug("Deleting old data...");

            await __dbContext.GeoNames.ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();

            __logger.LogDebug("Old data was deleted.");

            string fileName = Path.Combine(ArchiveFolder, "allCountries.txt");

            ulong i = 0;
            ulong totalRecords = 0;

            var swLocal = new Stopwatch();
            swLocal.Start();
            var geonames = new List<GeoName>();

            await using (FileStream file = File.OpenRead(fileName))
            {
                using (var reader = new StreamReader(file, __encoding))
                {
                    __logger.LogTrace("Analyzing file...");

                    while (await reader.ReadLineAsync() is not null)
                    {
                        totalRecords++;
                    }

                    __logger.LogDebug($"Total lines are {totalRecords:### ### ###} in {swLocal.Elapsed:mm\\m\\ ss\\s}");

                    swLocal.Restart();

                    file.Seek(0, SeekOrigin.Begin);

                    using (_pbar = new(100, "", __progressBarOptions))
                    {
                        string? line;

                        while ((line = await reader.ReadLineAsync()) is not null)
                        {
                            #region console log

                            i++;
                            ulong x = i % __magicNumber;

                            if (x == 0)
                            {

                                await __dbContext.BulkInsertAsync(geonames);

                                geonames.Clear();


                                decimal seconds = swLocal.ElapsedMilliseconds / 1000m;
                                seconds = seconds == 0 ? 1 : seconds;
                                decimal speed = __magicNumber / seconds;
                                decimal percents = i * 100m / totalRecords;
                                int prcnt = (int)decimal.Round(percents);

                                decimal records = ((totalRecords - i) / (__magicNumber * 1m));
                                int cycles = (int)decimal.Round(records * seconds);
                                var eta = new TimeSpan(0, 0, cycles);

                                _pbar.Tick(prcnt, $"{i:### ### ###} records of {totalRecords:### ### ###} total records. Processing {speed:### ###} records per second. ETA {eta:hh\\:mm\\:ss}");

                                swLocal.Restart();
                            }

                            #endregion console log

                            string[] values = line.Split('\t');

                            try
                            {
                                string? countryCode = GetNullIfEmpty(values[8]);

                                if (!string.IsNullOrWhiteSpace(countryCode) && __nonExistingCountries.Contains(countryCode))
                                {
                                    continue;
                                }

                                char? featureClassCode = GetNullIfEmpty(values[6]) == null ? null : values[6][0];
                                string? featureCodeCode = GetNullIfEmpty(values[7]);

                                if (__importerParameters.ImportOnlyAP)
                                {
                                    if (featureClassCode == null)
                                    {
                                        continue;
                                    }

                                    if (!__featureClassesAllowed.Contains(featureClassCode.Value))
                                    {
                                        continue;
                                    }

                                    if (featureCodeCode == null)
                                    {
                                        continue;
                                    }

                                    if (__featureCodesDisabled.Contains(featureCodeCode))
                                    {
                                        continue;
                                    }
                                }

                                string? countryCodesAlternate = GetNullIfEmpty(values[9]);

                                if (__importerParameters.NormalizeData)
                                {
                                    if (string.Equals(countryCode, countryCodesAlternate, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        countryCodesAlternate = null;
                                    }
                                }

                                var geoName = new GeoName
                                {
                                    GeoNameId = uint.Parse(values[0]),
                                    Name = values[1],
                                    ASCIIName = values[2],
                                    AlternateNames = GetNullIfEmpty(values[3]),
                                    Latitude = double.Parse(values[4], __numberFormatInfo),
                                    Longitude = double.Parse(values[5], __numberFormatInfo),
                                    FeatureClassCode = featureClassCode,
                                    FeatureCodeCode = featureCodeCode,
                                    CountryCode = countryCode,
                                    CountryCodesAlternate = countryCodesAlternate,
                                    Admin1Code = GetNullIfEmpty(values[10]),
                                    Admin2Code = GetNullIfEmpty(values[11]),
                                    Admin3Code = GetNullIfEmpty(values[12]),
                                    Admin4Code = GetNullIfEmpty(values[13]),
                                    DEM = int.Parse(values[16]),
                                    TimeZoneName = GetNullIfEmpty(values[17]),
                                    ModificationDate = DateOnly.Parse(values[18])
                                };

                                if (!string.IsNullOrWhiteSpace(values[14]))
                                {
                                    long population = long.Parse(values[14]);
                                    geoName.Population = population;
                                }

                                if (!string.IsNullOrWhiteSpace(values[15]))
                                {
                                    int elevation = int.Parse(values[15]);
                                    geoName.Elevation = elevation;
                                }

                                geonames.Add(geoName);
                            }
                            catch (Exception exception)
                            {
                                __logger.LogError(exception, exception.Message);

                                throw;
                            }
                        }

                        _pbar.Tick(100, $"{i:### ### ###} records of {totalRecords:### ### ###} total records.");
                    }
                }
            }

            try
            {
                __logger.LogTrace("Adding records to DBSet...");
                swLocal.Restart();

                await __dbContext.BulkInsertAsync(geonames);

                __logger.LogTrace($"Adding records to DBSet took {swLocal.Elapsed}");

                geonames.Clear();

                __logger.LogTrace("START DB saving...");
                swLocal.Restart();

                await __dbContext.SaveChangesAsync();

                __logger.LogTrace($"END DB saving in {swLocal.Elapsed:hh\\:mm\\:ss}");
                swLocal.Stop();
            }
            catch (Exception exception)
            {
                __logger.LogError(exception, exception.Message);

                throw;
            }
        }
    }
}