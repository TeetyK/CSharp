namespace ShopMusicProject.ViewModels
{
    public class PdVM
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public double? Price { get; set; } = null!;

        public double? Stock { get; set; } = null!;

        public double? Cost { get; set; } = null!;

        public string LastbuyAt { get; set; } = null!;

        public string? LastsaleAt { get; set; }

        public string BrandName { get; set; } = null!;

        public string CategoryName { get; set; } = null!;
    }
}
