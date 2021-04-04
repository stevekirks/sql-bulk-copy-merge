using System;
using System.IO;
using System.Threading;

namespace SqlBulkCopyMerge.Tests.Docker
{
    public class DockerFixture : IDisposable
    {
        public string SourceSqlDbConnectionString;
        public string TargetSqlDbConnectionString;

        private readonly DockerSetup _dockerSetup;

        public DockerFixture()
        {
            var seedDatabasesScript = File.ReadAllText("Docker/SeedDatabases.sql");

            _dockerSetup = new DockerSetup();
            _dockerSetup.RunDockerContainer();
            var dbStarting = true;
            var retryCount = 0;
            while (dbStarting && retryCount < 10)
            {
                try
                {
                    retryCount++;
                    dbStarting = false;
                    SqlUtils.ExecuteScalarScript<int>(_dockerSetup.GetMasterDbConnectionString(),
                        "Database running", "SELECT 1").GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains(
                        "A connection was successfully established with the server, but then an error occurred during the pre-login handshake")
                    || e.Message.Contains("Failed to connect to server ")
                    )
                    {
                        // Need to allow for SQL Server application to be ready
                        Thread.Sleep(5000);
                        dbStarting = true;
                    }
                }
            }
            SqlUtils.ExecuteNonQueryScript(_dockerSetup.GetMasterDbConnectionString(),
                "Seed Test Databases", seedDatabasesScript).GetAwaiter().GetResult();
            SourceSqlDbConnectionString = _dockerSetup.GetSourceDbConnectionString();
            TargetSqlDbConnectionString = _dockerSetup.GetTargetDbConnectionString();
        }

        public void Dispose()
        {
            //_dockerSetup.RemoveDockerContainer();
        }
    }
}
