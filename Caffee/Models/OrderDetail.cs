namespace Caffee.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }
        public Dish? Dish { get; set; }
        public int Amount {  get; set; }
        public double UnitPrice { get; set; }
    }
}