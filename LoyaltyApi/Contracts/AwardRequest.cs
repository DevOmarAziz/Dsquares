namespace LoyaltyApi.Contracts
{
    public sealed class AwardRequest
    {
        public string? SourceId { get; set; }
        public string? ExternalTransactionId { get; set; }
        public string? WalletId { get; set; }
        public int? Points { get; set; }
    }
}
