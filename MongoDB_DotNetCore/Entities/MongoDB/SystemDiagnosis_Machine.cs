using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB_DotNetCore.Entities.MongoDB
{
    public class SystemDiagnosis_Machine

    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? PCName { get; set; } = null!;

        public string? MachineName { get; set; } = "";

        public int? Status { get; set; } = 0;
        public int? CycleTime { get; set; } = 0;
        public int? ExecuteTime { get; set; } = 0;

        [BsonElement("Updatedate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Updatedate { get; set; }




        // Simply use the object type without BsonRepresentation for nullables.
        //public object? value { get; set; }


    }
}
