namespace SqlBulkCopyMerge.Models
{
    public class CopyAndMergeConfig
    {
        public SqlBulkCopyConfig SqlBulkCopyConfig { get; set; }
        public SqlMergeConfig SqlMergeConfig { get; set; }
    }
}
