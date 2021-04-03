using Microsoft.Data.SqlClient;

namespace SqlBulkCopyMerge.Models
{
    public class SqlBulkCopyConfig
    {
        /// <summary>
        /// Throw an exception if after the Copy there are 0 rows in the target.
        /// If false and there are 0 rows, then MERGE is skipped
        /// </summary>
        public bool ThrowExceptionIfCopyResultsInZeroRows { get; set; }

        public int? ReaderCommandTimeout { get; set; }

        public int? BulkCopyTimeout { get; set; }

        public int? BatchSize { get; set; }

        public SqlBulkCopyOptions? CopyOption { get; set; }
    }
}
