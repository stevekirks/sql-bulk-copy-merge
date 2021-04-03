namespace SqlBulkCopyMerge.Models
{
    public class CopyAndMergeConfig
    {
        /// <summary>
        /// Set to true if the MERGE is only to Insert and Update rows, not Delete
        /// </summary>
        public bool NoRowDeletion { get; set; }

        public SqlBulkCopyConfig SqlBulkCopyConfig { get; set; }
        public SqlMergeConfig SqlMergeConfig { get; set; }
    }
}
