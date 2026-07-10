namespace GenAIPlatform.IntegrationTests;

public sealed class TestIsolationTests
{
    [Fact]
    public void HostCompositionTests_AreIsolatedFromParallelCollections()
    {
        Assert.Single(
            typeof(HostCompositionTests).GetCustomAttributes(
                typeof(CollectionAttribute<CurrentDirectorySensitiveCollection>),
                inherit: false));

        var collectionDefinition = Assert.IsType<CollectionDefinitionAttribute>(
            Assert.Single(
                typeof(CurrentDirectorySensitiveCollection).GetCustomAttributes(
                    typeof(CollectionDefinitionAttribute),
                    inherit: false)));

        Assert.True(collectionDefinition.DisableParallelization);
    }
}
