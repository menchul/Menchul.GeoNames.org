using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public record TimeZone
    {
        public string Name { get; set; } = default!;

        public string CountryCode { get; set; } = default!;

        public Country? Country { get; set; }

        /// <summary> GMT offset on 1st of January </summary>
        public decimal GMTOffset { get; set; } = default!;

        /// <summary> DST offset to gmt on 1st of July (of the current year) </summary>
        public decimal DSTOffset { get; set; } = default!;

        /// <summary> Raw Offset without DST </summary>
        public decimal RawOffset { get; set; } = default!;



        public virtual List<GeoName>? GeoNames { get; set; }
    }
}