using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using SqlBulkCopyMerge.Models;

namespace SqlBulkCopyMerge
{
    /// <summary>
    /// Simplify common workflows that copy table data between SQL Server databases
    /// </summary>
    public interface ISqlBulkCopyMergeService
    {
        /// <summary>
        /// Copy data from table or view in source database to a temporary table in the target database
        /// before running SQL MERGE from the temporary table to the target table.<br/>
        /// Steps:<br/>
        /// -   Check target schema and table exists. If not, create it<br/>
        /// -   Create temp table (target table name appended with '_temp'<br/>
        /// -   Generate MERGE statement with the temp table as source and targetTable as target<br/>
        /// -   Copy from table in source db to temp table in target db<br/>
        /// -   Run MERGE statement<br/>
        /// -   Drop the temp table<br/>
        /// </summary>
        /// <param name="sourceTable">Table in the source database</param>
        /// <param name="targetTable">Table in the target database</param>
        /// <param name="columnMappings">Optional columns mappings. These are only necessary if the column names differ between source and target</param>
        /// <param name="copyAndMergeConfig">Optional config for the SQLBulkCopy and the SQL MERGE</param>
        /// <returns>Results of the copy and merge, including number of rows Inserted, Updated, and Deleted</returns>
        Task<MergeResult> CopyAndMerge(string sourceTable, string targetTable, List<ColumnMapping> columnMappings = null, CopyAndMergeConfig copyAndMergeConfig = null);

        /// <summary>
        /// Copy data from table or view in source database to a temporary table in the target database
        /// before running SQL MERGE from the temporary table to the target table.<br/>
        /// Steps:<br/>
        /// -   Check target schema and table exists. If not, create it<br/>
        /// -   Create temp table (target table name appended with '_temp'<br/>
        /// -   Generate MERGE statement with the temp table as source and targetTable as target<br/>
        /// -   Copy from table in source db to temp table in target db<br/>
        /// -   Run MERGE statement<br/>
        /// -   Drop the temp table<br/>
        /// </summary>
        /// <param name="sourceTable">Table in the source database</param>
        /// <param name="targetTable">Table in the target database</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="columnMappings">Optional columns mappings. These are only necessary if the column names differ between source and target</param>
        /// <param name="copyAndMergeConfig">Optional config for the SQLBulkCopy and the SQL MERGE</param>
        /// <returns>Results of the copy and merge, including number of rows Inserted, Updated, and Deleted</returns>
        Task<MergeResult> CopyAndMerge(string sourceTable, string targetTable, CancellationToken cancellationToken, List<ColumnMapping> columnMappings = null, CopyAndMergeConfig copyAndMergeConfig = null);

        /// <summary>
        /// Copy the latest data. The data to transfer is determined by the keyColumnName.
        /// A query is made on the target for the latest keyColumnName value, and then the 
        /// query on the source gets data where the keyColumnName value is greater than the 
        /// latest in the target. Eg. if the keyColumnName is [id], and its latest value is 2,
        /// then only rows with an [id] value greater than 2 are copied. <br/>
        /// If no keyColumnName is specified, the primary key is used. If more than one primary key, 
        /// the first is used.
        /// </summary>
        /// <param name="sourceTable">The source table or view name in the source database. Eg. [schema].[table_name]</param>
        /// <param name="targetTable">The target table name in the target database. Eg. [schema].[table_name]</param>
        /// <param name="keyColumnName">The column name used to determine the latest data. Eg. [Id]</param>
        /// <param name="columnMappings">Optional columns mappings. These are only necessary if the column names differ between source and target</param>
        /// <param name="sqlBulkCopyConfig">Optional config for the SQLBulkCopy</param>
        /// <returns>The number of Rows Copied</returns>
        Task<int> CopyLatest(string sourceTable, string targetTable, string keyColumnName = null, List<ColumnMapping> columnMappings = null, SqlBulkCopyConfig sqlBulkCopyConfig = null);

