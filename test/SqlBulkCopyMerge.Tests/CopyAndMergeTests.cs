using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
