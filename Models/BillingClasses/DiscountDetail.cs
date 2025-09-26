namespace TheSocialCebu_Capstone.Models.BillingClasses
{
    public partial class DiscountDetail
    {
        public string DiscountDetailId { get; set; } = null!;
        public string BillingId { get; set; } = null!;
        public string DiscountTypeId { get; set; } = null!;
        public int? NumOfCustomer { get; set; }
        public int? NumOfDiscountHolder { get; set; }

        public virtual Billing Billing { get; set; } = null!;
        public virtual DiscountType DiscountType { get; set; } = null!;
    }
}
