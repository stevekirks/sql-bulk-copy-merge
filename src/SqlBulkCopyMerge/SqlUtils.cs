using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SqlBulkCopyMerge.Tests")]
namespace SqlBulkCopyMerge
{
    internal static class SqlUtils
    {
        internal static async Task ExecuteNonQuery(string dbConnectionString, string cmdText, int commandTimeout = 180)
        {
            await using var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(cmdText, connection)
            {
                CommandTimeout = commandTimeout
            };
            await command.ExecuteNonQueryAsync();
        }

        internal static async Task<int> ExecuteNonQueryScript(string dbConnectionString, string scriptName, string script, int statementTimeout = 3600, ILogger logger = null)
        {
            var internalLogger = logger ?? Log.Logger;
            await using var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync();
            connection.InfoMessage += TargetConnectionInfoMessage;
            void TargetConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
            {
                internalLogger.Verbose("{0} SQL Output Message: {1}", scriptName, e.Message);
            }

            internalLogger.Verbose("Running {0}", scriptName);

            var server = new Server(new ServerConnection(connection));

            server.ConnectionContext.StatementTimeout = statementTimeout;

            var rowsAffected = server.ConnectionContext.ExecuteNonQuery(script);

            internalLogger.Debug("Completed {0} : {1}", scriptName, rowsAffected);

            connection.InfoMessage -= TargetConnectionInfoMessage;

            return rowsAffected;
        }

        internal static async Task<object> ExecuteScalar(string dbConnectionString, string cmdText, int commandTimeout = 180)
        {
            await using var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(cmdText, connection)
            {
                CommandTimeout = commandTimeout
            };
            var result = await command.ExecuteScalarAsync();
            return result;
        }

        internal static async Task<T> ExecuteScalar<T>(string dbConnectionString, string cmdText, int commandTimeout = 180)
        {
            await using var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(cmdText, connection)
            {
                CommandTimeout = commandTimeout
            };
            var result = await command.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result, typeof(T));
        }

        internal static async Task<T> ExecuteScalarScript<T>(string dbConnectionString, string scriptName, string script, int statementTimeout = 3600, ILogger logger = null)
        {
            var internalLogger = logger ?? Log.Logger;
            await using var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync();
            connection.InfoMessage += TargetConnectionInfoMessage;
            void TargetConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
            {
                internalLogger.Verbose("{0}. SQL Output Message: {1}", scriptName, e.Message);
            }

            var server = new Server(new ServerConnection(connection));

            server.ConnectionContext.StatementTimeout = statementTimeout;

            var result = server.ConnectionContext.ExecuteScalar(script);
            var typedResult = (T)Convert.ChangeType(result, typeof(T));

            connection.InfoMessage -= TargetConnectionInfoMessage;

            return typedResult;
        }
    }
}
