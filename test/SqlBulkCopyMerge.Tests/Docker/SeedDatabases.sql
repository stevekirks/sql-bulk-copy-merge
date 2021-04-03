USE [master]

IF NOT EXISTS (SELECT * FROM master.sys.databases WHERE name = 'sourcedb')
	CREATE DATABASE sourcedb
GO

IF NOT EXISTS (SELECT * FROM master.sys.databases WHERE name = 'targetdb')
	CREATE DATABASE targetdb
GO





-- *************************************************************************************************************************
USE [sourcedb]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copying_latest_rows')
	DROP TABLE [dbo].[test_copying_latest_rows]
GO
CREATE TABLE [dbo].[test_copying_latest_rows](
	[id] INT NOT NULL,
	[notes] NVARCHAR(MAX),
	[timestamp] DATETIME2,
	[version_control] BINARY(8)
	CONSTRAINT [PK_test_copying_latest_rows] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copying_latest_rows] ( [id], [notes], [timestamp], [version_control] ) VALUES
(30, 'Note', '2020-01-01', CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note', '2020-01-30', CONVERT(VARBINARY(8), 10000001)),
(3, 'Note', '2020-02-01', CONVERT(VARBINARY(8), 10000001)),
(4, NULL, '2020-01-01', CONVERT(VARBINARY(8), 10000030));
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copying_latest_rows_from_view')
	DROP TABLE [dbo].[test_copying_latest_rows_from_view]
GO
CREATE TABLE [dbo].[test_copying_latest_rows_from_view](
	[id] INT IDENTITY(1,5) NOT NULL,
	[notes] NVARCHAR(MAX)
	CONSTRAINT [PK_test_copying_latest_rows_from_view] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copying_latest_rows_from_view] ( [notes] ) VALUES
('Note 1'), ('Note 2'), ('Note 3');
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'vtest_copying_latest_rows_from_view')
	DROP VIEW [dbo].[vtest_copying_latest_rows_from_view]
GO
CREATE VIEW [dbo].[vtest_copying_latest_rows_from_view] AS
(
	SELECT * FROM [dbo].[test_copying_latest_rows_from_view]
)
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copying_latest_rows_where_target_is_empty')
	DROP TABLE [dbo].[test_copying_latest_rows_where_target_is_empty]
GO
CREATE TABLE [dbo].[test_copying_latest_rows_where_target_is_empty](
	[id] INT NOT NULL,
	[version_control] BINARY(8),
	[timestamp] DATETIME2,
	[notes] NVARCHAR(MAX),
	CONSTRAINT [PK_test_copying_latest_rows_where_target_is_empty] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copying_latest_rows_where_target_is_empty] ( [id], [notes], [timestamp], [version_control] ) VALUES