        /// <summary>
        /// Copy the latest data. The data to transfer is determined by the keyColumnName.
        /// A query is made on the target for the latest keyColumnName value, and then the 
        /// query on the source gets data where the keyColumnName value is greater than the 
        /// latest in the target. Eg. if the keyColumnName is [id], and its latest value is 2,
        /// then only rows with an [id] value greater than 2 are copied. <br/>
        /// If no keyColumnName is specified, the primary key is used. If more than one primary key, 
        /// the first is used.
        /// </summary>
        /// <param name="sourceTable">The source table or view name in the source database. Eg. [schema].[table_name]</param>
        /// <param name="targetTable">The target table name in the target database. Eg. [schema].[table_name]</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <param name="keyColumnName">The column name used to determine the latest data. Eg. [Id]</param>
        /// <param name="columnMappings">Optional columns mappings. These are only necessary if the column names differ between source and target</param>
        /// <param name="sqlBulkCopyConfig">Optional config for the SQLBulkCopy</param>
        /// <returns>The number of Rows Copied</returns>
        Task<int> CopyLatest(string sourceTable, string targetTable, CancellationToken cancellationToken, string keyColumnName = null, List<ColumnMapping> columnMappings = null, SqlBulkCopyConfig sqlBulkCopyConfig = null);

        /// <summary>
        /// SQL Bulk Copy<br/>
        /// This method is used internally but is public as it may be suitable for other uses.
        /// </summary>
        /// <param name="sourceSelectQuery"></param>
        /// <param name="targetTable"></param>
        /// <param name="sqlParameters"></param>
        /// <param name="columnMappings">Optional columns mappings. These are only necessary if the column names differ between source and target</param>
        /// <param name="sqlBulkCopyConfig"></param>
        /// <returns>The number of Rows Copied</returns>
        Task<int> SqlBulkCopy(string sourceSelectQuery, string targetTable,
            IEnumerable<SqlParameter> sqlParameters = null,
            IEnumerable<ColumnMapping> columnMappings = null,
            SqlBulkCopyConfig sqlBulkCopyConfig = null);

        /// <summary>
        /// SQL Bulk Copy<br/>
        /// This method is used internally but is public as it may be suitable for other uses.
        /// </summary>
        /// <param name="sourceSelectQuery"></param>
        /// <param name="targetTable"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="sqlParameters"></param>
        /// <param name="columnMappings">Optional columns mappings. These are only necessary if the column names differ between source and target</param>
        /// <param name="sqlBulkCopyConfig"></param>
        /// <returns>The number of Rows Copied</returns>
        Task<int> SqlBulkCopy(string sourceSelectQuery, string targetTable,
            CancellationToken cancellationToken,
            IEnumerable<SqlParameter> sqlParameters = null,
            IEnumerable<ColumnMapping> columnMappings = null,
            SqlBulkCopyConfig sqlBulkCopyConfig = null);
    }

    /// <inheritdoc />
    public class SqlBulkCopyMergeService : ISqlBulkCopyMergeService
    {
        private readonly string _sourceDbConnectionString;
        private readonly string _targetDbConnectionString;
        private readonly ILogger _logger;

        /// <summary>
        /// Simplify common workflows that copy table data between SQL Server databases
        /// </summary>
        /// <param name="sourceDbConnectionString">Source database connection string.<br/>
        /// Requires READER permission.</param>
        /// <param name="targetDbConnectionString">Target database connection string.<br/>
        /// Requires READER + WRITER + CREATE TABLE + EXECUTE permissions.</param>
        /// <param name="logger">Inject an optional logger</param>
        public SqlBulkCopyMergeService(string sourceDbConnectionString, string targetDbConnectionString, ILogger logger = null)
        {
            _sourceDbConnectionString = sourceDbConnectionString;
            _targetDbConnectionString = targetDbConnectionString;
            _logger = logger ?? Log.Logger;
        }

        /// <inheritdoc />
        public async Task<int> CopyLatest(string sourceTable, string targetTable, string keyColumnName = null,
            List<ColumnMapping> columnMappings = null,
            SqlBulkCopyConfig sqlBulkCopyConfig = null)
        {
            return await CopyLatest(sourceTable, targetTable, CancellationToken.None, keyColumnName, columnMappings, sqlBulkCopyConfig);
        }

