using LoyaltyApi.Contracts;
using LoyaltyApi.Data;
using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public class AwardService(InMemoryStore store) : IAwardService
    {
        public AwardResult CreateAward(AwardRequest request)
        {
            var key = new AwardKey(request.SourceId!,request.ExternalTransactionId!);

            lock (store.syncRoot)
            {
                if(store.Awards.TryGetValue(key, out var existingAward))
                {
                    bool sameRequest = 
                        existingAward.WalletId == request.WalletId &&
                        existingAward.Points == request.Points;

                    return sameRequest
                        ? new AwardResult (AwardResultStatus.ExistingMatch, ToResponse(existingAward))
                        : new AwardResult (AwardResultStatus.Conflict);
                }

                if(!store.Wallets.TryGetValue(request.WalletId!, out var wallet))
                    return new AwardResult(AwardResultStatus.WalletNotFound);
                
                var award = new Award
                {
                    AwardId = Guid.NewGuid(),
                    SourceId = request.SourceId!,
                    ExternalTransactionId = request.ExternalTransactionId!,
                    WalletId = request.WalletId!,
                    Points = request.Points!.Value
                };

                wallet.Balance += award.Points;
                store.Awards.Add(key, award);

                return new AwardResult(AwardResultStatus.Created, ToResponse(award));
            }
        }

        public WalletResponse? GetWallet(string walletId)
        {
            lock (store.syncRoot)
            {
                if(!store.Wallets.TryGetValue(walletId, out var wallet))
                    return null;
                
                return new WalletResponse(wallet.WalletId, wallet.Balance);
            }
        }

        private static AwardResponse ToResponse(Award award) =>
        new(award.AwardId, award.SourceId, award.ExternalTransactionId,
            award.WalletId, award.Points);
    }
}
