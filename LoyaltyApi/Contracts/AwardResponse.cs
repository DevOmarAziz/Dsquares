namespace LoyaltyApi.Contracts
{
    public sealed record AwardResponse(
        Guid AwardId,
        string SourceId,
        string ExternalTransactionId,
        string WalletId,
        int Points
    );
}
