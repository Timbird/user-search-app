using Elastic.Clients.Elasticsearch;
using UserSearch.Api.Infrastructure;
using UserSearch.Api.Repositories;
using UserSearch.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var esUri = builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
builder.Services.AddSingleton(_ => new ElasticsearchClient(new Uri(esUri)));

builder.Services.AddScoped<IUserRepository, ElasticsearchUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var esClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ElasticsearchSetup.EnsureIndexAsync(esClient, logger);
}

app.Run();

public partial class Program { }
