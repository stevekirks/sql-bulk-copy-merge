# SQL Bulk Copy & Merge

This library aims to make copying table data between SQL databases easier.

SQLBulkCopy is useful to copy between databases, but truncating the destination table each time before copying is not always possible or efficient.
An alternative is to use SQLBulkCopy to copy to a temporary table and then run SQL MERGE between the temporary table and target.
Some other solutions that do this require extra work defining the table schemas or are dependant on a stored proc.

This .NET library has the following methods:

## CopyAndMerge
Uses SQLBulkCopy to copy data from a table or view in the source database to a temporary table in the target database before running SQL MERGE from the temporary table to the destination table.

The specific steps it performs:
-   Checks target schema and table exists. If not, creates them
-   Create temp table (target table name appended with '_temp'
-   Generate MERGE statement with the temp table as source and targetTable as target
-   Copy from table in source db to temp table in target db
-   Run MERGE statement
-   Drop the temp table

### Usage:
```
var copyService = new SqlBulkCopyMergeService(sourceDbConnectionString, targetDbConnectionString);
var result = await copyService.CopyAndMerge(sourceTable, targetTable);
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
var result = await copyService.CopyAndMerge(sourceTable, targetTable, columnMappings);
```

## CopyLatest
For source tables that are only ever added to it is more efficient to copy only the new rows into the target table.
This method copies the latest data determined by the keyColumnName.

A query is made on the target for the latest keyColumnName value, and then source is queried where the keyColumnName value is greater than the latest in the target.
Eg. if the keyColumnName is [id], and its latest value is 2, then only rows with an [id] value greater than 2 are copied.

If no keyColumnName is specified, the primary key is used. If more than one primary key, the first is used.

The result of the source query is directly copied into the target table using SQLBulkCopy.

### Usage:
```
var copyService = new SqlBulkCopyMergeService(sourceDbConnectionString, targetDbConnectionString);
var result = await copyService.CopyLatest(sourceTable, targetTable, keyColumnName);
Console.WriteLine("Rows Copied: " + result.RowsCopied);
```

## Notes
For the connections, the Source database requires READER permission, the Target database requires READER + WRITER + CREATE TABLE + EXECUTE permissions.

Tested on SQL Server 2019.

Spatial types (Geometry, Geography) are unsupported because SQLBulkCopy does not support them.