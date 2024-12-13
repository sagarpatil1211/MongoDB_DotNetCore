using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MongoDB_DotNetCore.Entities.MSSQL
{
    public class Sql_LatestCapturedEndDate
    {
        [Key]
        public int Id { get; set; }  // Primary Key (auto-incremented)

        [Column(TypeName = "NVARCHAR")]
        [MaxLength(256)]
        public string? collectionName { get; set; }

        public DateTime? LatestCapturedEndDate { get; set; }
    }
}