(1, 'Note', '2020-01-01', CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note', '2020-01-30', CONVERT(VARBINARY(8), 10000001)),
(3, 'Note', '2020-02-01', CONVERT(VARBINARY(8), 10000001)),
(4, NULL, '2020-01-01', CONVERT(VARBINARY(8), 10000030));
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge')
	DROP TABLE [dbo].[test_copy_and_merge]
GO
CREATE TABLE [dbo].[test_copy_and_merge](
	[id] INT NOT NULL,
	[notes] NVARCHAR(MAX),
	[timestamp] DATETIME2,
	[version_control] BINARY(8),
	[geom] GEOMETRY
	CONSTRAINT [PK_test_copy_and_merge] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge] ( [id], [notes], [timestamp], [version_control] ) VALUES
(1, 'Note 1', '2020-01-01', CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note 2 overwrite', '2020-01-02', CONVERT(VARBINARY(8), 10000002)),
(3, 'Note 3', '2020-01-03', CONVERT(VARBINARY(8), 10000003)),
(4, NULL, '2020-01-04', CONVERT(VARBINARY(8), 10000004));
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge_subset')
	DROP TABLE [dbo].[test_copy_and_merge_subset]
GO
CREATE TABLE [dbo].[test_copy_and_merge_subset](
	[id] INT NOT NULL,
	[notes2] NVARCHAR(MAX),
	[notes] NVARCHAR(MAX)
	CONSTRAINT [PK_test_copy_and_merge_subset] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge_subset] ( [id], [notes2], [notes] ) VALUES
(3, 'Ignored', 'Note New'), 
(5, 'Ignored', 'Note Overwrite')
GO
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'vtest_copy_and_merge_subset')
	DROP VIEW [dbo].[vtest_copy_and_merge_subset]
GO
CREATE VIEW [dbo].[vtest_copy_and_merge_subset] AS
(
	SELECT [id], [notes] FROM [dbo].[test_copy_and_merge_subset]
)
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge_with_deletes_disabled')
	DROP TABLE [dbo].[test_copy_and_merge_with_deletes_disabled]
GO
CREATE TABLE [dbo].[test_copy_and_merge_with_deletes_disabled](
	[id] INT NOT NULL,
	[notes] NVARCHAR(MAX),
	[timestamp] DATETIME2,
	[geom] GEOMETRY,
	[version_control] BINARY(8)
	CONSTRAINT [PK_test_copy_and_merge_with_deletes_disabled] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge_with_deletes_disabled] ( [id], [notes], [timestamp], [version_control] ) VALUES
(2, 'Note 2 update', '2020-01-02', CONVERT(VARBINARY(8), 10000002)), 
(3, 'Note 3 new', '2020-01-03', CONVERT(VARBINARY(8), 10000003))
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge_with_different_column_names')
	DROP TABLE [dbo].[test_copy_and_merge_with_different_column_names]
GO
CREATE TABLE [dbo].[test_copy_and_merge_with_different_column_names](
	[id] INT NOT NULL,
	[notes] NVARCHAR(MAX),
	[timestamp] DATETIME2,
	[geom] GEOMETRY,
	[notes_2] NVARCHAR(200),
	[version_control] BINARY(8)
	CONSTRAINT [PK_test_copy_and_merge_with_different_column_names] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge_with_different_column_names] ( [id], [notes], [timestamp], [notes_2], [version_control] ) VALUES
(1, 'Note Update', '2020-01-01', NULL, CONVERT(VARBINARY(8), 10000001)), 
(3, 'Note New', '2020-01-03', NULL, CONVERT(VARBINARY(8), 10000003)), 
(4, 'Note Unchanged', '2020-01-04', 'Note not included', CONVERT(VARBINARY(8), 10000004))
GO


-- *************************************************************************************************************************
USE [targetdb]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copying_latest_rows')
	DROP TABLE [dbo].[test_copying_latest_rows]
GO
CREATE TABLE [dbo].[test_copying_latest_rows](
	[id] INT NOT NULL,
	[version_control] BINARY(8),
	[timestamp] DATETIME2,
	[notes] NVARCHAR(MAX),
	CONSTRAINT [PK_test_copying_latest_rows] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copying_latest_rows] ( [id], [notes], [timestamp], [version_control] ) VALUES
(29, 'Note 29', '2020-01-29', CONVERT(VARBINARY(8), 10000029));
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copying_latest_rows_from_view')
	DROP TABLE [dbo].[test_copying_latest_rows_from_view]
GO
CREATE TABLE [dbo].[test_copying_latest_rows_from_view](
	[id] INT IDENTITY(1,1) NOT NULL,
	[timestamp_no_match] DATETIME2,
	[notes] NVARCHAR(MAX),
	[geom_no_match] GEOMETRY,
	[version_control_no_match] BINARY(8)
	CONSTRAINT [PK_test_copying_latest_rows_from_view] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copying_latest_rows_from_view] ( [notes] ) VALUES
('Note 1');
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copying_latest_rows_where_target_is_empty')
	DROP TABLE [dbo].[test_copying_latest_rows_where_target_is_empty]
GO
CREATE TABLE [dbo].[test_copying_latest_rows_where_target_is_empty](
	[id] INT NOT NULL,
	[version_control] BINARY(8),
	[timestamp] DATETIME2,
	[notes] NVARCHAR(MAX),
	CONSTRAINT [PK_test_copying_latest_rows_where_target_is_empty] PRIMARY KEY ([id])
);
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge')
	DROP TABLE [dbo].[test_copy_and_merge]
GO
CREATE TABLE [dbo].[test_copy_and_merge](
	[id] INT NOT NULL,
	[notes] NVARCHAR(MAX),
	[timestamp] DATETIME2,
	[geom] GEOMETRY,
	[version_control] BINARY(8)
	CONSTRAINT [PK_test_copy_and_merge] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge] ( [id], [notes], [timestamp], [version_control] ) VALUES
(1, 'Note 1', '2020-01-01', CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note 2', '2020-01-02', CONVERT(VARBINARY(8), 10000002)),
(4, NULL, '2020-01-04', CONVERT(VARBINARY(8), 10000040)),
(5, NULL, '2020-01-05', CONVERT(VARBINARY(8), 10000050));
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge_subset')
	DROP TABLE [dbo].[test_copy_and_merge_subset]
GO
CREATE TABLE [dbo].[test_copy_and_merge_subset](
	[id] INT NOT NULL,
	[timestamp_no_match] DATETIME2,
	[notes] NVARCHAR(MAX),
	[version_control_no_match] BINARY(8)
	CONSTRAINT [PK_test_copy_and_merge_subset] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge_subset] ( [id], [notes], [timestamp_no_match], [version_control_no_match] ) VALUES
(1, 'Note 1', '2020-01-01', CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note 2', '2020-01-02', CONVERT(VARBINARY(8), 10000002)),
(4, NULL, '2020-01-04', CONVERT(VARBINARY(8), 10000040)),
(5, NULL, '2020-01-05', CONVERT(VARBINARY(8), 10000050));
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge_with_deletes_disabled')
	DROP TABLE [dbo].[test_copy_and_merge_with_deletes_disabled]
GO
CREATE TABLE [dbo].[test_copy_and_merge_with_deletes_disabled](
	[id] INT NOT NULL,
	[notes] NVARCHAR(MAX),
	[timestamp] DATETIME2,
	[geom] GEOMETRY,
	[version_control] BINARY(8)
	CONSTRAINT [PK_test_copy_and_merge_with_deletes_disabled] PRIMARY KEY ([id])
);
INSERT INTO [dbo].[test_copy_and_merge_with_deletes_disabled] ( [id], [notes], [timestamp], [version_control] ) VALUES
(1, 'Note wont be deleted even though it doesnt exist in source', '2020-01-01', CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note 2', '2020-01-02', CONVERT(VARBINARY(8), 10000002))
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'test_copy_and_merge_with_different_column_names_d')
	DROP TABLE [dbo].[test_copy_and_merge_with_different_column_names_d]
GO
CREATE TABLE [dbo].[test_copy_and_merge_with_different_column_names_d](
	[d_id] INT NOT NULL,
	[d_notes] NVARCHAR(200),
	[d_timestamp] DATETIME2,
	[d_geom] GEOMETRY,
	[notes_2] NVARCHAR(200),
	[d_version_control] BINARY(8)
	CONSTRAINT [PK_test_copy_and_merge_with_different_column_names_d] PRIMARY KEY ([d_id])
);
INSERT INTO [dbo].[test_copy_and_merge_with_different_column_names_d] ( [d_id], [d_notes], [d_timestamp], [notes_2], [d_version_control] ) VALUES
(1, 'Note 1', '2020-01-01', NULL, CONVERT(VARBINARY(8), 10000001)), 
(2, 'Note 2', '2020-01-02', NULL, CONVERT(VARBINARY(8), 10000002)), 
(4, 'Note Unchanged', '2020-01-04', 'Note not updated', CONVERT(VARBINARY(8), 10000004))
GO


USE [master]