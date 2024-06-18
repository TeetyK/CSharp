namespace ShopMusicProject.ViewModels
{
    public class StockJournal
    {
        public string? PdName { get; set; }

       public string? BrandName { get; set; }

        public string? CategoryName { get; set; }

        public double? StockLimitMore { get; set; }

        //public double Cqty { get; set; }
        public double? StockLimitLess {  get; set; }
        public double? Cprice { get; set; }

        public double? CremainW { get; set; }

        //public string? status { get; set; }
    }
}
