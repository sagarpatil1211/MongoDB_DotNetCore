namespace MongoDB_DotNetCore.Entities.Sql
{
    public class equipment_product_results
    {
        public long Id { get; set; }
        public DateTime? start { get; set; }   
        public DateTime? end { get; set; }
        public string? equipmentName { get; set; }
        public string? productName { get; set; }
        public int? Base { get; set; } = 0;
        public int? increment { get; set; } = 0;
    }
}
