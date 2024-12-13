using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB_DotNetCore.Entities.MongoDB
{
    public class Alarm_History
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string L1Name { get; set; } = null!;
        public string L0Name { get; set; } = null!;
        public string number { get; set; } = null!;
        public string message { get; set; } = null!;



        [BsonElement("updatedate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? updatedate { get; set; }

        [BsonElement("enddate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? enddate { get; set; }

        public int? level { get; set; }

        public string? type { get; set; }

        public double timespan { get; set; }


        public override string ToString()
        {
            return $"Id : {Id}, EndDate: {enddate:yyyy-MM-dd HH:mm:ss.fff},UpdateDate: {updatedate:yyyy-MM-dd HH:mm:ss.fff}, number: {number}, level : {level}";
        }



    }
}
