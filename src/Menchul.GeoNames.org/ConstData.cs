using Menchul.GeoNames.org.Models;

namespace Menchul.GeoNames.org
{
    internal static class ConstData
    {
        public static readonly Continent[] Continents =
        {
            new(6255146, "AF", "Africa"),
            new(6255147, "AS", "Asia"),
            new(6255148, "EU", "Europe"),
            new(6255149, "NA", "North America"),
            new(6255151, "OC", "Oceania"),
            new(6255150, "SA", "South America"),
            new(6255152, "AN", "Antarctica")
        };

        public static readonly FeatureClass[] FeatureClasses =
        {
            new('A', "country, state, region,..."),
            new('H', "stream, lake,..."),
            new('L', "parks,area,..."),
            new('P', "city, village,..."),
            new('R', "road, railroad"),
            new('S', "spot, building, farm"),
            new('T', "mountain,hill,rock,..."),
            new('U', "undersea"),
            new('V', "forest,heath,...")
        };
    }
}