using Menchul.GeoNames.org.Models.Base;
using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public class Continent : BaseGeoNameIdEntity
    {
        public string ISO2 { get; set; } = default!;

        public string Name { get; set; } = default!;

        public virtual List<Country>? Countries { get; set; }



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