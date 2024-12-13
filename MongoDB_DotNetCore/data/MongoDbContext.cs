using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace MongoDB_DotNetCore.data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];
            //var client = new MongoClient(connectionString);

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);

            settings.ClusterConfigurator = builder =>
            {
                builder.Subscribe<CommandStartedEvent>(e =>
                {
                    Console.WriteLine($"Command {e.CommandName} started with details {e.Command.ToJson()}");
                });

                builder.Subscribe<CommandFailedEvent>(e =>
                {
                    Console.WriteLine($"Command {e.CommandName} failed with details {e.Failure.Message}");
                });
            };

            var client = new MongoClient(settings);


            _database = client.GetDatabase(databaseName);
            Console.WriteLine("Connection Ok");
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
