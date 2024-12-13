using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities.MongoDB;
using MongoDB_DotNetCore.Entities.MSSQL;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MongoDB_DotNetCore.Domain.Stream
{
    public class Tag_Pool_Active_Transfer
    {
        private readonly MongoDbContext _mongoDbContext;

        private readonly SQLDatabaseContext _sqlDbContext;
        private readonly IMongoCollection<Tag_Pool_Active> _Tag_Pool_Active;

        private const string TopicName = "KOELKOP/Tag_Pool_Active";

        public Tag_Pool_Active_Transfer(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _Tag_Pool_Active = mongoDbContext.GetCollection<Tag_Pool_Active>("Tag_Pool_Active");
            _sqlDbContext = sqlDbContext;
        }

        public async Task transferTag_Pool_ActiveRecentRecords(MessageHubConnection hubConnection)
        {
            try
            {
                Console.WriteLine("Before get Tag_Pool_Active records.");
                // Execute the query and return the All records
                var records = await _Tag_Pool_Active.Find(FilterDefinition<Tag_Pool_Active>.Empty).ToListAsync();
                Console.WriteLine("After get Tag_Pool_Active records.");

                JObject ConvertToSerializableRecord(Tag_Pool_Active input)
                {
                    return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));
                }

                List<JObject> recordsToTransmit = records.Select(e => ConvertToSerializableRecord(e)).ToList();

                PayloadPacket packet = new()
                {
                    Topic = TopicName,
                    Payload = recordsToTransmit
                };
                Console.WriteLine("Before Publish Tag_Pool_Active records.");
                hubConnection?.PublishPayloadPacket(TopicName, packet, true);
                Console.WriteLine("After Publish Tag_Pool_Active records.");

                var a = new List<Tag_Pool_Active>();
            }
            catch (Exception ex)
            {

            }


        }

    }
}
