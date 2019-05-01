using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MicroServicesOnDocker.Services.CartApi.Model
{
    public class RedisCartRepository : ICartRepository
    {
        private readonly ILogger<RedisCartRepository> _logger;
        //Redis comes already with .net core no nuget pack is needed
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCartRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisCartRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteCartAsync(string cardId)
        {
            return await _database.KeyDeleteAsync(cardId);
        }

        

        public async Task<Cart> GetCartAsync(string cardId)
        {
            var data = await _database.StringGetAsync(cardId);
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Cart>(data);
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            var created = await _database.StringSetAsync(cart.CardId, JsonConvert.SerializeObject(cart));
            if (!created)
            {
                _logger.LogInformation("Problem occur when persisting the cart.");
                return null;
            }

            _logger.LogInformation("Cart item is successfully persisted.");

            return await GetCartAsync(cart.CardId);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();
            return data?.Select(k => k.ToString());
        }

    }
}
