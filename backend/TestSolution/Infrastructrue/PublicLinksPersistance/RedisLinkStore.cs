using DocumentStore.Infrastructrue.FileSystem;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using StackExchange.Redis;

namespace DocumentStore.Infrastructrue.PublicLinksPersistance;

public class RedisLinkStore : IPublicLinkStore
{
	private readonly IConnectionMultiplexer _connectionMultiplexer;
	private readonly IDatabase _database;

	public RedisLinkStore(IOptions<RedisSettings> options)
	{
		_connectionMultiplexer = ConnectionMultiplexer.Connect(options.Value.ConnectionString);
		_database = _connectionMultiplexer.GetDatabase();
	}

	public async Task Save(string publicId, Guid documentId, int expirationInHours, CancellationToken token)
	{
		var redisValue = documentId.ToString();
		var expiration = TimeSpan.FromHours(expirationInHours);

		await _database.StringSetAsync(publicId, redisValue, expiration);
	}

	public async Task<OneOf<Guid, NotFound>> Get(string publicId, CancellationToken token)
	{
		var redisValue = await _database.StringGetAsync(publicId);

		if (redisValue.IsNullOrEmpty)
		{
			return new NotFound();
		}

		return Guid.Parse(redisValue);
	}
}