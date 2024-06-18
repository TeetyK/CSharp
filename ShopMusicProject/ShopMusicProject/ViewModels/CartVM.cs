namespace ShopMusicProject.ViewModels
{
    public class CartVM
    {
         public string CartId { get; set; } = null!;

        public string PdId { get; set; } = null!;

        public string PdName { get; set; }

        public double? CdtlQty { get; set; }

        public double? CdtlPrice { get; set; }

        public double? CdtlMoney { get; set; }
    }
}
