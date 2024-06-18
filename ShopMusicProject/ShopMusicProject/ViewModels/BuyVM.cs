namespace ShopMusicProject.ViewModels
{
    public class BuyVM
    {
        public string BuyId { get; set; } = null!;
        public string PdId { get; set; } = null;
        public string PdName { get; set; } = null!;

        public string? SupId { get; set; }
        public string? SupName { get; set; }

        public DateOnly? BuyDate { get; set; }

        public string? StfId { get; set; }

        public string? BuyDocId { get; set; }

        public string? Saleman { get; set; }

        public double? BuyQty { get; set; }

        public double? BuyMoney { get; set; }

        public string? BuyRemark { get; set; }

        public double? BdtlQty { get; set; }
        public double? BdtlPrice { get; set; }
        public double? BdtlMoney { get; set; }
    }
}
