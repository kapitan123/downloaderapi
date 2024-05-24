using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace IntegrationTests;
public class TestServer : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseSetting("https_port", "443");

		builder.ConfigureTestServices(services =>
		{
			// services.AddSingleton<IPushNotificationService, PushNotificationServiceMock>(); // Register a mock

		});
	}
}
