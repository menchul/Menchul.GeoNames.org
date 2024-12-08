using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public record Continent
    {
        public uint GeoNameId { get; set; }

        public string ISO2 { get; set; }

        public string Name { get; set; }

        public virtual List<Country> Countries { get; set; }



        public Continent()
        {
        }

        public Continent(uint geoNameId, string iso2, string name)
        {
            GeoNameId = geoNameId;
            ISO2 = iso2;
            Name = name;
        }
    }
}