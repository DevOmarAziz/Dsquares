using LoyaltyApi.Contracts;

namespace LoyaltyApi.Services
{
    public interface IAwardService
    {
        AwardResult CreateAward(AwardRequest request);
        WalletResponse? GetWallet(string walletId);
    }

    public sealed record AwardResult
     (
        AwardResultStatus Status,
        AwardResponse? Award = null
    );

    public enum AwardResultStatus
    {
        Created,
        ExistingMatch,
        Conflict,
        WalletNotFound
    }
}
