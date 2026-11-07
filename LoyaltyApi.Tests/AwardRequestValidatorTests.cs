using LoyaltyApi.Contracts;

namespace LoyaltyApi.Tests
{
    public sealed class AwardRequestValidatorTests
    {
        [Fact]
        public void Validate_ValidRequest_ReturnsNoErrors()
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "SRC-1",
                ExternalTransactionId = "TX_1001",
                WalletId = "W100",
                Points = 10
            });

            Assert.Empty(errors);
        }

        [Fact]
        public void Validate_NullRequest_ReturnsError()
        {
            var errors = AwardRequestValidator.Validate(null);

            Assert.Contains("AwardRequest", errors.Keys);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1001)]
        [InlineData(5000)]
        public void Validate_PointsOutOfRange_ReturnsError(int? points)
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "SRC-1",
                ExternalTransactionId = "TX-1",
                WalletId = "W100",
                Points = points
            });

            Assert.Contains("points", errors.Keys);
        }

        [Fact]
        public void Validate_MaxAllowedPoints_ReturnsNoErrors()
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "SRC-1",
                ExternalTransactionId = "TX-1",
                WalletId = "W100",
                Points = 1000
            });

            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("has space")]
        [InlineData("bad!char")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Validate_InvalidSourceId_ReturnsError(string? sourceId)
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = sourceId,
                ExternalTransactionId = "TX-1",
                WalletId = "W100",
                Points = 10
            });

            Assert.Contains("sourceId", errors.Keys);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bad id")]
        public void Validate_InvalidExternalTransactionId_ReturnsError(string? externalTransactionId)
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "SRC-1",
                ExternalTransactionId = externalTransactionId,
                WalletId = "W100",
                Points = 10
            });

            Assert.Contains("externalTransactionId", errors.Keys);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bad id")]
        public void Validate_InvalidWalletId_ReturnsError(string? walletId)
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "SRC-1",
                ExternalTransactionId = "TX-1",
                WalletId = walletId,
                Points = 10
            });

            Assert.Contains("walletId", errors.Keys);
        }

        [Fact]
        public void Validate_MultipleInvalidFields_ReturnsAllErrors()
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "",
                ExternalTransactionId = "",
                WalletId = "",
                Points = 0
            });

            Assert.Equal(4, errors.Count);
        }

        [Fact]
        public void Validate_IdentifierWithUnderscoreAndHyphen_IsAccepted()
        {
            var errors = AwardRequestValidator.Validate(new AwardRequest
            {
                SourceId = "src_1-2",
                ExternalTransactionId = "TX_1001-A",
                WalletId = "W-100_1",
                Points = 1
            });

            Assert.Empty(errors);
        }
    }
}
