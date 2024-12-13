using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB_DotNetCore.Entities.MongoDB
{
    public class L1Signal_Pool_Active
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string L1Name { get; set; } = null!;

        [BsonElement("updatedate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? updatedate { get; set; }

        [BsonElement("enddate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? enddate { get; set; }

        public double timespan { get; set; }

        public string signalname { get; set; } = null!;

        public override string ToString()
        {
            return $"Id : {Id}, EndDate: {enddate:yyyy-MM-dd HH:mm:ss.fff},UpdateDate: {updatedate:yyyy-MM-dd HH:mm:ss.fff}, SignalName: {signalname}, Value : {value}";
        }

        // Simply use the object type without BsonRepresentation for nullables.
        public object? value { get; set; }

        public object? filter { get; set; }

        public object? TypeID { get; set; }

        public object? Judge { get; set; }

        public object? Error { get; set; }

        public object? Warning { get; set; }

    }
}
