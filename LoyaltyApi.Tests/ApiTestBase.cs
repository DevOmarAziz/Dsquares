using System.Net.Http.Json;
using LoyaltyApi.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LoyaltyApi.Tests
{
    public abstract class ApiTestBase : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;

        protected HttpClient Client { get; }

        protected ApiTestBase()
        {
            _factory = new WebApplicationFactory<Program>();
            Client = _factory.CreateClient();
        }

        protected Task<HttpResponseMessage> PostAwardAsync(AwardRequest request) =>
            Client.PostAsJsonAsync("/Awards", request);

        protected async Task<WalletResponse?> GetWalletAsync(string walletId)
        {
            var response = await Client.GetAsync($"/Wallets/{walletId}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<WalletResponse>()
                : null;
        }

        protected static AwardRequest ValidRequest(
            string sourceId = "SRC-1",
            string externalTransactionId = "TX-1001",
            string walletId = "W100",
            int points = 10) =>
            new()
            {
                SourceId = sourceId,
                ExternalTransactionId = externalTransactionId,
                WalletId = walletId,
                Points = points
            };

        public void Dispose()
        {
            Client.Dispose();
            _factory.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