        /// <inheritdoc />
        public async Task<int> CopyLatest(string sourceTable, string targetTable, CancellationToken cancellationToken,
            string keyColumnName = null, List<ColumnMapping> columnMappings = null, SqlBulkCopyConfig sqlBulkCopyConfig = null)
        {
            var sourceSchemaAndTable = sourceTable.FormatTableSchemaAndName();
            var targetSchemaAndTable = targetTable.FormatTableSchemaAndName();
            var sourceColumns = await GetColumnSchemaInfo(_sourceDbConnectionString, sourceTable);
            var targetColumns = await GetColumnSchemaInfo(_targetDbConnectionString, targetTable);

            columnMappings = columnMappings.ValidateAndFormatColumnMappings(sourceColumns, targetColumns);

            if (keyColumnName == null)
            {
                keyColumnName = targetColumns.FirstOrDefault(a => a.IsPrimaryKey 
                                                                  && (sourceColumns.Any(s => string.Equals(a.Name, s.Name, StringComparison.InvariantCultureIgnoreCase))
                                                                  || (columnMappings?.Any(s => string.Equals(a.Name, s.Target, StringComparison.InvariantCultureIgnoreCase)) == true)))?.Name;
                if (keyColumnName == null)
                    throw new ArgumentException(
                        $"{nameof(keyColumnName)} must be specified as table has no primary key to use instead");
            }
            var keyColumnSchema = targetColumns.FirstOrDefault(a =>
                string.Equals(a.Name, keyColumnName.RemoveSquareBrackets(), StringComparison.InvariantCultureIgnoreCase));
            if (keyColumnSchema == null)
                throw new ArgumentException($"{nameof(keyColumnName)} does not exist in target table");

            var latestKeyColumnVal = await SqlUtils.ExecuteScalar(_targetDbConnectionString,
                $"SELECT TOP 1 {keyColumnSchema.Name.AddSquareBrackets()} FROM {targetSchemaAndTable} ORDER BY {keyColumnSchema.Name.AddSquareBrackets()} DESC");

            var sqlBulkCopyQuery = new StringBuilder();
            sqlBulkCopyQuery.Append("SELECT ");
            var selectColumns = new List<string>();
            if (columnMappings?.Any() == true)
            {
                selectColumns.AddRange(columnMappings.Select(a => a.Source));
            }
            else
            {
                foreach (var targetColumn in targetColumns)
                {
                    if (sourceColumns.Any(a => a.Name == targetColumn.Name))
                    {
                        selectColumns.Add(targetColumn.Name.AddSquareBrackets());
                    }
                }
                if (!selectColumns.Any())
                    throw new Exception("No column names between the source and target tables match");
            }
            sqlBulkCopyQuery.AppendLine(string.Join(", ", selectColumns));
            sqlBulkCopyQuery.AppendLine($"FROM {sourceSchemaAndTable}");

            List<SqlParameter> sqlParameters = null;
            const string latestKeyValueLabel = "LatestKeyValue";
            if (latestKeyColumnVal != null)
            {
                sqlBulkCopyQuery.AppendLine($"WHERE {keyColumnName} > @{latestKeyValueLabel}");
                sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter(latestKeyValueLabel, keyColumnSchema.Type) {Value = latestKeyColumnVal}
                };
            }

            var result = await SqlBulkCopy(sqlBulkCopyQuery.ToString(), targetSchemaAndTable, cancellationToken, 
                sqlParameters, columnMappings ?? selectColumns.Select(a => new ColumnMapping(a,a)), sqlBulkCopyConfig);

            return result;
        }

        /// <inheritdoc />
        public async Task<MergeResult> CopyAndMerge(string sourceTable, string targetTable, List<ColumnMapping> columnMappings = null, CopyAndMergeConfig copyAndMergeConfig = null)
        {
            return await CopyAndMerge(sourceTable, targetTable, CancellationToken.None, columnMappings, copyAndMergeConfig);
        }

