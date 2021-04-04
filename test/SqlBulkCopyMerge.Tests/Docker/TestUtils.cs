using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SqlBulkCopyMerge.Tests.Docker
{
    public class TestUtils
    {
        public static async Task<List<List<object>>> QueryTable(string connectionString, string sql)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(sql, connection);
            var reader = await command.ExecuteReaderAsync();
            var result = new List<List<object>>();
            while (reader.Read())
            {
                var r = new List<object>();
                for (var i = 0; i < reader.VisibleFieldCount; i++)
                {
                    r.Add(reader.GetValue(i));
                }
                result.Add(r);
            }
            return result;
        }
    }
}
