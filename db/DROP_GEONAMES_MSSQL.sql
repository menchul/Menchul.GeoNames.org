USE [GeoNames.org];
GO


DROP TABLE [GeoNames.org].[dbo].[__EFMigrationsHistory];

DROP TABLE [GeoNames.org].[gno].[AlternateNamesV2];

DROP TABLE [GeoNames.org].[gno].[geonames];

DROP TABLE [GeoNames.org].[gno].[TimeZones];

DROP TABLE [GeoNames.org].[gno].[Countries];

DROP TABLE [GeoNames.org].[gno].[FeatureCodeNames];

DROP TABLE [GeoNames.org].[gno].[FeatureCodes];

DROP TABLE [GeoNames.org].[gno].[FeatureClasses];

DROP TABLE [GeoNames.org].[gno].[ISOLanguages];

DROP TABLE [GeoNames.org].[gno].[Continents];
GO


DBCC SHRINKDATABASE ([GeoNames.org], 0);
GO