        /// <inheritdoc />
        public async Task<MergeResult> CopyAndMerge(string sourceTable, string targetTable, CancellationToken cancellationToken,
            List<ColumnMapping> columnMappings = null, CopyAndMergeConfig copyAndMergeConfig = null)
        {
            var sourceSchemaAndTable = sourceTable.FormatTableSchemaAndName();
            var targetSchemaAndTable = targetTable.FormatTableSchemaAndName();
            var ts = targetSchemaAndTable.GetTableSchemaAndNameInParts();

            var sourceColumns = await GetColumnSchemaInfo(_sourceDbConnectionString, sourceTable);
            var targetColumns = await GetColumnSchemaInfo(_targetDbConnectionString, targetTable);

            columnMappings = columnMappings.ValidateAndFormatColumnMappings(sourceColumns, targetColumns);

            var schemaExists = await SqlUtils.ExecuteScalar<bool>(_targetDbConnectionString,
                $@"SELECT CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{ts.TableSchema}') THEN 1 ELSE 0 END");
            if (!schemaExists)
            {
                await SqlUtils.ExecuteScalar<bool>(_targetDbConnectionString, $"CREATE SCHEMA {ts.TableSchema}");
            }
            var tableExists = await SqlUtils.ExecuteScalar<bool>(_targetDbConnectionString,
                $@"SELECT CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{ts.TableSchema}' AND  TABLE_NAME = '{ts.TableName}') THEN 1 ELSE 0 END");
            if (!tableExists)
            {
                var createTableScript = await GenerateCreateTableDdlScriptFromSource(sourceSchemaAndTable);
                await SqlUtils.ExecuteNonQueryScript(_targetDbConnectionString, $"Create Table {sourceSchemaAndTable}", createTableScript);
            }

            var tempTableExists = await SqlUtils.ExecuteScalar<bool>(_targetDbConnectionString,
                $@"SELECT CASE WHEN EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{ts.TableSchema}' AND  TABLE_NAME = '{GetTempTableName(targetSchemaAndTable)}') THEN 1 ELSE 0 END");
            if (tempTableExists)
            {
                await SqlUtils.ExecuteNonQuery(_targetDbConnectionString, $"DROP TABLE {GetTempTableSchemaAndName(targetSchemaAndTable)}");
            }
            await CreateTempTableInTarget(targetSchemaAndTable);

            var mergeScript = GenerateSqlMergeStatement(GetTempTableSchemaAndName(targetSchemaAndTable), targetSchemaAndTable,
                targetColumns, targetColumns, 
                columnMappings?.Select(a => a.Target).ToList() ?? null, 
                copyAndMergeConfig?.NoRowDeletion ?? false);

            var sqlBulkCopyQuery = new StringBuilder();
            sqlBulkCopyQuery.Append("SELECT ");
            var selectColumns = new List<string>();
            if (columnMappings?.Any() == true)
            {
                selectColumns.AddRange(columnMappings.Select(a => a.Source));
            }
            else
            {
                foreach (var targetColumn in targetColumns)
                {
                    if (sourceColumns.Any(a => a.Name == targetColumn.Name))
                    {
                        selectColumns.Add(targetColumn.Name.AddSquareBrackets());
                    }
                }
                if (!selectColumns.Any())
                    throw new Exception("No column names between the source and target tables match");
            }
            sqlBulkCopyQuery.AppendLine(string.Join(", ", selectColumns));
            sqlBulkCopyQuery.AppendLine($"FROM {sourceSchemaAndTable}");

            var copyResult = await SqlBulkCopy(sqlBulkCopyQuery.ToString(), GetTempTableSchemaAndName(targetSchemaAndTable), cancellationToken,
                columnMappings: columnMappings ?? selectColumns.Select(a => new ColumnMapping(a,a)));

            var mergeResult = new MergeResult();
            if (copyResult > 0)
            {
                mergeResult = await ExecuteMergeStatement(mergeScript, cancellationToken, copyAndMergeConfig?.SqlMergeConfig?.StatementTimeout ?? 3600);
            }
            else if (copyAndMergeConfig?.SqlBulkCopyConfig?.ThrowExceptionIfCopyResultsInZeroRows == true)
            {
                throw new Exception("No rows copied from source to the temp table");
            }

            await SqlUtils.ExecuteNonQuery(_targetDbConnectionString, $"DROP TABLE {GetTempTableSchemaAndName(targetSchemaAndTable)}");

            return mergeResult;
        }

        /// <inheritdoc />
        public async Task<int> SqlBulkCopy(string sourceSelectQuery, string targetTable,
            IEnumerable<SqlParameter> sqlParameters = null, 
            IEnumerable<ColumnMapping> columnMappings = null,
            SqlBulkCopyConfig sqlBulkCopyConfig = null)
        {
            return await SqlBulkCopy(sourceSelectQuery, targetTable, CancellationToken.None,
                sqlParameters, columnMappings, sqlBulkCopyConfig);
        }

