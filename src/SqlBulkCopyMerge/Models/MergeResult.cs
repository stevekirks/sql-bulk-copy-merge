namespace SqlBulkCopyMerge.Models
{
    public class MergeResult
    {
        public int Inserted { get; set; } = 0;
        public int Updated { get; set; } = 0;
        public int Deleted { get; set; } = 0;
    }
}
