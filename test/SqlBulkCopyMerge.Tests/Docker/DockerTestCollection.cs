using Xunit;

namespace SqlBulkCopyMerge.Tests.Docker
{
    [CollectionDefinition(nameof(DockerTestCollection))]
    public class DockerTestCollection : ICollectionFixture<DockerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
