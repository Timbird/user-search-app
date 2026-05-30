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
        var existsResponse = await client.Indices.ExistsAsync(IndexName);
        if (!existsResponse.IsValidResponse)
        {
            logger.LogInformation("Creating Elasticsearch index '{Index}'", IndexName);
            await client.Indices.CreateAsync(IndexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Custom("name_autocomplete", ca => ca
                                .Tokenizer("standard")
                                .Filter(["lowercase", "name_edge_ngram"])
                            )
                            .Custom("name_search", ca => ca
                                .Tokenizer("standard")
                                .Filter(["lowercase"])
                            )
                        )
                        .TokenFilters(tf => tf
                            .EdgeNGram("name_edge_ngram", eng => eng
                                .MinGram(2)
                                .MaxGram(20)
                            )
                        )
                    )
                )
                .Mappings(m => m
                    .Properties<UserDocument>(p => p
                        .Keyword(k => k.Id)
                        .Text(t => t.FirstName, cfg => cfg
                            .Analyzer("name_autocomplete")
                            .SearchAnalyzer("name_search"))
                        .Text(t => t.LastName, cfg => cfg
                            .Analyzer("name_autocomplete")
                            .SearchAnalyzer("name_search"))
                        .Text(t => t.FullName, cfg => cfg
                            .Analyzer("name_autocomplete")
                            .SearchAnalyzer("name_search"))
                        .Keyword(k => k.JobTitle)
                        .Keyword(k => k.Phone)
                        .Keyword(k => k.Email)
                    )
                )
            );
        }

        var countResponse = await client.CountAsync(new CountRequest(IndexName));
        if (countResponse.Count == 0)
        {
            logger.LogInformation("Seeding {Count} users into Elasticsearch", SeedUsers.Length);
            var bulkResponse = await client.BulkAsync(b => b
                .IndexMany(SeedUsers, (op, doc) => op.Index(IndexName).Id(doc.Id))
            );

            if (bulkResponse.Errors)
                logger.LogError("Seed bulk index had errors: {Errors}", bulkResponse.ItemsWithErrors.Count());
        }
    }
}
