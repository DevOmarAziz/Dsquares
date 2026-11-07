using System.Net;
using System.Net.Http.Json;
using LoyaltyApi.Contracts;

namespace LoyaltyApi.Tests
{
    public sealed class WalletsApiTests : ApiTestBase
    {
        [Fact]
        public async Task GetWallet_ExistingWallet_Returns200_WithInitialBalance()
        {
            var response = await Client.GetAsync("/Wallets/W100");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var wallet = await response.Content.ReadFromJsonAsync<WalletResponse>();
            Assert.NotNull(wallet);
            Assert.Equal("W100", wallet.WalletId);
            Assert.Equal(100, wallet.Balance);
        }

        [Fact]
        public async Task GetWallet_SecondSeededWallet_Returns200()
        {
            var wallet = await GetWalletAsync("W200");

            Assert.NotNull(wallet);
            Assert.Equal(50, wallet.Balance);
        }

        [Fact]
        public async Task GetWallet_UnknownWallet_Returns404()
        {
            var response = await Client.GetAsync("/Wallets/W999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetWallet_AfterAward_ReflectsUpdatedBalance()
        {
            var award = await PostAwardAsync(ValidRequest(points: 40));
            Assert.Equal(HttpStatusCode.Created, award.StatusCode);

            var wallet = await GetWalletAsync("W100");

            Assert.NotNull(wallet);
            Assert.Equal(140, wallet.Balance);
        }
    }
}
