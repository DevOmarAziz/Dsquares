using LoyaltyApi.Models;

namespace LoyaltyApi.Data
{
    public readonly record struct AwardKey(
        string SourceId, 
        string ExternalTransactionId);
    
    public sealed class InMemoryStore
    {
        public object syncRoot = new();

        public Dictionary<string, Wallet> Wallets { get; } = new( StringComparer.Ordinal);

        public Dictionary<AwardKey, Award> Awards { get; } = new();
    
        public InMemoryStore()
        {
             Wallets.Add("W100", new Wallet{ WalletId = "W100", Balance = 100 });
             Wallets.Add("W200", new Wallet{ WalletId = "W200", Balance = 50 });
        }

        public void Reset()
        {
            lock (syncRoot)
            {
                Wallets.Clear();
                Awards.Clear();
                Wallets.Add("W100", new Wallet { WalletId = "W100", Balance = 100 });
                Wallets.Add("W200", new Wallet { WalletId = "W200", Balance = 50 });
            }
        }
    }
}
