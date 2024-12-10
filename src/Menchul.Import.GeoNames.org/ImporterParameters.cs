namespace Menchul.Import.GeoNames.org
{
    internal record ImporterParameters
    {
        public string TempFolder { get; set; }

        public Server Server { get; set; }

        public string ConnectionString { get; set; }

        public bool ImportOnlyAP { get; set; }

        public bool IgnoreBadNames { get; set; }
    }
}