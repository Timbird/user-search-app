using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserSearch.Api.IntegrationTests.Infrastructure;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly IContainer _esContainer;
    private string _esUri = string.Empty;

    public IntegrationTestFactory()
    {
        _esContainer = new ContainerBuilder()
            .WithImage("docker.elastic.co/elasticsearch/elasticsearch:9.4.2")
            .WithEnvironment("discovery.type", "single-node")
            .WithEnvironment("xpack.security.enabled", "false")
            .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")
            .WithPortBinding(9200, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPath("/_cluster/health").ForPort(9200)))
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Elasticsearch:Uri"] = _esUri
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _esContainer.StartAsync();
        _esUri = $"http://{_esContainer.Hostname}:{_esContainer.GetMappedPublicPort(9200)}";

        // Trigger app startup — creates the index and seeds the 12 users
        CreateClient();

        // Force a refresh so seeded documents are immediately searchable in tests
        await ForceRefreshAsync();
    }

    public async Task ForceRefreshAsync()
    {
        var esClient = Services.GetRequiredService<ElasticsearchClient>();
        await esClient.Indices.RefreshAsync("users");
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _esContainer.DisposeAsync();
    }
}
