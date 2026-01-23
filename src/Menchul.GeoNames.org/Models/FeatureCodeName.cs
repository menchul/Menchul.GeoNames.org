namespace Menchul.GeoNames.org.Models
{
    public record FeatureCodeName
    {
        public string FeatureCodeCode { get; set; } = default!;
        public virtual FeatureCode? FeatureCode { get; set; }

        public string Language { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}