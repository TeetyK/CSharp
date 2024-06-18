namespace ShopMusicProject.ViewModels
{
    public class SaleJournal
    {
        public DateOnly? date {  get; set; }
        public string PdName { get; set; }

        public string CusName { get; set; }

        public double? Cqty { get; set;}

        public double? Cmoney { get; set;}
    }
}
