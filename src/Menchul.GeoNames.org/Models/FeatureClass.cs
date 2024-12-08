using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public record FeatureClass
    {
        public char Code { get; set; }

        public string Name { get; set; }

        public virtual List<FeatureCode> Codes { get; set; }

        public virtual List<GeoName> GeoNames { get; set; }



        public FeatureClass()
        {
        }

        public FeatureClass(char code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}