        /// <inheritdoc />
        public async Task<int> SqlBulkCopy(string sourceSelectQuery, string targetTable,
            CancellationToken cancellationToken,
            IEnumerable<SqlParameter> sqlParameters = null, 
            IEnumerable<ColumnMapping> columnMappings = null,
            SqlBulkCopyConfig sqlBulkCopyConfig = null)
        {
            var targetSchemaAndTableName = targetTable.FormatTableSchemaAndName();
            
            await using var sourceConnection = new SqlConnection(_sourceDbConnectionString);
            await sourceConnection.OpenAsync(cancellationToken);
            
            var commandSourceData = new SqlCommand(sourceSelectQuery, sourceConnection)
            {
                CommandTimeout = sqlBulkCopyConfig?.ReaderCommandTimeout ??
                                 Convert.ToInt32(TimeSpan.FromMinutes(3).TotalSeconds)
            };
            if (sqlParameters?.Any() == true)
            {
                commandSourceData.Parameters.AddRange(sqlParameters.ToArray());
            }

            await using var reader = await commandSourceData.ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);

            await using var targetConnection = new SqlConnection(_targetDbConnectionString);
            await targetConnection.OpenAsync(cancellationToken);

            // Set up the bulk copy object.
            using var bulkCopy = new SqlBulkCopy(targetConnection,
                sqlBulkCopyConfig?.CopyOption ?? SqlBulkCopyOptions.KeepIdentity,
                null)
            {
                BulkCopyTimeout = sqlBulkCopyConfig?.BulkCopyTimeout ?? Convert.ToInt32(TimeSpan.FromHours(1).TotalSeconds),
                DestinationTableName = targetSchemaAndTableName,
                BatchSize = sqlBulkCopyConfig?.BatchSize ?? 0
            };

            // Note if mapped columns aren't specified, the columns must match in name and order
            if (columnMappings?.Any() == true)
            {
                foreach (var columnMapping in columnMappings)
                {
                    bulkCopy.ColumnMappings.Add(columnMapping.Source, columnMapping.Target);
                }
            }
            
            await bulkCopy.WriteToServerAsync(reader, cancellationToken);
            
