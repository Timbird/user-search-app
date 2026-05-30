using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using UserSearch.Api.Models;

namespace UserSearch.Api.Repositories;

public class ElasticsearchUserRepository(ElasticsearchClient client) : IUserRepository
{
    private const string IndexName = "users";

    public async Task<IEnumerable<string>> AutocompleteAsync(string query)
    {
        var response = await client.SearchAsync<UserDocument>(s => s
            .Indices(IndexName)
            .Size(10)
            .Source(s => s.Filter(f => f.Includes(d => d.FirstName, d => d.LastName)))
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(query)
                    .Fields(f => f.FirstName, f => f.LastName, f => f.FullName)
                    .Type(TextQueryType.MostFields)
                )
            )
        );

        return response.Documents
            .Select(d => $"{d.FirstName} {d.LastName}")
            .Distinct();
    }

    public async Task<IEnumerable<User>> SearchAsync(string query)
    {
        var response = await client.SearchAsync<UserDocument>(s => s
            .Indices(IndexName)
            .Size(50)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(query)
                    .Fields(f => f.FirstName, f => f.LastName, f => f.FullName)
                    .Type(TextQueryType.MostFields)
                )
            )
        );

        return response.Documents.Select(d => d.ToUser());
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var response = await client.CountAsync<UserDocument>(s => s
            .Indices(IndexName)
            .Query(q => q
                .Term(t => t
                    .Field(f => f.Email)
                    .Value(email.ToLowerInvariant())
                )
            )
        );

        return response.Count > 0;
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Id = Guid.NewGuid().ToString();
        var doc = UserDocument.FromUser(user);

        await client.IndexAsync(new IndexRequest<UserDocument>(doc, IndexName, doc.Id));

        return user;
    }
}
