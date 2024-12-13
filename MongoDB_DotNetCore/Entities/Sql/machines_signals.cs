namespace MongoDB_DotNetCore.Entities.Sql
{
    public class machines_signals
    {
        public long Id { get; set; }
        public long machineId { get; set; }   
        public string name { get; set; }
        public string? displayName { get; set; }
        public string? dataType {  get; set; }   
        public int? enabled { get; set; } = 0;  
        public int? readCycle { get; set; }
        public string? category { get; set; }
        public string? label { get; set; }
        public int? path { get; set; }
    }
}