            return bulkCopy.RowsCopied;
        }

        private string GenerateSqlMergeStatement(string sourceSchemaAndTable,
            string targetSchemaAndTable, 
            List<ColumnSchemaModel> sourceColumnSchemaInfo, 
            List<ColumnSchemaModel> targetColumnSchemaInfo,
            List<string> columnsToInclude = null,
            bool doNotDeleteRows = false)
        {
            var columnsToUseInMerge = new List<ColumnSchemaModel>();
            foreach (var column in targetColumnSchemaInfo)
            {
                if (column.IsComputed) continue;

                if (columnsToInclude?.Any() == true
                    && !columnsToInclude.Any(a => string.Equals(a, column.Name)))
                    continue;

                if (sourceColumnSchemaInfo.Any(a =>
                        string.Equals(a.Name, column.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    columnsToUseInMerge.Add(column);
                }
            }

            const string labelSource = "[Source]";
            const string labelTarget = "[Target]";
            var primaryKeys = columnsToUseInMerge.Where(c => c.IsPrimaryKey).ToList();
            var hasPrimaryKeys = primaryKeys.Any();
            if (!hasPrimaryKeys)
            {
                primaryKeys = columnsToUseInMerge;
            }

            var b = new StringBuilder();
            b.AppendLine("SET NOCOUNT ON");
            b.AppendLine(string.Empty);
            b.AppendLine("CREATE TABLE #ChangesFromMerge (Change NVARCHAR(50))");
            b.AppendLine($"MERGE INTO {targetSchemaAndTable} AS {labelTarget}");
            b.AppendLine($"USING {sourceSchemaAndTable} AS {labelSource}");
            b.Append($"ON (");
            var primaryKeyMatches = new List<string>();
            foreach (var column in primaryKeys)
            {
                switch (column.Type)
                {
                    case SqlDbType.Text:
                    case SqlDbType.NText:
                        primaryKeyMatches.Add(
                            $"CAST({labelTarget}.[{column.Name}] AS VARCHAR(MAX))=CAST({labelSource}.[{column.Name}] AS VARCHAR(MAX))");
                        break;
                    default:
                        primaryKeyMatches.Add(
                            $"{labelTarget}.[{column.Name}]={labelSource}.[{column.Name}]");
                        break;
                }
            }
            b.Append(string.Join(" AND ", primaryKeyMatches));
            b.AppendLine(")");

            if (hasPrimaryKeys && columnsToUseInMerge.Count == primaryKeys.Count)
            {
                // Don't include an update section if all the columns are primary key columns
            }
            else
            {
                b.AppendLine("WHEN MATCHED AND (");
                var updateMatches = new List<string>();
                foreach (var column in columnsToUseInMerge)
                {
                    if (!column.IsIdentity && !(hasPrimaryKeys && column.IsPrimaryKey))
                    {
                        switch (column.Type)
                        {
                            case SqlDbType.Text:
                                updateMatches.Add(
                                    $"	NULLIF(CAST({labelSource}.[{column.Name}] AS VARCHAR(MAX)), CAST({labelTarget}.[{column.Name}] AS VARCHAR(MAX))) IS NOT NULL OR NULLIF(CAST({labelTarget}.[{column.Name}] AS VARCHAR(MAX)), CAST({labelSource}.[{column.Name}] AS VARCHAR(MAX))) IS NOT NULL");
                                break;
                            case SqlDbType.NText:
                                updateMatches.Add(
                                    $"	NULLIF(CAST({labelSource}.[{column.Name}] AS NVARCHAR(MAX)), CAST({labelTarget}.[{column.Name}] AS NVARCHAR(MAX))) IS NOT NULL OR NULLIF(CAST({labelTarget}.[{column.Name}] AS NVARCHAR(MAX)), CAST({labelSource}.[{column.Name}] AS NVARCHAR(MAX))) IS NOT NULL");
                                break;
                            default:
                                updateMatches.Add(
                                    $"	NULLIF({labelSource}.[{column.Name}], {labelTarget}.[{column.Name}]) IS NOT NULL OR NULLIF({labelTarget}.[{column.Name}], {labelSource}.[{column.Name}]) IS NOT NULL");
                                break;
                        }
                    }
                }
                b.Append(string.Join(" OR " + Environment.NewLine, updateMatches));
                b.AppendLine(") THEN");

                b.AppendLine(" UPDATE SET");
                var updateMatchSets = new List<string>();
                foreach (var column in columnsToUseInMerge)
                {
                    if (!column.IsIdentity)
                    {
                        updateMatchSets.Add($"  {labelTarget}.[{column.Name}] = {labelSource}.[{column.Name}]");
                    }
                }
                b.AppendLine(string.Join(", " + Environment.NewLine, updateMatchSets));
            }

            b.AppendLine("WHEN NOT MATCHED BY TARGET THEN");
            b.AppendLine($" INSERT({string.Join(",", columnsToUseInMerge.Where(a => !a.IsIdentity).Select(a => "[" + a.Name + "]"))})");
            b.AppendLine($" VALUES({string.Join(",", columnsToUseInMerge.Where(a => !a.IsIdentity).Select(a => labelSource + ".[" + a.Name + "]"))})");
            if (!doNotDeleteRows)
            {
                b.AppendLine("WHEN NOT MATCHED BY SOURCE THEN ");
                b.AppendLine(" DELETE");
            }

            b.AppendLine(" OUTPUT $ACTION INTO #ChangesFromMerge;");
            b.AppendLine("DECLARE @mergeError int");
            b.AppendLine(" , @mergeCount int");
            b.AppendLine("SELECT @mergeError = @@ERROR, @mergeCount = @@ROWCOUNT");
            b.AppendLine("IF @mergeError != 0");
            b.AppendLine(" BEGIN");
            b.AppendLine(
                $" PRINT 'ERROR OCCURRED IN MERGE FOR {targetSchemaAndTable}. Rows affected: ' + CAST(@mergeCount AS VARCHAR(100)); -- SQL should always return zero rows affected");
            b.AppendLine(" END");
            b.AppendLine("ELSE");
            b.AppendLine(" BEGIN");
            b.AppendLine(
                $" PRINT 'MERGE Succeeded. {targetSchemaAndTable} rows affected by MERGE: ' + CAST(@mergeCount AS VARCHAR(100));");
            b.AppendLine(" END");
            b.AppendLine("GO");
            b.AppendLine(string.Empty);
            b.AppendLine(string.Empty);
            b.AppendLine("SET NOCOUNT OFF");
            b.AppendLine("GO");

            var rawSql = b.ToString();
            return rawSql;
        }

        private async Task<MergeResult> ExecuteMergeStatement(string mergeStatement, CancellationToken cancellationToken, int statementTimeout = 3600)
        {
            var scriptName = "MERGE statement";
            var mergeResult = new MergeResult
            {
                Inserted = 0,
                Updated = 0,
                Deleted = 0
            };

            await using var connection = new SqlConnection(_targetDbConnectionString);
            connection.InfoMessage += TargetConnectionInfoMessage;
            void TargetConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
            {
                _logger.Verbose("SQL Output Message: {0}", e.Message);
            }
            
            _logger.Debug("Running {0} : {1}", scriptName, mergeStatement);
            var server = new Server(new ServerConnection(connection));
            server.ConnectionContext.StatementTimeout = statementTimeout;
            server.ConnectionContext.ExecuteNonQuery(mergeStatement);
            var cmd = new SqlCommand("SELECT Change, COUNT(*) FROM #ChangesFromMerge GROUP BY Change",
                connection);
            var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (reader.Read())
            {
                var change = reader.GetString(0);
                var count = reader.GetInt32(1);
                if (change == "INSERT")
                {
                    mergeResult.Inserted = count;
                }
                else if (change == "UPDATE")
                {
                    mergeResult.Updated = count;
                }
                else if (change == "DELETE")
                {
                    mergeResult.Deleted = count;
                }
                else
                {
                    _logger.Warning("Unrecognised result from MERGE Query : {0}", change);
                }
            }

            _logger.Debug("Completed {0} : {1}", scriptName, mergeResult);

            connection.InfoMessage -= TargetConnectionInfoMessage;

            return mergeResult;
        }

        private async Task<string> GenerateCreateTableDdlScriptFromSource(string tableSchemaAndName)
        {
            await using var connection = new SqlConnection(_sourceDbConnectionString);

            var tableUrn = await GetUrn(_sourceDbConnectionString, tableSchemaAndName);

            var srv = new Server(new ServerConnection(connection));

            var scrp = new Scripter(srv)
            {
                Options =
                    {
                        ScriptForCreateDrop = true, // doesnt work
                        WithDependencies = false,
                        Indexes = true,
                        DriPrimaryKey = true,
                        DriForeignKeys = false,
                        DriClustered = true,
                        DriNonClustered = true,
                        FullTextCatalogs = false,
                        FullTextIndexes = false
                    }
            };
            scrp.Options.GetScriptingPreferences().Behavior = ScriptBehavior.DropAndCreate; // workaround because ScriptForCreateDrop doesnt work
            
            var sc = scrp.Script(new[] { tableUrn });
            var l = new List<string>();
            foreach (var st in sc)
            {
                if (st.StartsWith("DROP INDEX"))
                    continue; // ignore this
                l.Add(st);
            }
            if (!l.Any())
                throw new Exception($"DB table {tableSchemaAndName} not found. Cannot generate CREATE Table DDL Script");
            var script = string.Join(Environment.NewLine, l);
            return script;
        }

        private async Task CreateTempTableInTarget(string tableSchemaAndName)
        {
            await using var connection = new SqlConnection(_targetDbConnectionString);

            var tableUrn = await GetUrn(_targetDbConnectionString, tableSchemaAndName);

            var srv = new Server(new ServerConnection(connection));
            
            var scrp = new Scripter(srv)
            {
                Options =
                {
                    ScriptForCreateOrAlter = false,
                    ScriptForCreateDrop = false,
                    WithDependencies = false,
                    Indexes = false,
                    DriPrimaryKey = true,
                    DriForeignKeys = false,
                    DriClustered = false,
                    DriNonClustered = false,
                    FullTextCatalogs = false,
                    FullTextIndexes = false
                }
            };
            
            var sc = scrp.Script(new[] { tableUrn });
            var l = new List<string>();
            foreach (var st in sc)
            {
                l.Add(st);
            }
            if (!l.Any())
                throw new Exception($"DB table {tableSchemaAndName} not found. Cannot generate CREATE Temp Table Script");
            var createTempTableScript = string.Join(Environment.NewLine, l);

            var tempTableName = GetTempTableName(tableSchemaAndName);
            var tempTableSchemaAndName = GetTempTableSchemaAndName(tableSchemaAndName);

            // Rename the table to the temp table
            createTempTableScript = createTempTableScript
                .Replace($"CREATE TABLE {tableSchemaAndName}", $"CREATE TABLE {tempTableSchemaAndName}");

            // Replace Primary Key constraint
            var ts = tableSchemaAndName.GetTableSchemaAndNameInParts();
            var tempTablePrimaryKeyLabel = $"[{ts.TableSchema}_{tempTableName}_PK]";
            createTempTableScript = Regex.Replace(createTempTableScript, "CONSTRAINT [^ ]+ PRIMARY KEY",
                $"CONSTRAINT {tempTablePrimaryKeyLabel} PRIMARY KEY");

            // validate the table was renamed
            if (!(createTempTableScript.Contains($"CREATE TABLE {tempTableSchemaAndName}")
                  && (!createTempTableScript.Contains("PRIMARY KEY") || createTempTableScript.Contains($"CONSTRAINT {tempTablePrimaryKeyLabel} PRIMARY KEY")))
                || createTempTableScript.Contains(tableSchemaAndName))
            {
                throw new Exception($"The generated script to create the temp table contains unexpected data{Environment.NewLine}{createTempTableScript}");
            }
            
            srv.ConnectionContext.StatementTimeout = (int)TimeSpan.FromMinutes(3).TotalSeconds;
            srv.ConnectionContext.ExecuteNonQuery(createTempTableScript);
        }

        private string GetTempTableName(string table)
        {
            return table.GetTableSchemaAndNameInParts().TableName + "_temp";
        }

        private string GetTempTableSchemaAndName(string table)
        {
            var ts = table.GetTableSchemaAndNameInParts();
            return $"[{ts.TableSchema}].[{GetTempTableName(table)}]";
        }

        private async Task<Urn> GetUrn(string connectionString, string table)
        {
            var ts = table.GetTableSchemaAndNameInParts();
            var urnSelectQuery =
                $"SELECT 'Server[@Name=''' + @@SERVERNAME + ''']/Database[@Name=''' + DB_Name() + ''']/Table[@Name=''{ts.TableName}'' and @Schema=''{ts.TableSchema}'']'";
            var urnVal = await SqlUtils.ExecuteScalar<string>(connectionString, urnSelectQuery);
            return new Urn(urnVal);
        }

        private async Task<List<ColumnSchemaModel>> GetColumnSchemaInfo(string connectionString, string table)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var ts = table.GetTableSchemaAndNameInParts();

            var sql = $@"SELECT COLUMN_NAME, DATA_TYPE
,COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA+'.'+TABLE_NAME),COLUMN_NAME,'IsComputed') as IS_COMPUTED
,COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA+'.'+TABLE_NAME),COLUMN_NAME,'IsIdentity') as IS_IDENTITY
,CASE WHEN EXISTS (
    SELECT Col.Column_Name
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab
    JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col ON Col.Constraint_Name = Tab.Constraint_Name AND Col.TABLE_SCHEMA = Tab.TABLE_SCHEMA AND Col.Table_Name = Tab.Table_Name
    WHERE 
        Constraint_Type = 'PRIMARY KEY'
        AND Col.TABLE_SCHEMA = C.TABLE_SCHEMA
        AND Col.Table_Name = C.TABLE_NAME
        AND Col.COLUMN_NAME = C.COLUMN_NAME
) THEN 1 ELSE 0 END as IS_PRIMARY_KEY
FROM INFORMATION_SCHEMA.COLUMNS C
WHERE TABLE_SCHEMA = '{ts.TableSchema}' AND TABLE_NAME = '{ts.TableName}'
ORDER BY ORDINAL_POSITION";

            var command = new SqlCommand(sql, connection);
            
            var sqlDataReader = await command.ExecuteReaderAsync();
            
            var columns = new List<ColumnSchemaModel>();
            while (sqlDataReader.Read())
            {
                var column = new ColumnSchemaModel
                {
                    Name = sqlDataReader.GetString("COLUMN_NAME"),
                    IsIdentity = sqlDataReader.GetInt32("IS_IDENTITY") == 1,
                    IsComputed = sqlDataReader.GetInt32("IS_COMPUTED") == 1,
                    IsPrimaryKey = sqlDataReader.GetInt32("IS_PRIMARY_KEY") == 1
                };
                var dataType = sqlDataReader.GetString("DATA_TYPE");
                if (Enum.TryParse(typeof(SqlDbType), dataType, true, out var dataTypeParsed))
                {
                    column.Type = (SqlDbType)dataTypeParsed;
                }
                else
                {
                    // Unsupported column type
                    _logger.Warning("Unsupported column type: {0}. Ignoring", dataType);
                    continue;
                }
                columns.Add(column);
            }

            if (columns?.Any() != true)
            {
                throw new Exception("No columns returned");
            }

            return columns;
        }
    }
}
