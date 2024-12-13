namespace MongoDB_DotNetCore.Entities.Sql
{
    public class equipment_machines
    {
        public long Id { get; set; }
        public long equipmenId { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public int enabled { get; set; } = 0;
        public string? type { get; set; }
        public string? host { get; set; }
        public int? port { get; set; }
        public int? timeout { get; set; }   
        public int? pathCount { get; set; } 
        public int? cncTimeSyncCycle { get; set; }
    }
}
