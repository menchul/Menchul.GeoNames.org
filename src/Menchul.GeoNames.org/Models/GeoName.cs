using System;
using System.Collections.Generic;

namespace Menchul.GeoNames.org.Models
{
    public record GeoName
    {
        /// <summary> Integer id of record in geonames database </summary>
        public uint GeoNameId { get; set; }

        /// <summary> Name of geographical point (utf8) varchar(200) </summary>
        public string Name { get; set; }

        /// <summary> Name of geographical point in plain ascii characters, varchar(200) </summary>
        public string ASCIIName { get; set; }

        /// <summary> Alternatenames, comma separated, ascii names automatically transliterated, convenience attribute from alternatename table, varchar(10000) </summary>
        public string AlternateNames { get; set; }

        /// <summary> Latitude in decimal degrees (wgs84) </summary>
        public double Latitude { get; set; }

        /// <summary> Longitude in decimal degrees (wgs84) </summary>
        public double Longitude { get; set; }

        /// <summary> See http://www.geonames.org/export/codes.html, char(1) </summary>
        public char? FeatureClassCode { get; set; }
        public virtual FeatureClass FeatureClass { get; set; }

        /// <summary> See http://www.geonames.org/export/codes.html, varchar(10) </summary>
        public string FeatureCodeCode { get; set; }
        public virtual FeatureCode FeatureCode { get; set; }

        /// <summary> ISO-3166 2-letter country code, 2 characters </summary>
        public string CountryCode { get; set; }
        public virtual Country Country { get; set; }

        /// <summary> Alternate country codes, comma separated, ISO-3166 2-letter country code, 200 characters </summary>
        public string CountryCodesAlternate { get; set; }

        /// <summary> FipsCode (subject to change to iso code), see exceptions below, see file admin1Codes.txt for display names of this code; varchar(20) </summary>
        public string Admin1Code { get; set; }

        /// <summary> Code for the second administrative division, a county in the US, see file admin2Codes.txt; varchar(80) </summary>
        public string Admin2Code { get; set; }

        /// <summary> Code for third level administrative division, varchar(20) </summary>
        public string Admin3Code { get; set; }

        /// <summary> Code for fourth level administrative division, varchar(20) </summary>
        public string Admin4Code { get; set; }

        /// <summary> bigint (8 byte int) </summary>
        public long? Population { get; set; }

        /// <summary> in meters, integer </summary>
        public int? Elevation { get; set; }

        /// <summary> Digital elevation model, srtm3 or gtopo30, average elevation of 3''x3'' (ca 90mx90m) or 30''x30'' (ca 900mx900m) area in meters, integer. srtm processed by cgiar/ciat. </summary>
        public int DEM { get; set; }

        /// <summary> The IANA timezone (see file timeZone.txt) varchar(40) </summary>
        public string TimeZoneName { get; set; }

        public TimeZone TimeZone { get; set; }

        /// <summary> Date of last modification in yyyy-MM-dd format </summary>
#if NET5_0
        public DateTime ModificationDate { get; set; }
#else
        public DateOnly ModificationDate { get; set; }
#endif

        public virtual List<AlternateNameV2> AlternateNamesV2 { get; set; }
    }
}