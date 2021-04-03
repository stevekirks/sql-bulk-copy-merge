namespace SqlBulkCopyMerge.Models
{
    public class SqlMergeConfig
    {
        /// <summary>
        /// Set a timeout in seconds for running the MERGE statement
        /// </summary>
        public int StatementTimeout { get; set; }
    }
}
