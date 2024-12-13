using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoDB_DotNetCore.Entities.MongoDB
{
    public class L1_Pool_Opened
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string L1Name { get; set; } = null!;

        public DateTime? updatedate { get; set; }

        public DateTime? enddate { get; set; }  // Nullable, as enddate can be null

        public double timespan { get; set; }

        public string signalname { get; set; } = null!;

        public string? value { get; set; }  // Nullable, as value can be a string or null

        public object? filter { get; set; }  // Nullable, can be null

        public object? TypeID { get; set; }  // Nullable, can be null

        public object? Judge { get; set; }  // Nullable, can be null

        public object? Error { get; set; }  // Nullable, can be null

        public object? Warning { get; set; }  // Nullable, can be null
    }
}
