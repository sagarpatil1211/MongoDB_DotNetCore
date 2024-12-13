using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities.MongoDB;
using MongoDB_DotNetCore.Entities.MSSQL;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MongoDB_DotNetCore.Domain.Stream
{
    public class ProductResult_History_Active_Transfer
    {
        private readonly MongoDbContext _mongoDbContext;

        private readonly SQLDatabaseContext _sqlDbContext;
        private readonly IMongoCollection<ProductResult_History_Active> _ProductResult_History_Active;

        private const string TopicName = "KOELKOP/ProductResult_History_Active";

        public ProductResult_History_Active_Transfer(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _ProductResult_History_Active = mongoDbContext.GetCollection<ProductResult_History_Active>("ProductResult_History_Active");
            _sqlDbContext = sqlDbContext;
        }
        public async Task transferProductResult_History_ActiveRecentRecords(MessageHubConnection hubConnection)
        {
            try
            {
                // Execute the query and return the All records
                var records = await _ProductResult_History_Active.Find(FilterDefinition<ProductResult_History_Active>.Empty).ToListAsync();

                JObject ConvertToSerializableRecord(ProductResult_History_Active input)
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

                var a = new List<ProductResult_History_Active>();
            }
            catch (Exception ex)
            {

            }


        }

    }
}
