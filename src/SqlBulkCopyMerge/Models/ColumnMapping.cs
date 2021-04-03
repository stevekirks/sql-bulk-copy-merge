namespace SqlBulkCopyMerge.Models
{
    public class ColumnMapping
    {
        public ColumnMapping() { }
        public ColumnMapping(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public string Source { get; set; }
        public string Target { get; set; }
    }
}
