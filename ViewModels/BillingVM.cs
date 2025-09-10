using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class BillingVM
    {
        public decimal Subtotal { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public Table Table { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
