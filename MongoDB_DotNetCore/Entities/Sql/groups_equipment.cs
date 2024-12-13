namespace MongoDB_DotNetCore.Entities.Sql
{
    public class groups_equipment
    {
        public long Id { get; set; }
        public long groupId { get; set; }   
        public string name { get; set; }
        public string displayName { get; set; }
        public int enabled { get; set; } = 0;    
    }
}
