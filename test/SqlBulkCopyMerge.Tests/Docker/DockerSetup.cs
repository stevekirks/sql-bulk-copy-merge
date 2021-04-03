using System;
using System.Diagnostics;
using System.Text;

namespace SqlBulkCopyMerge.Tests.Docker
{
    public class DockerSetup
    {
        private const string SqlServerMasterDbName = "master";
        private const string SqlServerSourceDbName = "sourcedb";
        private const string SqlServerTargetDbName = "targetdb";
        // These are specified in the docker-compose.yml file
        private const int SqlServerDbPort = 41433;
        private const string SqlServerSaPassword = "thisStrong(!)Password";
        private const string DockerFolder = "Docker";

        public void RunDockerContainer()
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = "up -d",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = DockerFolder
                });

            // wait max 5 minutes - it can take a little while to download the image to the local cache
            proc.WaitForExit(1000 * 60 * 5);

            if (proc.ExitCode != 0)
            {
                var b = new StringBuilder();
                b.AppendLine("Failed to start docker containers");
                b.AppendLine("StdOut:");
                b.AppendLine(proc.StandardOutput.ReadToEnd());
                b.AppendLine("StdErr:");
                b.AppendLine(proc.StandardError.ReadToEnd());
                Console.Write(b.ToString());
                throw new Exception(b.ToString());
            }

            Console.WriteLine("Containers running");
        }

        public void RemoveDockerContainer()
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = "down",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = DockerFolder
                });

            proc.WaitForExit(1000 * 60);
        }

        public string GetMasterDbConnectionString()
        {
            return GetDbConnectionString(SqlServerMasterDbName, SqlServerDbPort);
        }

        public string GetSourceDbConnectionString()
        {
            return GetDbConnectionString(SqlServerSourceDbName, SqlServerDbPort);
        }

        public string GetTargetDbConnectionString()
        {
            return GetDbConnectionString(SqlServerTargetDbName, SqlServerDbPort);
        }
        
        private string GetDbConnectionString(string databaseName, int port)
        {
            var connectionString =
                $"Server=tcp:localhost,{port};Database={databaseName};User ID=sa;Password={SqlServerSaPassword};Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;MultipleActiveResultSets=True;";
            return connectionString;
        }
    }
}
