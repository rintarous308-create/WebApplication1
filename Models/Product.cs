namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }             // id_pd
        public string? Name { get; set; }       // name_pd
        public double Price { get; set; }       // price_pd
        public string? Detail { get; set; }     // detail_pd
        public int TypeId { get; set; }         // id_pt (Foreign Key)
        public string? TypeName { get; set; }   // name_pt (มาจากการ JOIN)
    }
}
