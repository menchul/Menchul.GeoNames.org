using EFCore.BulkExtensions;
using Menchul.GeoNames.org;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Importers
{
    internal class AlternateNamesV2Importer : BaseImporter
    {
        private const uint __magicNumber = 100_000;
        private readonly HashSet<uint> __namesToRemove = new HashSet<uint>
        {
            17416183,
            2432644,
            7841995,

            17436381 /* Cherkasy */,
            8487117  /* Chernihiv */,
            17436392 /* Chernivtsi */,
            17436365 /* Dnipropetrovsk */,
            16868887 /* Donetsk */,
            17439886 /* Ivano-Frankivsk */,
            16868884 /* Kharkiv */,
            5650373  /* Kharkivs’ka Oblast’ */,
            17436390 /* Kherson */,
            17436387 /* Khmelnytskyi */,
            17436373 /* Kirovohrad */,
            16453734 /* Crimea */,
            8791795  /* Republic of Crimea */,
            8487107  /* Kiev */,
            8556550  /* Kiev Oblast */,
            13332554 /* Kyiv */,
            8556560  /* Kyivshchyna */,
            16868882 /* Luhansk */,
            16868881 /* Lviv */,
            17436370 /* Mykolaiv */,
            17436367 /* Odesa */,
            11284660 /* Odesa region */,
            16608411 /* Odessa */,
            17435484 /* Poltava */,
            16868878 /* Rivne */,
            13720982 /* Sebastopol City */,
            16868877 /* Sumy */,
            17439880 /* Ternopil */,
            17436384 /* Vinnytsia */,
            16868875 /* Volyn */,
            11990568 /* Transcarpathia */,
            17433769 /* Zakarpattia */,
            17436391 /* Zaporizhzhia */,
            16868873 /* Zhytomyr */,

            11990322 /* Lower Silesia */,
            13962465 /* Lower Silesian */,
            5463437  /* Kujawsko-Pomorskie */,
            13962471 /* Kuyavian-Pomeranian */,
            12095140 /* Lublin */,
            12095139 /* Lublin Province */,
            5463452  /* Lubusz */,
            1636267  /* Lubusz Voivodship */,
            13962464 /* Lesser Poland */,
            13962466 /* Masovian */,
            11990469 /* Mazovia */,
            11990468 /* Mazovia Province */,
            13962470 /* Subcarpathia */,
            13962467 /* Podlachian */,
            5463453  /* Podlasie */,
            1636402  /* Podlasie Voivodship */,
            11990471 /* Pomerania */,
            11990470 /* Pomerania Province */,
            12133956 /* Silesia */,
            5463454  /* Świętokrzyskie */,
            1636380  /* Świętokrzyskie Voivodship */,
            11990328 /* Warmia-Masuria */,
            11990327 /* Warmia-Masuria Province */,
            13962468 /* Warmian-Masurian */,
            11990321 /* Greater Poland */,
            11990320 /* West Pomerania */,
            13962469 /* West Pomeranian */,
            11990319 /* West Pomerania Province */,


            9711883 /* Lubelskie */,
            2185032 /* woj.warmińsko - mazurskie */
        };

        public AlternateNamesV2Importer(GeoNamesOrgDbContext dbContext, ILogger logger, ImporterParameters importerParameters)
            : base(dbContext, logger, importerParameters)
        {
        }

        protected override string FileURL => __baseUrl + "alternateNamesV2.zip";

        protected override async Task ImportData()
        {
            __logger.LogDebug("Deleting old data...");

            await __dbContext.AlternateNamesV2.ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();

            __logger.LogDebug("Old data was deleted.");

            HashSet<uint> geoNamesIds = await __dbContext.GeoNames.Select(x => x.GeoNameId).OrderBy(x => x).ToHashSetAsync();

            string fileName = Path.Combine(ArchiveFolder, "alternateNamesV2.txt");

            ulong i = 0;
            ulong totalRecords = 0;

            var swLocal = new Stopwatch();
            swLocal.Start();

            await using (FileStream file = File.OpenRead(fileName))
            {
                using (var reader = new StreamReader(file, __encoding))
                {
                    #region file analyzer

                    __logger.LogTrace("Analyzing file...");

                    while (file.CanRead && !reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();

                        if (line == null)
                        {
                            break;
                        }

                        totalRecords++;
                    }

                    __logger.LogDebug($"Total lines are {totalRecords:### ### ###} in {swLocal.Elapsed:mm\\m\\ ss\\s}");

                    swLocal.Restart();

                    file.Seek(0, SeekOrigin.Begin);

                    #endregion file analyzer

                    using (_pbar = new(100, "", __progressBarOptions))
                    {
                        var alternateNames = new List<AlternateNameV2>();

                        while (file.CanRead && !reader.EndOfStream)
                        {
                            #region console log

                            i++;

                            ulong x = i % __magicNumber;

                            if (x == 0)
                            {
                                await __dbContext.BulkInsertAsync(alternateNames);

                                alternateNames.Clear();

                                decimal seconds = swLocal.ElapsedMilliseconds / 1000m;
                                seconds = seconds == 0 ? 1 : seconds;
                                decimal speed = __magicNumber / seconds;
                                decimal percents = i * 100m / totalRecords;
                                int prcnt = (int)decimal.Round(percents);

                                decimal records = ((totalRecords - i) / (__magicNumber * 1m));
                                int cycles = (int)decimal.Round(records * seconds);
                                var eta = new TimeSpan(0, 0, cycles);

                                _pbar.Tick(prcnt, $"{i:### ### ###} records of {totalRecords:### ### ###} total records. Processing {speed:### ###} records per second. ETA {eta:hh\\:mm\\:ss} seconds");

                                swLocal.Restart();

                                try
                                {
                                    await __dbContext.SaveChangesAsync();
                                }
                                catch (Exception exception)
                                {
                                    __logger.LogError(exception, exception.Message);

                                    throw;
                                }
                            }

                            #endregion console log


                            string line = await reader.ReadLineAsync();

                            if (line == null)
                            {
                                break;
                            }

                            string[] values = line.Split('\t');


                            try
                            {
                                uint geoNameId = uint.Parse(values[0]);

                                if (__importerParameters.NormalizeData)
                                {
                                    if (__namesToRemove.Contains(geoNameId))
                                    {
                                        continue;
                                    }
                                }

                                string language = GetNullIfEmpty(values[2]);

                                if (string.Equals(language, "ru", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    continue;
                                }

                                string name = GetNullIfEmpty(values[3]);

                                if (name == null)
                                {
                                    continue;
                                }

                                if (__importerParameters.NormalizeData)
                                {
                                    if (!IsTextOnLanguage(name, language))
                                    {
                                        continue;
                                    }
                                }

                                uint geoNameIdRef = uint.Parse(values[1]);

                                if (!geoNamesIds.Contains(geoNameIdRef))
                                {
                                    continue;
                                }

                                var alternateNameV2 = new AlternateNameV2
                                {
                                    GeoNameId = geoNameId,
                                    GeoNameIdRef = geoNameIdRef,
                                    Language = language,
                                    Name = name,
                                    C4 = GetNullIfEmpty(values[4]),
                                    C5 = GetNullIfEmpty(values[5]),
                                    C6 = GetNullIfEmpty(values[6]),
                                    C7 = GetNullIfEmpty(values[7]),
                                    C8 = GetNullIfEmpty(values[8]),
                                    C9 = GetNullIfEmpty(values[9])
                                };

                                alternateNames.Add(alternateNameV2);
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

            await __dbContext.SaveChangesAsync();

            geoNamesIds.Clear();
            geoNamesIds = null;

            //await __dbContext.TimeZones.Where(x => x.CountryCode == "RU").ExecuteDeleteAsync();

            string l404 = GeoNamesImporter.__404.ToLower() + (char)115;

            await __dbContext.ISOLanguages.Where(x => x.ISO639_3 == l404).ExecuteDeleteAsync();

            await __dbContext.Countries.Where(x => x.ContinentISO2 == GeoNamesImporter.__404).ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();

            swLocal.Stop();
        }

        private const string alphabet_ua = @"АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
        private readonly Regex regex_ua = new Regex($"[{alphabet_ua}]");

        private const string alphabet_bg = @"АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
        private readonly Regex regex_bg = new Regex($"[{alphabet_bg}]");

        private const string alphabet_be = @"АБВГДЕЁЖЗІЙКЛМНОПРСТУЎФХЦЧШЫЬЭЮЯ";
        private readonly Regex regex_be = new Regex($"[{alphabet_be}]");

        private Regex GetAlphabetRegex(string language)
        {
            switch (language)
            {
                case "uk": return regex_ua;
                case "bg": return regex_bg;
                case "be": return regex_be;
                default:
                    return null;
            }
        }

        private bool IsTextOnLanguage(string text, string language)
        {
            Regex regExp = GetAlphabetRegex(language);

            if (regExp == null)
            {
                return true;
            }

            CultureInfo culture = CultureInfo.GetCultureInfo(language);

            bool result = regExp.IsMatch(text.ToUpper(culture));

            return result;
        }
    }
}