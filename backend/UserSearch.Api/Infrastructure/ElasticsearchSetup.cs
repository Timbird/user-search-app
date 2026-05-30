using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using UserSearch.Api.Models;

namespace UserSearch.Api.Infrastructure;

public static class ElasticsearchSetup
{
    private const string IndexName = "users";

    private static readonly UserDocument[] SeedUsers =
    [
        new() { Id = Guid.NewGuid().ToString(), FirstName = "David",   LastName = "Jones",        FullName = "David Jones",        JobTitle = "Developer",                    Phone = "07789 543768", Email = "djones@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Lisa",    LastName = "Holmes",       FullName = "Lisa Holmes",        JobTitle = "Development Lead",             Phone = "07756 896512", Email = "lholmes@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Alex",    LastName = "Smith",        FullName = "Alex Smith",         JobTitle = "QA Lead",                      Phone = "07723 743289", Email = "asmith@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Kieran",  LastName = "James",        FullName = "Kieran James",       JobTitle = "Developer",                    Phone = "07898 654123", Email = "kjames@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Gavin",   LastName = "Miles",        FullName = "Gavin Miles",        JobTitle = "UX Designer",                  Phone = "07881 987554", Email = "gmiles@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Kathy",   LastName = "Smith",        FullName = "Kathy Smith",        JobTitle = "UX Lead",                      Phone = "07765 332287", Email = "ksmith@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Phil",    LastName = "Walker",       FullName = "Phil Walker",        JobTitle = "Senior QA",                    Phone = "07889 984447", Email = "pwalker@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Rebecca", LastName = "Bates",        FullName = "Rebecca Bates",      JobTitle = "Product Development Manager",  Phone = "07798 548733", Email = "rbates@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Hayley",  LastName = "Walker-Smith", FullName = "Hayley Walker-Smith",JobTitle = "Developer",                    Phone = "07888 932145", Email = "hwalker@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Alexis",  LastName = "Crawley",      FullName = "Alexis Crawley",     JobTitle = "DevOps Engineer",              Phone = "07778 667412", Email = "acrawley@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "David",   LastName = "Gold",         FullName = "David Gold",         JobTitle = "DevOps Engineer",              Phone = "07768 479563", Email = "dgold@test.com" },
        new() { Id = Guid.NewGuid().ToString(), FirstName = "Phillipa",LastName = "Walker",       FullName = "Phillipa Walker",    JobTitle = "QA Lead",                      Phone = "07775 357951", Email = "pwalker2@test.com" },
    ];

    public static async Task EnsureIndexAsync(ElasticsearchClient client, ILogger logger)
    {
        await CreateIndexIfNullAsync(client, logger);
        await SeedIfEmptyAsync(client, logger);
    }

    private static async Task CreateIndexIfNullAsync(ElasticsearchClient client, ILogger logger)
    {
        if (await client.Indices.ExistsAsync(IndexName) is { Exists: true })
        {
            logger.LogInformation("Index '{Index}' already exists — skipping creation.", IndexName);
            return;
        }
        logger.LogInformation("Creating Elasticsearch index '{Index}'.", IndexName);

        var createResponse = await client.Indices.CreateAsync(new CreateIndexRequest(IndexName)
        {
            Settings = new IndexSettings
            {
                Analysis = new IndexSettingsAnalysis
                {
                    Analyzers = new Analyzers
                    {
                        ["name_autocomplete"] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            Filter = ["lowercase", "name_edge_ngram"]
                        },
                        ["name_search"] = new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            Filter = ["lowercase"]
                        }
                    },
                    TokenFilters = new TokenFilters
                    {
                        ["name_edge_ngram"] = new EdgeNGramTokenFilter
                        {
                            MinGram = 2,
                            MaxGram = 20
                        }
                    }
                }
            },
            Mappings = new TypeMapping
            {
                Properties = new Properties
                {
                    ["id"]        = new KeywordProperty(),
                    ["firstName"] = new TextProperty { Analyzer = "name_autocomplete", SearchAnalyzer = "name_search" },
                    ["lastName"]  = new TextProperty { Analyzer = "name_autocomplete", SearchAnalyzer = "name_search" },
                    ["fullName"]  = new TextProperty { Analyzer = "name_autocomplete", SearchAnalyzer = "name_search" },
                    ["jobTitle"]  = new KeywordProperty(),
                    ["phone"]     = new KeywordProperty(),
                    ["email"]     = new KeywordProperty()
                }
            }
        });

        if (!createResponse.IsValidResponse)
            throw new Exception($"Failed to create index '{IndexName}': {createResponse.DebugInformation}");

        logger.LogInformation("Index '{Index}' created successfully.", IndexName);
    }

    private static async Task SeedIfEmptyAsync(ElasticsearchClient client, ILogger logger)
    {
        var countResponse = await client.CountAsync(new CountRequest(IndexName));
        if (!countResponse.IsValidResponse)
            throw new Exception($"Failed to count documents in '{IndexName}': {countResponse.DebugInformation}");

        if (countResponse.Count > 0)
        {
            logger.LogInformation("Index '{Index}' already has {Count} documents — skipping seed.", IndexName, countResponse.Count);
            return;
        }

        logger.LogInformation("Seeding {Count} users into Elasticsearch.", SeedUsers.Length);
        var bulkResponse = await client.BulkAsync(b => b
            .IndexMany(SeedUsers, (op, doc) => op.Index(IndexName).Id(doc.Id))
        );

        if (bulkResponse.Errors)
            logger.LogError("Seed bulk index had errors: {Count} failed.", bulkResponse.ItemsWithErrors.Count());
        else
            logger.LogInformation("Seeded {Count} users successfully.", SeedUsers.Length);
    }
}
