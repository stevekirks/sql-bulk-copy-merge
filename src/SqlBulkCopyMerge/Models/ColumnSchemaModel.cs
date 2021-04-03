using System.Data;

namespace SqlBulkCopyMerge.Models
{
    public class ColumnSchemaModel
    {
        public string Name { get; set; }
        public SqlDbType Type { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComputed { get; set; }
    }
}
