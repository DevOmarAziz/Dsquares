namespace LoyaltyApi.Models
{
    public sealed class Wallet
    {
        public required string WalletId { get; set; }
        public int Balance { get; set; }
    }
}
