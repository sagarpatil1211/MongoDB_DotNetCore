using Microsoft.EntityFrameworkCore;
using MongoDB_DotNetCore.Entities.MSSQL;

namespace MongoDB_DotNetCore.data
{
    public class SQLDatabaseContext: DbContext
    {
        public SQLDatabaseContext(DbContextOptions<SQLDatabaseContext> options) : base(options) { }
        public DbSet<Sql_L1Signal_Pool> L1Signal_Pool { get; set; }
        public DbSet<Sql_LatestCapturedEndDate> LatestCapturedEndDate { get; set; }
        public DbSet<Sql_Alarm_History> Alarm_History { get; set; }
        public DbSet<Sql_L1_Pool> L1_Pool { get; set; }
        public DbSet<Sql_Tag_Pool> Tag_Pool { get; set; }
        public DbSet<Sql_L1Signal_Pool_Capped> L1Signal_Pool_Capped { get; set; }
        public DbSet<Sql_Operator_History> Operator_History { get; set; }
        public DbSet<Sql_ProductResult_History> ProductResult_History { get; set; }
        public DbSet<Sql_SystemDiagnosis_Machine> SystemDiagnosis_Machine { get; set; }

    }
}
