namespace Caffee.Models
{
    public class Discount
    {
        public int ID { get; set; }
        public DiscountType Type { get; set; }
        public double Value { get; set; }
    }
}