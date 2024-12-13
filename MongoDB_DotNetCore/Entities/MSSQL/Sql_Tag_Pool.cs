using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoDB_DotNetCore.Entities.MSSQL
{
    public class Sql_Tag_Pool
    {
        [Key]
        public int Id { get; set; }  // Primary Key (auto-incremented)

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(64)]
        public string? ObjectId { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? TagName { get; set; }
        public DateTime? updatedate { get; set; }
       
        public DateTime? enddate { get; set; }
        public double timespan { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? signalname { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? value { get; set; }
    }
}
