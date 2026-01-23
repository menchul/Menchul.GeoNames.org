using Menchul.GeoNames.org.Models.Base;
using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public class Country : BaseGeoNameIdEntity
    {
        public ushort ISONumeric { get; set; }

        public string ISO2 { get; set; } = default!;

        public string ISO3 { get; set; } = default!;

        public string? Fips { get; set; }

        public string? EquivalentFipsCode { get; set; }

        public string Name { get; set; } = default!;

        public string? Capital { get; set; }

        public decimal Area { get; set; }

        public uint Population { get; set; }

        public string ContinentISO2 { get; set; } = default!;

        public virtual Continent? Continent { get; set; }

        public string? TLD { get; set; }

        public string? CurrencyCode { get; set; }

        public string? CurrencyName { get; set; }

        public string? PhoneCode { get; set; }

        public string? PostalCodeFormat { get; set; }

        public string? PostalCodeRegex { get; set; }

        public string? Languages { get; set; }

        public string? Neighbours { get; set; }



        public virtual List<TimeZone>? TimeZones { get; set; }

        public virtual List<GeoName>? GeoNames { get; set; }
    }
}