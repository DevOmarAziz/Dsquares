using System.Net;
using System.Net.Http.Json;
using LoyaltyApi.Contracts;

namespace LoyaltyApi.Tests
{
    public sealed class AwardsApiTests : ApiTestBase
    {
        [Fact]
        public async Task CreateAward_NewAward_Returns201_AndIncreasesBalance()
        {
            var response = await PostAwardAsync(ValidRequest(points: 25));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var award = await response.Content.ReadFromJsonAsync<AwardResponse>();
            Assert.NotNull(award);
            Assert.Equal("SRC-1", award.SourceId);
            Assert.Equal("TX-1001", award.ExternalTransactionId);
            Assert.Equal("W100", award.WalletId);
            Assert.Equal(25, award.Points);
            Assert.NotEqual(Guid.Empty, award.AwardId);

            var wallet = await GetWalletAsync("W100");
            Assert.NotNull(wallet);
            Assert.Equal(125, wallet.Balance);
        }

        [Fact]
        public async Task CreateAward_ResendSameRequest_Returns200_AndBalanceIncreasedOnlyOnce()
        {
            var request = ValidRequest(points: 30);

            var first = await PostAwardAsync(request);
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);
            var firstAward = await first.Content.ReadFromJsonAsync<AwardResponse>();

            var second = await PostAwardAsync(request);
            Assert.Equal(HttpStatusCode.OK, second.StatusCode);
            var secondAward = await second.Content.ReadFromJsonAsync<AwardResponse>();

            Assert.NotNull(firstAward);
            Assert.NotNull(secondAward);
            Assert.Equal(firstAward.AwardId, secondAward.AwardId);

            var wallet = await GetWalletAsync("W100");
            Assert.NotNull(wallet);
            Assert.Equal(130, wallet.Balance);
        }

        [Fact]
        public async Task CreateAward_SameKeyDifferentPoints_Returns409_AndBalanceUnchanged()
        {
            var first = await PostAwardAsync(ValidRequest(points: 10));
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var conflicting = await PostAwardAsync(ValidRequest(points: 99));
            Assert.Equal(HttpStatusCode.Conflict, conflicting.StatusCode);

            var wallet = await GetWalletAsync("W100");
            Assert.NotNull(wallet);
            Assert.Equal(110, wallet.Balance);
        }

        [Fact]
        public async Task CreateAward_SameKeyDifferentWallet_Returns409()
        {
            var first = await PostAwardAsync(ValidRequest(walletId: "W100"));
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var conflicting = await PostAwardAsync(ValidRequest(walletId: "W200"));
            Assert.Equal(HttpStatusCode.Conflict, conflicting.StatusCode);
        }

        [Fact]
        public async Task CreateAward_UnknownWallet_Returns404()
        {
            var response = await PostAwardAsync(ValidRequest(walletId: "W999"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateAward_DifferentTransactionIds_AccumulateBalance()
        {
            var first = await PostAwardAsync(ValidRequest(externalTransactionId: "TX-1", points: 10));
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var second = await PostAwardAsync(ValidRequest(externalTransactionId: "TX-2", points: 20));
            Assert.Equal(HttpStatusCode.Created, second.StatusCode);

            var wallet = await GetWalletAsync("W100");
            Assert.NotNull(wallet);
            Assert.Equal(130, wallet.Balance);
        }

        [Fact]
        public async Task CreateAward_DifferentSourceSameTransaction_Returns201()
        {
            var first = await PostAwardAsync(ValidRequest(sourceId: "SRC-A"));
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var second = await PostAwardAsync(ValidRequest(sourceId: "SRC-B"));
            Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        }

        [Fact]
        public async Task CreateAward_AwardsToDifferentWallets_DoNotAffectEachOther()
        {
            var toW100 = await PostAwardAsync(ValidRequest(externalTransactionId: "TX-1", walletId: "W100", points: 10));
            Assert.Equal(HttpStatusCode.Created, toW100.StatusCode);

            var toW200 = await PostAwardAsync(ValidRequest(externalTransactionId: "TX-2", walletId: "W200", points: 10));
            Assert.Equal(HttpStatusCode.Created, toW200.StatusCode);

            var w100 = await GetWalletAsync("W100");
            var w200 = await GetWalletAsync("W200");

            Assert.NotNull(w100);
            Assert.NotNull(w200);
            Assert.Equal(110, w100.Balance);
            Assert.Equal(60, w200.Balance);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(1001)]
        public async Task CreateAward_InvalidPoints_Returns400(int points)
        {
            var response = await PostAwardAsync(ValidRequest(points: points));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAward_MaxAllowedPoints_Returns201()
        {
            var response = await PostAwardAsync(ValidRequest(points: 1000));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bad id!")]
        [InlineData("has space")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public async Task CreateAward_InvalidSourceId_Returns400(string sourceId)
        {
            var response = await PostAwardAsync(ValidRequest(sourceId: sourceId));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAward_MissingWalletId_Returns400()
        {
            var request = ValidRequest();
            request.WalletId = null;

            var response = await PostAwardAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAward_NullBody_Returns400()
        {
            var response = await Client.PostAsJsonAsync<AwardRequest?>("/Awards", null);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAward_EmptyBody_Returns400()
        {
            using var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/Awards", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAward_ConflictResponse_ContainsMessage()
        {
            await PostAwardAsync(ValidRequest(points: 10));

            var conflicting = await PostAwardAsync(ValidRequest(points: 20));

            Assert.Equal(HttpStatusCode.Conflict, conflicting.StatusCode);
            var body = await conflicting.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.NotNull(body);
            Assert.True(body.ContainsKey("message"));
        }
    }
}
