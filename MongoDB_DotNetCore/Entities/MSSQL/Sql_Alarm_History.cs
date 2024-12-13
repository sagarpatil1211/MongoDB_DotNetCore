using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoDB_DotNetCore.Entities.MSSQL
{
    public class Sql_Alarm_History
    {
        [Key]
        public int Id { get; set; }  // Primary Key (auto-incremented)

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(64)]
        public string? ObjectId { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? L1Name { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? L0Name { get; set; }


        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? number { get; set; }
        public DateTime? updatedate { get; set; }
       
        public DateTime? enddate { get; set; }
        public double timespan { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? message { get; set; } = "";

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? type { get; set; }

        public int? level { get; set; }
    }
}
