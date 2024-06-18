namespace ShopMusicProject.ViewModels
{
    public class CVM
    {
        public string? CusId { get; set; } = null!;

        public string Fname { get; set; } = null!;

        public string? Lname { get; set; }

        public string Email { get; set; } = null!;

        public string? Address { get; set; }

        public string Password { get; set; } = null!;

        public string Username { get; set; } = null!;

        public DateOnly? CreateAt { get; set; }

        public DateOnly? LoginAt { get; set; }

        public string? Phone { get; set; }
    }
}
