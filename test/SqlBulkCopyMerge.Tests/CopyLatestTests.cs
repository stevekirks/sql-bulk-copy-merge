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
        public async Task CopiesLatestRowsByInt()
        {
            var sourceTable = "[dbo].[test_copying_latest_rows_by_int]";
            var targetTable = sourceTable;
            var rowCount = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable);
            Assert.Equal(1, rowCount);

            var targetRows = await TestUtils.QueryTable(_dockerFixture.TargetSqlDbConnectionString,
                $"SELECT [id], [notes], [timestamp] FROM {targetTable}");
            Assert.Collection(targetRows, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 1", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(2, row[0]);
                Assert.Equal("Note 2", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(3, row[0]);
                Assert.Equal("Note 3", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(4, row[0]);
                Assert.Equal("Note 4", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            });
        }

        [Fact]
        public async Task CopiesLatestRowsByDate()
        {
            var sourceTable = "[dbo].[test_copying_latest_rows_by_date]";
            var targetTable = sourceTable;
            var keyColumn = "[timestamp]";
            var rowCount = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable, keyColumn);
            Assert.Equal(3, rowCount);

            var targetRows = await TestUtils.QueryTable(_dockerFixture.TargetSqlDbConnectionString,
                $"SELECT [id], [notes], [timestamp] FROM {targetTable}");
            Assert.Collection(targetRows, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 1", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 2", row[1]);
                Assert.Equal(2, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 3", row[1]);
                Assert.Equal(3, ((DateTime)row[2]).Day);
            }, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 4", row[1]);
                Assert.Equal(4, ((DateTime)row[2]).Day);
            });
        }

        [Fact]
        public async Task CopiesLatestRowsByBinary()
        {
            var sourceTable = "[dbo].[test_copying_latest_rows_by_binary]";
            var targetTable = sourceTable;
            var keyColumn = "[version_control]";
            var rowCount = await _sqlBulkCopyCautiouslyService.CopyLatest(sourceTable, targetTable, keyColumn);
            Assert.Equal(2, rowCount);

            var targetRows = await TestUtils.QueryTable(_dockerFixture.TargetSqlDbConnectionString,
                $"SELECT [id], [notes], [timestamp], [version_control] FROM {targetTable}");
            Assert.Collection(targetRows, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 1", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
                Assert.Equal(1, ((byte[])row[3])[3]);
            }, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 2", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
                Assert.Equal(2, ((byte[])row[3])[3]);
            }, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 3", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
                Assert.Equal(3, ((byte[])row[3])[3]);
            }, row =>
            {
                Assert.Equal(1, row[0]);
                Assert.Equal("Note 4", row[1]);
                Assert.Equal(1, ((DateTime)row[2]).Day);
                Assert.Equal(4, ((byte[])row[3])[3]);
            });
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
