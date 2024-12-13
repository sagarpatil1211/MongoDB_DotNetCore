using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoDB_DotNetCore.Entities.MSSQL
{
    public class Sql_SystemDiagnosis_Machine
    {
        [Key]
        public int Id { get; set; }  // Primary Key (auto-incremented)

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(64)]
        public string? ObjectId { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? PCName { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(255)]
        public string? MachineName { get; set; }

        public int? Status { get; set; } = 0;
        public int? CycleTime { get; set; } = 0;
        public int? ExecuteTime { get; set; } = 0;
        public DateTime? Updatedate { get; set; }
       

    }
}
