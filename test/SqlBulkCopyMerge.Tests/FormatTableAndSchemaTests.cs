using System;
using Microsoft.SqlServer.Management.Common;
using Xunit;

namespace SqlBulkCopyMerge.Tests
{
    public class FormatTableAndSchemaTests
    {
        [Fact]
        public void TableWithoutSchemaIsCorrect()
        {
            var t = "my table name";
            var r = t.FormatTableSchemaAndName();
            Assert.Equal("[dbo].[my table name]", r);
        }
        [Fact]
        public void TableWithSquareBracketsWithoutSchemaIsCorrect()
        {
            var t = "[my table name]";
            var r = t.FormatTableSchemaAndName();
            Assert.Equal("[dbo].[my table name]", r);
        }
        [Fact]
        public void TableWithSchemaIsCorrect()
        {
            var t = "my schema.my table name";
            var r = t.FormatTableSchemaAndName();
            Assert.Equal("[my schema].[my table name]", r);
        }
        [Fact]
        public void TableWithSquareBracketsIsCorrect()
        {
            var t = "[my schema].[my table name]";
            var r = t.FormatTableSchemaAndName();
            Assert.Equal("[my schema].[my table name]", r);
        }
        [Fact]
        public void TableWithSquareBracketsOnSchemaOnlyIsCorrect()
        {
            var t = "[my schema].my table name";
            var r = t.FormatTableSchemaAndName();
            Assert.Equal("[my schema].[my table name]", r);
        }
        [Fact]
        public void TableWithSquareBracketsOnTableOnlyIsCorrect()
        {
            var t = "my schema.[my table name]";
            var r = t.FormatTableSchemaAndName();
            Assert.Equal("[my schema].[my table name]", r);
        }
        [Fact]
        public void EmptyTableNameFails()
        {
            var t = "";
            Assert.Throws<ArgumentNullException>(() =>
            {
                t.FormatTableSchemaAndName();
            });
        }
        [Fact]
        public void InvalidTableNameFails()
        {
            var t = "[]";
            Assert.Throws<InvalidArgumentException>(() =>
            {
                t.FormatTableSchemaAndName();
            });
        }
        [Fact]
        public void TableWithDatabaseFails()
        {
            var t = "my database.my schema.my table name";
            Assert.Throws<InvalidArgumentException>(() =>
            {
                t.FormatTableSchemaAndName();
            });
        }
    }
}
