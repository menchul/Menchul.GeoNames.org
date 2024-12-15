using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public record FeatureCode
    {
        public string Code { get; set; }

        public char FeatureClassCode { get; set; }
        public virtual FeatureClass FeatureClass { get; set; }

        public virtual List<FeatureCodeName> Names { get; set; }

        public virtual List<GeoName> GeoNames { get; set; }
    }
}