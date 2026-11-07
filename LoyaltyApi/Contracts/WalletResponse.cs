namespace LoyaltyApi.Contracts
{
    public sealed record WalletResponse
    (
        string WalletId,
        int Balance
    );
}
