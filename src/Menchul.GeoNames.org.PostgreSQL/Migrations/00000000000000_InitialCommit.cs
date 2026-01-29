using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Menchul.GeoNames.org.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gno");

            migrationBuilder.CreateTable(
                name: "Continents",
                schema: "gno",
                columns: table => new
                {
                    GeoNameId = table.Column<long>(type: "bigint", nullable: false),
                    ISO2 = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continents", x => x.GeoNameId);
                    table.UniqueConstraint("AK_Continents_ISO2", x => x.ISO2);
                });

            migrationBuilder.CreateTable(
                name: "FeatureClasses",
                schema: "gno",
                columns: table => new
                {
                    Code = table.Column<char>(type: "character(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureClasses", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ISOLanguages",
                schema: "gno",
                columns: table => new
                {
                    ISO639_3 = table.Column<string>(type: "character(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    ISO639_2 = table.Column<string>(type: "character(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    ISO639_1 = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISOLanguages", x => x.ISO639_3);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "gno",
                columns: table => new
                {
                    GeoNameId = table.Column<long>(type: "bigint", nullable: false),
                    ISONumeric = table.Column<int>(type: "integer", nullable: false),
                    ISO2 = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    ISO3 = table.Column<string>(type: "character(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    Fips = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    EquivalentFipsCode = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Capital = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Area = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Population = table.Column<long>(type: "bigint", nullable: false),
                    ContinentISO2 = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    TLD = table.Column<string>(type: "character(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    CurrencyName = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    PhoneCode = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    PostalCodeFormat = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    PostalCodeRegex = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    Languages = table.Column<string>(type: "text", unicode: false, nullable: true),
                    Neighbours = table.Column<string>(type: "text", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.GeoNameId);
                    table.UniqueConstraint("AK_Countries_ISO2", x => x.ISO2);
                    table.ForeignKey(
                        name: "FK_Continents_ISO2__Countries_ContinentISO2",
                        column: x => x.ContinentISO2,
                        principalSchema: "gno",
                        principalTable: "Continents",
                        principalColumn: "ISO2",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureCodes",
                schema: "gno",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    FeatureClass = table.Column<char>(type: "character(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureCodes", x => x.Code);
                    table.ForeignKey(
                        name: "FK_FeatureCodes_FeatureClassCode__FeatureClass_Code",
                        column: x => x.FeatureClass,
                        principalSchema: "gno",
                        principalTable: "FeatureClasses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                schema: "gno",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    CountryCode = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    GMTOffset = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    DSTOffset = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    RawOffset = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.Name);
                    table.ForeignKey(
                        name: "FK_TimeZones_CountryCode__Countries_ISO2",
                        column: x => x.CountryCode,
                        principalSchema: "gno",
                        principalTable: "Countries",
                        principalColumn: "ISO2",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureCodeNames",
                schema: "gno",
                columns: table => new
                {
                    FeatureCode = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureCodeNames", x => new { x.FeatureCode, x.Language });
                    table.ForeignKey(
                        name: "FK_FeatureCode_Code__FeatureCodeName_FeatureCodeCode",
                        column: x => x.FeatureCode,
                        principalSchema: "gno",
                        principalTable: "FeatureCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeoNames",
                schema: "gno",
                columns: table => new
                {
                    GeoNameId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ASCIIName = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    AlternateNames = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", precision: 8, scale: 5, nullable: false),
                    Longitude = table.Column<double>(type: "double precision", precision: 8, scale: 5, nullable: false),
                    FeatureClass = table.Column<char>(type: "character(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    FeatureCode = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    CountryCode = table.Column<string>(type: "character(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    CountryCodesAlternate = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    Admin1Code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    Admin2Code = table.Column<string>(type: "character varying(80)", unicode: false, maxLength: 80, nullable: true),
                    Admin3Code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    Admin4Code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    Population = table.Column<long>(type: "bigint", nullable: true),
                    Elevation = table.Column<int>(type: "integer", nullable: true),
                    DEM = table.Column<int>(type: "integer", nullable: false),
                    TimeZoneName = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: true),
                    ModificationDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoNames", x => x.GeoNameId);
                    table.ForeignKey(
                        name: "FK_GeoNames_CountryCode__Countries_ISO2",
                        column: x => x.CountryCode,
                        principalSchema: "gno",
                        principalTable: "Countries",
                        principalColumn: "ISO2");
                    table.ForeignKey(
                        name: "FK_GeoNames_FeatureCodeCode__FeatureCode_Code",
                        column: x => x.FeatureCode,
                        principalSchema: "gno",
                        principalTable: "FeatureCodes",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_GeoNames_TimeZoneName__TimeZones_Name",
                        column: x => x.TimeZoneName,
                        principalSchema: "gno",
                        principalTable: "TimeZones",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Geonames_FeatureClass__FeatureClass_Code",
                        column: x => x.FeatureClass,
                        principalSchema: "gno",
                        principalTable: "FeatureClasses",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "AlternateNamesV2",
                schema: "gno",
                columns: table => new
                {
                    GeoNameId = table.Column<long>(type: "bigint", nullable: false),
                    GeoNameIdRef = table.Column<long>(type: "bigint", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    C4 = table.Column<string>(type: "text", nullable: true),
                    C5 = table.Column<string>(type: "text", nullable: true),
                    C6 = table.Column<string>(type: "text", nullable: true),
                    C7 = table.Column<string>(type: "text", nullable: true),
                    C8 = table.Column<string>(type: "text", nullable: true),
                    C9 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternateNamesV2", x => x.GeoNameId);
                    table.ForeignKey(
                        name: "FK_AlternateNameV2_GeoNameId__GeoNames_GeoNameId",
                        column: x => x.GeoNameIdRef,
                        principalSchema: "gno",
                        principalTable: "GeoNames",
                        principalColumn: "GeoNameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "gno",
                table: "Continents",
                columns: new[] { "GeoNameId", "ISO2", "Name" },
                values: new object[,]
                {
                    { 6255146L, "AF", "Africa" },
                    { 6255147L, "AS", "Asia" },
                    { 6255148L, "EU", "Europe" },
                    { 6255149L, "NA", "North America" },
                    { 6255150L, "SA", "South America" },
                    { 6255151L, "OC", "Oceania" },
                    { 6255152L, "AN", "Antarctica" }
                });

            migrationBuilder.InsertData(
                schema: "gno",
                table: "FeatureClasses",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { 'A', "country, state, region,..." },
                    { 'H', "stream, lake,..." },
                    { 'L', "parks,area,..." },
                    { 'P', "city, village,..." },
                    { 'R', "road, railroad" },
                    { 'S', "spot, building, farm" },
                    { 'T', "mountain,hill,rock,..." },
                    { 'U', "undersea" },
                    { 'V', "forest,heath,..." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNamesV2_GeoNameIdRef",
                schema: "gno",
                table: "AlternateNamesV2",
                column: "GeoNameIdRef");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ContinentISO2",
                schema: "gno",
                table: "Countries",
                column: "ContinentISO2");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureCodes_FeatureClass",
                schema: "gno",
                table: "FeatureCodes",
                column: "FeatureClass");

            migrationBuilder.CreateIndex(
                name: "IX_GeoNames_CountryCode",
                schema: "gno",
                table: "GeoNames",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_GeoNames_FeatureClass",
                schema: "gno",
                table: "GeoNames",
                column: "FeatureClass");

            migrationBuilder.CreateIndex(
                name: "IX_GeoNames_FeatureCode",
                schema: "gno",
                table: "GeoNames",
                column: "FeatureCode");

            migrationBuilder.CreateIndex(
                name: "IX_GeoNames_TimeZoneName",
                schema: "gno",
                table: "GeoNames",
                column: "TimeZoneName");

            migrationBuilder.CreateIndex(
                name: "IX_TimeZones_CountryCode",
                schema: "gno",
                table: "TimeZones",
                column: "CountryCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternateNamesV2",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "FeatureCodeNames",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "ISOLanguages",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "GeoNames",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "FeatureCodes",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "TimeZones",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "FeatureClasses",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "gno");

            migrationBuilder.DropTable(
                name: "Continents",
                schema: "gno");
        }
    }
}