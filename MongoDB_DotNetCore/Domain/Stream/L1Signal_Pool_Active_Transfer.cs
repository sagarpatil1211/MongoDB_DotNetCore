using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities.MongoDB;
using MongoDB_DotNetCore.Entities.MSSQL;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MongoDB_DotNetCore.Domain.Stream
{
    public class L1Signal_Pool_Active_Transfer
    {
        private readonly MongoDbContext _mongoDbContext;

        private readonly SQLDatabaseContext _sqlDbContext;
        private readonly IMongoCollection<L1Signal_Pool_Active> _L1Signal_Pool_Active;

        private const string TopicName = "KOELKOP/L1Signal_Pool_Active";

        public L1Signal_Pool_Active_Transfer(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _L1Signal_Pool_Active = mongoDbContext.GetCollection<L1Signal_Pool_Active>("L1Signal_Pool_Active");
            _sqlDbContext = sqlDbContext;
        }
        public async Task transferL1Signal_Pool_ActiveRecentRecords(MessageHubConnection hubConnection)
        {
            try
            {
                // Execute the query and return the All records
                var records = await _L1Signal_Pool_Active.Find(FilterDefinition<L1Signal_Pool_Active>.Empty).ToListAsync();

                JObject ConvertToSerializableRecord(L1Signal_Pool_Active input)
                {
                    return new()
                    {
                        ["L1Name"] = input.L1Name,
                        ["updatedate"] = input.updatedate,
                        ["timespan"] = input.timespan,
                        ["signalname"] = input.signalname,
                        ["value"] = input.value is null ? null : JToken.FromObject(input.value)
                    };
                }

                List<JObject> recordsToTransmit = records.Select(e => ConvertToSerializableRecord(e)).ToList();

                PayloadPacket packet = new()
                {
                    Topic = TopicName,
                    Payload = recordsToTransmit
                };

                hubConnection?.PublishPayloadPacket(TopicName, packet, true);

                var a = new List<L1Signal_Pool_Active>();
            }
            catch (Exception ex)
            {

            }


        }

    }
}
