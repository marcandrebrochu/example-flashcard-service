using System.Net;
using FlashcardService.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace FlashcardService.API.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class PostgreSqlTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18.3-alpine").Build();
    
    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    public sealed class DeckControllerTests : IClassFixture<PostgreSqlTests>, IDisposable, IAsyncDisposable
    {
        private readonly WebApplicationFactory<Program> _waf;
        private readonly HttpClient _client;

        public DeckControllerTests(PostgreSqlTests fixture)
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            };

            _waf = new CustomWebApplicationFactory(fixture);
            _client = _waf.CreateClient(clientOptions);
        }

        public void Dispose()
        {
            _waf.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _waf.DisposeAsync();
        }

        private sealed class CustomWebApplicationFactory(PostgreSqlTests fixture) : WebApplicationFactory<Program>
        {
            private readonly string _connectionString = fixture._postgres.GetConnectionString();

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureServices(services =>
                {
                    // services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<ApplicationDbContext>) == service.ServiceType));
                    // services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType));
                    services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_connectionString));
                });
            }
        }

        [Fact]
        public async Task FirstTestUsingTestContainers()
        {
            var response = await _client.GetAsync("/decks", TestContext.Current.CancellationToken);
            
            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await Verify(json);
        }
    }
}