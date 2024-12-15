namespace Menchul.GeoNames.org.Models
{
    public record ISOLanguage
    {
        public string ISO639_3 { get; set; }

        public string ISO639_2 { get; set; }

        public string ISO639_1 { get; set; }

        public string Name { get; set; }
    }
}