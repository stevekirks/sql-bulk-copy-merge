using System;
using System.Threading.Tasks;
using SqlBulkCopyMerge.Tests.Docker;
using Xunit;
using Xunit.Abstractions;

namespace SqlBulkCopyMerge.Tests
{
    [Collection(nameof(DockerTestCollection))]
    public class CopyLatestTests
    {
        private readonly DockerFixture _dockerFixture;
        private readonly ISqlBulkCopyMergeService _sqlBulkCopyCautiouslyService;

        public CopyLatestTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;

            Log.Logger = new XUnitLogger(output);

            _sqlBulkCopyCautiouslyService = new SqlBulkCopyMergeService(_dockerFixture.SourceSqlDbConnectionString,
                _dockerFixture.TargetSqlDbConnectionString,
                Log.Logger);
        }

        [Fact]
        public async Task CopiesLatestRows()
        {
            var sourceTable = "[dbo].[test_copying_latest_rows]";
            var targetTable = sourceTable;
            var resultById = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable);
            Assert.Equal(1, resultById);
            
            var byDateColumn = "[timestamp]";
            var resultByDate = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable, byDateColumn);
            Assert.Equal(2, resultByDate);

            var byVersionControlColumn = "[version_control]";
            var resultByVersionControl = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable, byVersionControlColumn);
            Assert.Equal(1, resultByVersionControl);
        }
        
        [Fact]
        public async Task CopiesLatestRowsFromView()
        {
            var sourceView = "[dbo].[vtest_copying_latest_rows_from_view]";
            var targetTable = "[dbo].[test_copying_latest_rows_from_view]";
            var resultById = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceView, targetTable);
            Assert.Equal(2, resultById);
        }

        [Fact]
        public async Task CopiesLatestRowsWhereTargetIsEmpty()
        {
            var sourceTable = "[dbo].[test_copying_latest_rows_where_target_is_empty]";
            var targetTable = sourceTable;
            var resultById = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable);
            Assert.Equal(4, resultById);
        }
    }
}
