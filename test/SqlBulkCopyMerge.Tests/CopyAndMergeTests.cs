using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SqlBulkCopyMerge.Models;
using SqlBulkCopyMerge.Tests.Docker;
using Xunit;
using Xunit.Abstractions;

namespace SqlBulkCopyMerge.Tests
{
    [Collection(nameof(DockerTestCollection))]
    public class CopyAndMergeTests
    {
        private readonly DockerFixture _dockerFixture;
        private readonly ISqlBulkCopyMergeService _sqlBulkCopyCautiouslyService;

        public CopyAndMergeTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;

            Log.Logger = new XUnitLogger(output);

            _sqlBulkCopyCautiouslyService = new SqlBulkCopyMergeService(_dockerFixture.SourceSqlDbConnectionString,
                _dockerFixture.TargetSqlDbConnectionString,
                Log.Logger);
        }

        [Fact]
        public async Task CopiesAndMergesCorrectly()
        {
            var sourceTable = "[dbo].[test_copy_and_merge]";
            var targetTable = sourceTable;
            var result = await _sqlBulkCopyCautiouslyService.CopyAndMerge(sourceTable, targetTable);
            Assert.Equal(1, result.Inserted);
            Assert.Equal(2, result.Updated);
            Assert.Equal(1, result.Deleted);

            var targetRows = await TestUtils.QueryTable(_dockerFixture.TargetSqlDbConnectionString,
                $"SELECT [id], [notes], [timestamp] FROM {targetTable}");
            Assert.Collection(targetRows, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 1 unchanged", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(2, row[0]);
                Assert.Equal("Note 2 overwrite", row[1]);
                Assert.Equal(2, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(3, row[0]);
                Assert.Equal("Note 3 new", row[1]);
                Assert.Equal(3, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(4, row[0]);
                Assert.Equal(DBNull.Value, row[1]);
                Assert.Equal(4, ((DateTime)row[2]).Day);
            });
        }

        [Fact]
        public async Task CopiesAndMergesFromViewWithSubsetOfColumnsCorrectly()
        {
            var sourceTable = "vtest_copy_and_merge_subset";
            var targetTable = "dbo.test_copy_and_merge_subset";
            var result = await _sqlBulkCopyCautiouslyService.CopyAndMerge(sourceTable, targetTable);
            Assert.Equal(1, result.Inserted);
            Assert.Equal(1, result.Updated);
            Assert.Equal(3, result.Deleted);
        }

        [Fact]
        public async Task CopiesAndMergesWithDeletesDisabled()
        {
            var sourceTable = "test_copy_and_merge_with_deletes_disabled";
            var targetTable = sourceTable;
            var config = new CopyAndMergeConfig
            {
                NoRowDeletion = true
            };
            var result = await _sqlBulkCopyCautiouslyService.CopyAndMerge(sourceTable, targetTable, copyAndMergeConfig: config);
            Assert.Equal(1, result.Inserted);
            Assert.Equal(1, result.Updated);
            Assert.Equal(0, result.Deleted);

            var targetRows = await TestUtils.QueryTable(_dockerFixture.TargetSqlDbConnectionString,
                $"SELECT [id], [notes], [timestamp] FROM {targetTable}");
            Assert.Collection(targetRows, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note wont be deleted even though it doesnt exist in source", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(2, row[0]);
                Assert.Equal("Note 2 updated", row[1]);
                Assert.Equal(2, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(3, row[0]);
                Assert.Equal("Note 3 new", row[1]);
                Assert.Equal(3, ((DateTime)row[2]).Day);
            });
        }

        [Fact]
        public async Task CopiesAndMergesWithDifferentColumnNames()
        {
            var sourceTable = "test_copy_and_merge_with_different_column_names";
            var targetTable = "test_copy_and_merge_with_different_column_names_d";
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping("id", "d_id"),
                new ColumnMapping("notes", "[d_notes]"),
                new ColumnMapping("[timestamp]", "d_timestamp"),
                new ColumnMapping("[version_control]", "[d_version_control]")
            };
            var result = await _sqlBulkCopyCautiouslyService.CopyAndMerge(sourceTable, targetTable, columnMappings);
            Assert.Equal(1, result.Inserted);
            Assert.Equal(1, result.Updated);
            Assert.Equal(1, result.Deleted);
        }
        
        [Fact]
        public async Task CopiesAndMergesWhereSourceIsEmpty()
        {
            var sourceTable = "test_copy_and_merge_where_source_is_empty";
            var targetTable = sourceTable;
            var result = await _sqlBulkCopyCautiouslyService.CopyAndMerge(sourceTable, targetTable);
            Assert.Equal(0, result.Inserted);
            Assert.Equal(0, result.Updated);
            Assert.Equal(0, result.Deleted);

            // Target rows are not deleted when the source query returns nothing
            var targetRows = await TestUtils.QueryTable(_dockerFixture.TargetSqlDbConnectionString,
                $"SELECT [id], [notes] FROM {targetTable}");
            Assert.Collection(targetRows, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 1 not deleted", row[1]);
            });
        }
    }
}
