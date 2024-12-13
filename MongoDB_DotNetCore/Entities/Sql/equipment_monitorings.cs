namespace MongoDB_DotNetCore.Entities.Sql
{
    public class equipment_monitorings
    {
        public long Id { get; set; }
        public long equipmenId { get; set; }   
        public string name { get; set; }
        public string displayName { get; set; }
        public string machineName { get; set; }
        public string? formula { get; set; }
        public string? dataType { get; set; }
    }
}
