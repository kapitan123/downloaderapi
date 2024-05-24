using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;
[Collection("Integration Tests")]
public abstract class TestBase : IAsyncLifetime
{
	private readonly TestServer _server;
	private AsyncServiceScope _scope;
	protected IServiceProvider _services;
	protected HttpClient _client;

	public TestBase(TestServer server)
	{
		_server = server;
	}

	public async Task InitializeAsync()
	{
		_client = _server.CreateClient(new WebApplicationFactoryClientOptions
		{
			BaseAddress = new Uri("https://localhost/")
		});
		_scope = _server.Services.CreateAsyncScope();
		_services = _scope.ServiceProvider;
	}

	public async Task DisposeAsync()
	{
		await _scope.DisposeAsync();
	}
}
