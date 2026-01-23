using Menchul.GeoNames.org.Models.Base;

namespace Menchul.GeoNames.org.Models
{
    public class AlternateNameV2 : BaseGeoNameIdEntity
    {
        public uint GeoNameIdRef { get; set; }

        public virtual GeoName? GeoName { get; set; }

        public string? Language { get; set; }

        public string Name { get; set; } = default!;

        public string? C4 { get; set; }

        public string? C5 { get; set; }

        public string? C6 { get; set; }

        public string? C7 { get; set; }

        public string? C8 { get; set; }

        public string? C9 { get; set; }
    }
}