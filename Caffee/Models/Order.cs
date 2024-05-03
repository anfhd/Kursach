namespace Caffee.Models
{
    public class Order
    {
        public int ID { get; set; }
        public Table? Table { get; set; }
        public Status? Status { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastChangingStatusTime { get; set; }
        public Visitor? Visitor { get; set; }
        public Waiter? Waiter { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}