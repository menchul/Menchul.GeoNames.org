namespace Menchul.GeoNames.org.Models
{
    public record FeatureCodeName
    {
        public string FeatureCodeCode { get; set; }
        public virtual FeatureCode FeatureCode { get; set; }

        public string Language { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}