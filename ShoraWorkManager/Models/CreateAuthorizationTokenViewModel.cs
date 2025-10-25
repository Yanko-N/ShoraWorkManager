namespace ShoraWorkManager.Models
{
    public class CreateAuthorizationTokenViewModel
    {
        public string LastToken { get; set; }
        public string Token { get; set; }
        public int TokenId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool isUsed { get; set; }
    }
}
