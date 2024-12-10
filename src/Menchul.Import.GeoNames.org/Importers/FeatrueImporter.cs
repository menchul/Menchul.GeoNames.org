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
    internal class FeatureCodesImporter : BaseImporter
    {
        private static readonly List<string> __languages = ["bg", "nb", "nn", "no", "sv"];

        public FeatureCodesImporter(GeoNamesOrgDbContext dbContext, ILogger logger, ImporterParameters importerParameters)
            : base(dbContext, logger, importerParameters)
        {
        }

        protected override string FileURL => __baseUrl + "featureCodes_en.txt";
        protected override async Task ImportData()
        {
            foreach (string language in __languages)
            {
                string downloadFileName = $"featureCodes_{language}.txt";
                string localFileName = Path.Combine(__importerParameters.TempFolder, downloadFileName);

                if (File.Exists(localFileName))
                {
                    continue;
                }

                string fileURL = __baseUrl + downloadFileName;

                await DownloadFile(fileURL, localFileName);
            }


            await __dbContext.FeatureCodes.ExecuteDeleteAsync();

            await __dbContext.SaveChangesAsync();



            __languages.Add("en");

            var featureCodes = new List<FeatureCode>();
            var names = new List<FeatureCodeName>();

            foreach (string language in __languages)
            {
                string downloadFileName = $"featureCodes_{language}.txt";
                string localFileName = Path.Combine(__importerParameters.TempFolder, downloadFileName);

                string[] lines = await File.ReadAllLinesAsync(localFileName, __encoding);


                for (ulong i = FirstRow - 1; i < (ulong)lines.Length; i++)
                {
                    string line = lines[i];
                    string[] values = line.Split('\t');

                    if (!values[0].Contains('.'))
                    {
                        continue;
                    }

                    try
                    {
                        string featureCodeCode = values[0].Substring(2);
                        char featureClassCode = values[0][0];
                        string name = values[1];
                        string description = GetNullIfEmpty(values[2]);

                        bool fcEx = featureCodes.Any(x => x.Code == featureCodeCode);

                        if (!fcEx)
                        {
                            var featureCode = new FeatureCode
                            {
                                Code = featureCodeCode,
                                FeatureClassCode = featureClassCode
                            };

                            featureCodes.Add(featureCode);
                        }

                        bool nEx = names.Any(x => x.Language == language && x.FeatureCodeCode == featureCodeCode);

                        if (!nEx)
                        {
                            var fcn = new FeatureCodeName
                            {
                                FeatureCodeCode = featureCodeCode,
                                Language = language,
                                Name = name,
                                Description = description
                            };

                            names.Add(fcn);
                        }
                    }
                    catch (Exception exception)
                    {
                        __logger.LogError(exception, exception.Message);

                        throw;
                    }
                }
            }


            await __dbContext.FeatureCodes.AddRangeAsync(featureCodes);

            await __dbContext.FeatureCodeNames.AddRangeAsync(names);

            await __dbContext.SaveChangesAsync();
        }
    }
}