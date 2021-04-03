using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;
using SqlBulkCopyMerge.Models;

namespace SqlBulkCopyMerge
{
    internal static class SqlUtilExtensions
    {
        internal static (string TableSchema, string TableName) GetTableSchemaAndNameInParts(this string table)
        {
            if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException(nameof(table));

            var r = new Regex(@"^(?:\[?(?'table_schema'[^\[\]\.]+)\]?\.)?\[?(?'table_name'[^\[\]\.]+)\]?$");

            const string regexLabelTableSchema = "table_schema";
            const string regexLabelTableName = "table_name";
            if (r.IsMatch(table))
            {
                var match = r.Match(table);
                if (match.Groups.Cast<Group>().ToList().Any(a => a.Name == regexLabelTableName)
                    && match.Groups[regexLabelTableName].Success
                    && !string.IsNullOrWhiteSpace(match.Groups[regexLabelTableName].Value))
                {
                    var tableSchema = "dbo"; // default
                    var tableName = match.Groups[regexLabelTableName].Value;
                    if (match.Groups.Cast<Group>().ToList().Any(a => a.Name == regexLabelTableSchema)
                        && match.Groups[regexLabelTableSchema].Success
                        && !string.IsNullOrWhiteSpace(match.Groups[regexLabelTableSchema].Value))
                    {
                        tableSchema = match.Groups[regexLabelTableSchema].Value;
                    }

                    return (tableSchema, tableName);
                }
            }

            throw new InvalidArgumentException("Invalid table name " + table);
        }

        /// <summary>
        /// Format Table And Schema. Examples:<br/>
        /// table_name >>> [dbo].[table_name]<br/>
        /// my_schema.table_name >>> [my_schema].[table_name]
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static string FormatTableSchemaAndName(this string table)
        {
            var ts = GetTableSchemaAndNameInParts(table);
            return $"[{ts.TableSchema}].[{ts.TableName}]";
        }

        internal static string RemoveSquareBrackets(this string val)
        {
            return val?.Replace("[", "").Replace("]", "");
        }

        internal static string AddSquareBrackets(this string val)
        {
            if (val == null) return null;
            return $"[{RemoveSquareBrackets(val)}]";
        }
        
        internal static List<ColumnMapping> ValidateAndFormatColumnMappings(this List<ColumnMapping> columnMappings, List<ColumnSchemaModel> sourceColumns, List<ColumnSchemaModel> targetColumns)
        {
            if (columnMappings?.Any() == true)
            {
                // Validate column Mappings
                foreach (var columnMapping in columnMappings)
                {
                    if (string.IsNullOrWhiteSpace(columnMapping.Source) ||
                        string.IsNullOrWhiteSpace(columnMapping.Target))
                        throw new Exception("One or more column mappings are invalid");

                    columnMapping.Source = columnMapping.Source.RemoveSquareBrackets();
                    columnMapping.Target = columnMapping.Target.RemoveSquareBrackets();

                    if (!sourceColumns.Any(a => string.Equals(a.Name, columnMapping.Source, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new Exception($"Invalid Column Mappings. {columnMapping.Source} does not exist");
                    }
                    if (!targetColumns.Any(a => string.Equals(a.Name, columnMapping.Target, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new Exception($"Invalid Column Mappings. {columnMapping.Target} does not exist");
                    }
                }
            }

            return columnMappings;
        }
    }
}
