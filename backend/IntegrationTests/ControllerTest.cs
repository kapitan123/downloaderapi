using System.Net;
using FluentAssertions;

namespace IntegrationTests;

[Collection("Integration Tests")]
public class ControllerTest(TestServer server) : TestBase(server)
{
	[Fact]
	public async Task TestTemplates()
	{
		var testUrl = @"";
		var response = await _client.GetAsync(testUrl);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var result = await response.Content.ReadAsStringAsync();
		result.Should().Be("");
	}
}