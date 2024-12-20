
USE [master]
GO

ALTER DATABASE [dbFilmochskola] SET RECOVERY SIMPLE
GO

RESTORE DATABASE [dbFilmochskola]
	FROM DISK = '/tmp/db/dbFilmochskola_2024_07_23__19_20_06.bak'
	WITH MOVE 'dbFilmochskola' TO '/tmp/db/files/dbFilmochskola.mdf',
	MOVE 'dbFilmochskola_log' TO '/tmp/db/files/dbFilmochskola.ldf';

ALTER DATABASE [dbFilmochskola] SET RECOVERY FULL
GO
