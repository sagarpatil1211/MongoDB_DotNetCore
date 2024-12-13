using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB_DotNetCore.Entities.MongoDB
{
    public class ProductResult_History
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string L1Name { get; set; } = null!;
        public string productname { get; set; } = null!;

        [BsonElement("updatedate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? updatedate { get; set; }

        [BsonElement("enddate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? enddate { get; set; }

        public int productresult { get; set; } = 0;
        public int productresult_accumulate { get; set; } = 0;

        public double timespan { get; set; }

        public object? productserialnumber { get; set; }

        public bool resultflag { get; set; } = false;

        public override string ToString()
        {
            return $"Id : {Id}, EndDate: {enddate:yyyy-MM-dd HH:mm:ss.fff},UpdateDate: {updatedate:yyyy-MM-dd HH:mm:ss.fff}";
        }



    }
}
