using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities.MongoDB;
using MongoDB_DotNetCore.Entities.MSSQL;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MongoDB_DotNetCore.Domain.Stream
{
    public class L1_Pool_Opened_Transfer
    {
        private readonly MongoDbContext _mongoDbContext;

        private readonly SQLDatabaseContext _sqlDbContext;
        private readonly IMongoCollection<L1_Pool_Opened> _L1_Pool_Opened;

        private const string TopicName = "KOELKOP/L1_Pool_Opened";

        public L1_Pool_Opened_Transfer(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _L1_Pool_Opened = mongoDbContext.GetCollection<L1_Pool_Opened>("L1_Pool_Opened");
            _sqlDbContext = sqlDbContext;
        }
        public async Task transferL1_Pool_OpenedRecentRecords(MessageHubConnection hubConnection)
        {
            try
            {
                // Execute the query and return the All records
                var records = await _L1_Pool_Opened.Find(FilterDefinition<L1_Pool_Opened>.Empty).ToListAsync();

                JObject ConvertToSerializableRecord(L1_Pool_Opened input)
                {
                    return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));
                }

                List<JObject> recordsToTransmit = records.Select(e => ConvertToSerializableRecord(e)).ToList();

                PayloadPacket packet = new()
                {
                    Topic = TopicName,
                    Payload = recordsToTransmit
                };

                hubConnection?.PublishPayloadPacket(TopicName, packet, true);

                var a = new List<L1_Pool_Opened>();
            }
            catch (Exception ex)
            {

            }


        }

    }
}
