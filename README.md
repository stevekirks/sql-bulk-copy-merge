# SQL Bulk Copy & Merge [![CI](https://github.com/stevekirks/sql-bulk-copy-merge/actions/workflows/ci.yml/badge.svg)](https://github.com/stevekirks/sql-bulk-copy-merge/actions/workflows/ci.yml) [![NuGet Version](http://img.shields.io/nuget/v/SqlBulkCopyMerge.svg?style=flat)](https://www.nuget.org/packages/SqlBulkCopyMerge/)

This .NET library aims to simplify two common workflows that copy table data between SQL Server databases.

## Workflows
The workflows are.

### Copy & Merge
Uses SQLBulkCopy to copy data from a table or view in the source database to a temporary table in the target database before running SQL MERGE from the temporary table to the destination table.

This solution can be used instead of truncating the table each time before copying, which is not always possible or efficient.
Some other solutions that do this require defining the table schemas or are dependant on a SQL stored proc.

The specific steps it performs:
-   Checks target schema and table exists. If not, creates them
-   Creates temp table (target table name appended with '_temp')
-   Copies from table in source db to temp table in target db using SQLBulkCopy
-   Runs a SQL MERGE with the temp table as source and targetTable as target
-   Drops the temp table

#### Usage:
```
var copyService = new SqlBulkCopyMergeService(sourceDbConnectionString, targetDbConnectionString);
var result = await copyService.CopyAndMerge(sourceTableOrView, targetTable);
Console.WriteLine("Rows Inserted: " + result.Inserted);
Console.WriteLine("Rows Updated: " + result.Updated);
Console.WriteLine("Rows Deleted: " + result.Deleted);
```

If the column names between tables are different you can specify them, for example:
```
var columnMappings = new List<ColumnMapping>
{
    new ColumnMapping("id", "code"),
    new ColumnMapping("notes", "description")
};
var result = await copyService.CopyAndMerge(sourceTableOrView, targetTable, columnMappings);
```

### Copy Latest
For source tables that are only ever added to it is more efficient to copy only the new rows into the target table.
This method copies the latest data determined by a key column.

The specific steps it performs:
-   Queries target table for the latest key column value.
    If no key column is specified, the primary key is used. If more than one primary key, the first is used.
-   Queries source where the key column value is greater than the latest in the target.
    Eg. if the key column is [id], and its latest value is 2, then only rows with an [id] value greater than 2 are copied.
-   Copies from table in source db directly into table in target db using SQLBulkCopy.

#### Usage:
```
var copyService = new SqlBulkCopyMergeService(sourceDbConnectionString, targetDbConnectionString);
var result = await copyService.CopyLatest(sourceTableOrView, targetTable, keyColumnName);
Console.WriteLine("Rows Copied: " + result.RowsCopied);
```

## Notes
Source database connection requires READER permission.
Target database connection requires READER + WRITER + CREATE TABLE + EXECUTE permissions.

Tested on SQL Server 2019.

Spatial types (Geometry, Geography) are unsupported because SQLBulkCopy does not support them. The library ignores unsupported columns.
