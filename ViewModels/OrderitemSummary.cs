namespace TheSocialCebu_Capstone.ViewModels
{
    public class OrderitemSummary
    {
        public string ProdId { get; set; } = null!;
        public string ProdName { get; set; } = null!;
        public int TotalQty { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string CombinedInstructions { get; set; } = null!;
    }
}
