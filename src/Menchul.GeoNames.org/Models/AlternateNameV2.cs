namespace Menchul.GeoNames.org.Models
{
    public record AlternateNameV2
    {
        public uint GeoNameId { get; set; }

        public uint GeoNameIdRef { get; set; }

        public virtual GeoName GeoName { get; set; }

        public string Language { get; set; }

        public string Name { get; set; }

        public string C4 { get; set; }

        public string C5 { get; set; }

        public string C6 { get; set; }

        public string C7 { get; set; }

        public string C8 { get; set; }

        public string C9 { get; set; }
    }
}