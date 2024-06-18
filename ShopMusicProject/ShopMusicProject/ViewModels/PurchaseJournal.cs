namespace ShopMusicProject.ViewModels
{
    public class PurchaseJournal
    {
        public DateOnly? bdate { get; set; }
        public string? PdName { get; set; }
        public string? SupName { get; set; }
        public string? Saleman { get; set; }
        public double? Bqty { get; set; }
        public double? Bmoney { get; set; }
    }
}
