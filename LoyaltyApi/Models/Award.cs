namespace LoyaltyApi.Models
{
    public sealed class Award
    {
        public required Guid AwardId { get; set; }
        public required string SourceId { get; set; }
        public required string ExternalTransactionId { get; set; }
        public required string WalletId { get; set; }
        public int Points { get; set; }
    }
}
