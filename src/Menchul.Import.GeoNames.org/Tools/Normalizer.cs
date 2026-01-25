using Menchul.GeoNames.org;
using Menchul.GeoNames.org.Models;
using Menchul.Import.GeoNames.org.Tools.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Tools
{
    internal class Normalizer : INormalizer
    {
        private readonly GeoNamesOrgDbContext __dbContext;

        public Normalizer(GeoNamesOrgDbContext dbContext)
        {
            __dbContext = dbContext;
        }

        public async Task Normalize()
        {
            await NomalizeUA(__dbContext);
        }

        private static async Task NomalizeUA(GeoNamesOrgDbContext dbContext)
        {
            await NormalizeUAAlternateNames(dbContext);
        }

        private static async Task NormalizeUAAlternateNames(GeoNamesOrgDbContext dbContext)
        {
            AlternateNameV2[] names = await dbContext.AlternateNamesV2
                .Include(x => x.GeoName)
                .Where(x => x.GeoName!.CountryCode == "UA" && x.GeoName.FeatureCodeCode == "ADM1" && x.Name.Contains("щина"))
                .ToArrayAsync();

            dbContext.AlternateNamesV2.RemoveRange(names);

            names = await dbContext.AlternateNamesV2
                .Where(x => x.Name.Contains("Область"))
                .ToArrayAsync();

            foreach (AlternateNameV2 name in names)
            {
                name.Name = name.Name.Replace("Область", "область").Trim();
            }

            dbContext.AlternateNamesV2.UpdateRange(names);

            await dbContext.SaveChangesAsync();
        }
    }
}