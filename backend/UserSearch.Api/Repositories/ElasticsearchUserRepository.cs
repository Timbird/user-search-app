using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using UserSearch.Api.Models;

namespace UserSearch.Api.Repositories;

public class ElasticsearchUserRepository(ElasticsearchClient client) : IUserRepository
{
    private const string IndexName = "users";

    public async Task<IEnumerable<string>> AutocompleteAsync(string query)
    {
        var response = await client.SearchAsync<UserDocument>(s => s
            .Index(IndexName)
            .Size(10)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(query)
                    .Fields(new[] { "firstName", "lastName", "fullName" })
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
            .Index(IndexName)
            .Size(50)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(query)
                    .Fields(new[] { "firstName", "lastName", "fullName" })
                    .Type(TextQueryType.MostFields)
                )
            )
        );

        return response.Documents.Select(d => d.ToUser());
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var response = await client.SearchAsync<UserDocument>(s => s
            .Index(IndexName)
            .Size(1)
            .Query(q => q
                .Term(t => t
                    .Field("email")
                    .Value(email.ToLowerInvariant())
                )
            )
        );

        return response.Total > 0;
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Id = Guid.NewGuid().ToString();
        var doc = UserDocument.FromUser(user);

        await client.IndexAsync(new IndexRequest<UserDocument>(doc, IndexName, doc.Id));

        return user;
    }
}
